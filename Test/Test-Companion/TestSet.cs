namespace TestCompanion;

using System.Drawing;
using System.IO;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using ResourceTools;

public static class TestSet
{
    public static void SetLogger(ILogger logger)
    {
        ResourceLoader.SetLogger(logger);
    }

    public static void ClearLogger()
    {
        ResourceLoader.ClearLogger();
    }

    public static ILogger? Logger => ResourceLoader.Logger;

    public static bool LoadIcon(string resourceName, string assemblyName, out Icon value)
    {
        return ResourceLoader.LoadIcon(resourceName, assemblyName, out value);
    }

    public static bool LoadIcon(string resourceName, string assemblyName, out ImageSource value)
    {
        return ResourceLoader.LoadIcon(resourceName, assemblyName, out value);
    }

    public static bool LoadBitmap(string resourceName, string assemblyName, out Bitmap value)
    {
        return ResourceLoader.LoadBitmap(resourceName, assemblyName, out value);
    }

    public static bool Load<TResource>(string resourceName, string assemblyName, out TResource value)
        where TResource : class
    {
        return ResourceLoader.Load(resourceName, assemblyName, out value);
    }

    public static bool LoadStream(string resourceName, string assemblyName, out Stream resourceStream)
    {
        return ResourceLoader.LoadStream(resourceName, assemblyName, out resourceStream);
    }
}
