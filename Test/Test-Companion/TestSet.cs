namespace TestCompanion;

using System.Drawing;
using System.IO;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using ResourceTools;

/// <summary>
/// Represents a test set.
/// </summary>
public static class TestSet
{
    /// <inheritdoc cref="ResourceLoader.SetLogger(ILogger)" />
    public static void SetLogger(ILogger logger) => ResourceLoader.SetLogger(logger);

    /// <inheritdoc cref="ResourceLoader.ClearLogger" />
    public static void ClearLogger() => ResourceLoader.ClearLogger();

    /// <inheritdoc cref="ResourceLoader.Logger" />
    public static ILogger? Logger => ResourceLoader.Logger;

    /// <inheritdoc cref="ResourceLoader.LoadIcon(string, string, out Icon)" />
    public static bool LoadIcon(string resourceName, string assemblyName, out Icon value) => ResourceLoader.LoadIcon(resourceName, assemblyName, out value);

    /// <inheritdoc cref="ResourceLoader.LoadIcon(string, string, out ImageSource)" />
    public static bool LoadIcon(string resourceName, string assemblyName, out ImageSource value) => ResourceLoader.LoadIcon(resourceName, assemblyName, out value);

    /// <inheritdoc cref="ResourceLoader.LoadBitmap(string, string, out Bitmap)" />
    public static bool LoadBitmap(string resourceName, string assemblyName, out Bitmap value) => ResourceLoader.LoadBitmap(resourceName, assemblyName, out value);

    /// <inheritdoc cref="ResourceLoader.Load" />
    public static bool Load<TResource>(string resourceName, string assemblyName, out TResource value)
        where TResource : class => ResourceLoader.Load(resourceName, assemblyName, out value);

    /// <inheritdoc cref="ResourceLoader.LoadStream(string, string, out Stream)" />
    public static bool LoadStream(string resourceName, string assemblyName, out Stream resourceStream) => ResourceLoader.LoadStream(resourceName, assemblyName, out resourceStream);
}
