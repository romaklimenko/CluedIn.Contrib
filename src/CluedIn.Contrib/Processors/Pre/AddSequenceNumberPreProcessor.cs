using System.Globalization;
using CluedIn.Contrib.Extensions;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Processing.Processors.PreProcessing;
using Microsoft.Data.SqlClient;
using Serilog;
using ExecutionContext = CluedIn.Core.ExecutionContext;

namespace CluedIn.Contrib.Processors.Pre;

public class AddSequenceNumberPreProcessor : IPreProcessor
{
    private const string SOrigin = "MDM_ID";
    private static bool s_tableCreated;
    private static readonly object s_lock = new();

    public bool Accepts(ExecutionContext context, IEnumerable<IEntityCode> codes)
    {
        return this.IsEnabled();
    }

    public void Process(ExecutionContext context, IEntityMetadataPart metadata, IDataPart? data)
    {
        if (data?.EntityData == null)
        {
            return;
        }

        var connectionString = context.ApplicationContext.System.ConnectionStrings
            .GetConnectionString(Constants.Configuration.ConnectionStrings.CluedInEntities);

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        EnsureSequenceNumbersTable(connection);

        var sequenceNumber = GetSequenceNumber(connection, data.EntityData.OriginEntityCode);

        data.EntityData.Codes.Add(new EntityCode(data.EntityData.EntityType,
            CodeOrigin.CluedIn.CreateSpecific(SOrigin), sequenceNumber));
    }

    private static int GetSequenceNumber(SqlConnection connection, IEntityCode entityCode)
    {
        var entityCodeString = entityCode.ToString();

        if (string.IsNullOrEmpty(entityCodeString))
        {
            Log.Warning($"{nameof(AddSequenceNumberPreProcessor)}: EntityCode is null or empty.");
            return 0;
        }

        var entityCodeHash = entityCodeString.ToGuid();
        var sequenceNumber = SelectSequenceNumber(connection, entityCodeHash);

        if (sequenceNumber != 0)
        {
            return sequenceNumber;
        }

        InsertSequenceNumber(connection, entityCodeString.Truncate(255), entityCodeHash);

        // Re-read the sequence number.
        sequenceNumber = SelectSequenceNumber(connection, entityCodeHash);

        if (sequenceNumber == 0)
        {
#pragma warning disable CA2201
            throw new ApplicationException(
                $"{nameof(AddSequenceNumberPreProcessor)}: Failed to get SequenceNumber.");
#pragma warning restore CA2201
        }

        return sequenceNumber;
    }

    private static void InsertSequenceNumber(SqlConnection connection, string entityCodeTruncated, Guid entityCodeHash)
    {
        using var insertCmd = new SqlCommand(
            "INSERT INTO SequenceNumbers (EntityCode, EntityCodeHash) VALUES (@EntityCode, @EntityCodeHash)",
            connection);
        insertCmd.Parameters.AddWithValue("@EntityCode", entityCodeTruncated);
        insertCmd.Parameters.AddWithValue("@EntityCodeHash", entityCodeHash);

        try
        {
            insertCmd.ExecuteNonQuery();
        }
        // 2601: Indicates a unique constraint violation.
        // 2627: Indicates a primary key violation.
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            // A duplicate key error occurred (someone else inserted the same key concurrently).
            // We simply ignore the error and re-read the sequence number.
            Log.Warning(
                $"{nameof(AddSequenceNumberPreProcessor)}: Insert failed due to duplicate key. Retrying select.");
        }
    }

    private static int SelectSequenceNumber(SqlConnection connection, Guid entityCodeHash)
    {
        using var selectCmd =
            new SqlCommand(
                "SELECT SequenceNumber FROM SequenceNumbers WHERE EntityCodeHash = @EntityCodeHash",
                connection);
        selectCmd.Parameters.AddWithValue("@EntityCodeHash", entityCodeHash);

        var result = selectCmd.ExecuteScalar();
        return result != null ? Convert.ToInt32(result, CultureInfo.InvariantCulture) : 0;
    }

    private static void EnsureSequenceNumbersTable(SqlConnection connection)
    {
        if (s_tableCreated)
        {
            return;
        }

        lock (s_lock)
        {
            const string createTableSql = @"
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SequenceNumbers]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[SequenceNumbers] (
            [SequenceNumber] INT IDENTITY (1, 1) NOT NULL,
            [EntityCode]     NVARCHAR (255) NOT NULL,
            [EntityCodeHash] UNIQUEIDENTIFIER NOT NULL,
            CONSTRAINT [PK_SequenceNumbers] PRIMARY KEY CLUSTERED ([SequenceNumber] ASC)
        );
    END";
            using (var cmd = new SqlCommand(createTableSql, connection))
            {
                cmd.ExecuteNonQuery();
            }

            const string createIndexSql = @"
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_SequenceNumbers_EntityCodeHash' AND object_id = OBJECT_ID(N'[dbo].[SequenceNumbers]'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX [UX_SequenceNumbers_EntityCodeHash]
        ON [dbo].[SequenceNumbers]([EntityCodeHash] ASC);
    END";
            using (var cmd = new SqlCommand(createIndexSql, connection))
            {
                cmd.ExecuteNonQuery();
            }

            s_tableCreated = true;
        }
    }
}
