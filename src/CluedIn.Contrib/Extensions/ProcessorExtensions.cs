using CluedIn.Core.Configuration;
using CluedIn.Processing;
using CluedIn.Processing.Processors.PreProcessing;

namespace CluedIn.Contrib.Extensions;

public static class ProcessorExtensions
{
    private static bool IsEnabled(Type type)
    {
        return ConfigurationManagerEx.AppSettings.GetFlag($"{type.Name}.Enabled", false);
    }

    public static bool IsEnabled(this IPreProcessor processor)
    {
        return IsEnabled(processor.GetType());
    }

    public static bool IsEnabled(this IMergeDataPartsPostProcessor processor)
    {
        return IsEnabled(processor.GetType());
    }
}
