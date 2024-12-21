namespace ResourceTools;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Contracts;
using Microsoft.Extensions.Logging;

/// <summary>
/// This class provides an interface to load embedded resources of a specific type, including those compressed with Costura Fody.
/// </summary>
public static class ResourceLoader
{
    #region Logger
    /// <summary>
    /// Sets a tracing object for diagnostic purpose.
    /// </summary>
    /// <param name="logger">The tracing object.</param>
    public static void SetLogger(ILogger logger)
    {
        Contract.RequireNotNull(logger, out ILogger Logger);

        ResourceLoader.Logger = Logger;
    }

    /// <summary>
    /// Removes the tracing object set by <see cref="SetLogger(ILogger)"/>.
    /// </summary>
    public static void ClearLogger() => Logger = null;

    /// <summary>
    /// Gets the tracing object.
    /// </summary>
    public static ILogger? Logger { get; private set; }
    #endregion

    #region Icon
    /// <summary>
    /// Loads an icon resource. If <paramref name="assemblyName"/> is provided, tries to load it from the specified compressed assembly, and falls back to the calling assembly if not found.
    /// This method returns a <see cref="Icon"/> object.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="assemblyName">The assembly name, String.Empty to use the calling assembly directly.</param>
    /// <param name="value">The resource icon upon return.</param>
    /// <returns>True if the icon was found and loaded; otherwise, false.</returns>
    public static bool LoadIcon(string resourceName, string assemblyName, out Icon value)
    {
        Contract.RequireNotNull(resourceName, out string ResourceName);
        Contract.RequireNotNull(assemblyName, out string AssemblyName);

        Assembly CallingAssembly = Assembly.GetCallingAssembly();
        if (LoadInternal(ResourceName, AssemblyName, CallingAssembly, out value))
            return true;

        Contract.Unused(out value);
        return false;
    }

    /// <summary>
    /// Loads an icon resource. If <paramref name="assemblyName"/> is provided, tries to load it from the specified compressed assembly, and falls back to the calling assembly if not found.
    /// This method returns a <see cref="ImageSource"/> object.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="assemblyName">The assembly name, String.Empty to use the calling assembly directly.</param>
    /// <param name="value">The resource icon upon return.</param>
    /// <returns>True if the icon was found and loaded; otherwise, false.</returns>
    public static bool LoadIcon(string resourceName, string assemblyName, out ImageSource value)
    {
        Contract.RequireNotNull(resourceName, out string ResourceName);
        Contract.RequireNotNull(assemblyName, out string AssemblyName);

        Assembly CallingAssembly = Assembly.GetCallingAssembly();
        if (LoadInternalStream(ResourceName, AssemblyName, CallingAssembly) is Stream ResourceStream)
        {
            // Decode the icon from the stream and set the first frame to the BitmapSource.
            BitmapDecoder decoder = BitmapDecoder.Create(ResourceStream, BitmapCreateOptions.None, BitmapCacheOption.None);
            ImageSource Result = decoder.Frames[0];

            if (Logger is not null)
                LoggerMessage.Define(LogLevel.Debug, 0, $"Resource '{resourceName}' loaded")(Logger, null);

            value = Result;
            return true;
        }

        Contract.Unused(out value);
        return false;
    }
    #endregion

    #region Bitmap
    /// <summary>
    /// Loads an bitmap resource. If <paramref name="assemblyName"/> is provided, tries to load it from the specified compressed assembly, and falls back to the calling assembly if not found.
    /// This method returns a <see cref="Bitmap"/> object.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="assemblyName">The assembly name, String.Empty to use the calling assembly directly.</param>
    /// <param name="value">The resource bitmap upon return.</param>
    /// <returns>True if the bitmap was found and loaded; otherwise, false.</returns>
    public static bool LoadBitmap(string resourceName, string assemblyName, out Bitmap value)
    {
        Contract.RequireNotNull(resourceName, out string ResourceName);
        Contract.RequireNotNull(assemblyName, out string AssemblyName);

        Assembly CallingAssembly = Assembly.GetCallingAssembly();
        if (LoadInternal(ResourceName, AssemblyName, CallingAssembly, out value))
            return true;

        Contract.Unused(out value);
        return false;
    }
    #endregion

    #region Generic
    /// <summary>
    /// Loads a resource. If <paramref name="assemblyName"/> is provided, tries to load it from the specified compressed assembly, and falls back to the calling assembly if not found.
    /// </summary>
    /// <typeparam name="TResource">The resource type.</typeparam>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="assemblyName">The assembly name, String.Empty to use the calling assembly directly.</param>
    /// <param name="value">The resource object upon return.</param>
    /// <returns>True if the resource was found and loaded; otherwise, false.</returns>
    public static bool Load<TResource>(string resourceName, string assemblyName, out TResource value)
        where TResource : class
    {
        Contract.RequireNotNull(resourceName, out string ResourceName);
        Contract.RequireNotNull(assemblyName, out string AssemblyName);

        Assembly CallingAssembly = Assembly.GetCallingAssembly();
        if (LoadInternal(ResourceName, AssemblyName, CallingAssembly, out value))
            return true;

        Contract.Unused(out value);
        return false;
    }

    /// <summary>
    /// Loads a resource stream. If <paramref name="assemblyName"/> is provided, tries to load it from the specified compressed assembly, and falls back to the calling assembly if not found.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="assemblyName">The assembly name, String.Empty to use the calling assembly directly.</param>
    /// <param name="resourceStream">The stream object upon return.</param>
    /// <returns>True if the resource was found and stream loaded; otherwise, false.</returns>
    public static bool LoadStream(string resourceName, string assemblyName, out Stream resourceStream)
    {
        Contract.RequireNotNull(resourceName, out string ResourceName);
        Contract.RequireNotNull(assemblyName, out string AssemblyName);

        Assembly CallingAssembly = Assembly.GetCallingAssembly();
        if (LoadInternalStream(ResourceName, AssemblyName, CallingAssembly) is Stream LoadedStream)
        {
            resourceStream = LoadedStream;
            return true;
        }

        Contract.Unused(out resourceStream);
        return false;
    }
    #endregion

    #region Implementation
    private static bool LoadInternal<TResource>(string resourceName, string assemblyName, Assembly callingAssembly, out TResource value)
        where TResource : class
    {
        if (LoadInternalStream(resourceName, assemblyName, callingAssembly) is Stream ResourceStream)
        {
            TResource Result = (TResource)Contract.AssertNotNull(Activator.CreateInstance(typeof(TResource), ResourceStream));

            if (Logger is not null)
                LoggerMessage.Define(LogLevel.Debug, 0, $"Resource '{resourceName}' loaded")(Logger, null);

            value = Result;
            return true;
        }

        Contract.Unused(out value);
        return false;
    }

    private static Stream? LoadInternalStream(string resourceName, string assemblyName, Assembly callingAssembly)
    {
        if (assemblyName.Length > 0)
        {
            if (!DecompressedAssemblyTable.TryGetValue(assemblyName, out _))
            {
                if (LoadEmbeddedAssemblyStream(assemblyName, callingAssembly, out Assembly DecompressedAssembly))
                    DecompressedAssemblyTable.Add(assemblyName, DecompressedAssembly);
            }
        }

        Assembly ResourceAssembly = DecompressedAssemblyTable.TryGetValue(assemblyName, out Assembly? Value) ? Value : callingAssembly;

        if (GetResourcePath(ResourceAssembly, resourceName, out string ResourcePath))
        {
            Stream? ResourceStream = ResourceAssembly.GetManifestResourceStream(ResourcePath);
            _ = Contract.AssertNotNull(ResourceStream);
            return ResourceStream;
        }
        else
        {
            // If not found, it could be because it's not tagged as "Embedded Resource".
            if (Logger is not null)
                LoggerMessage.Define(LogLevel.Error, 0, $"Resource '{resourceName}' not found (is it tagged as \"Embedded Resource\"?)")(Logger, null);

            return null;
        }
    }

    private static bool LoadEmbeddedAssemblyStream(string assemblyName, Assembly callingAssembly, out Assembly decompressedAssembly)
    {
        string EmbeddedAssemblyResourcePath = $"costura.{assemblyName}.dll.compressed";
#pragma warning disable CA1308 // Normalize strings to uppercase
        EmbeddedAssemblyResourcePath = EmbeddedAssemblyResourcePath.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

        using Stream? CompressedStream = callingAssembly.GetManifestResourceStream(EmbeddedAssemblyResourcePath);
        if (CompressedStream is null)
        {
            if (Logger is not null)
                LoggerMessage.Define(LogLevel.Error, 0, $"Assembly {assemblyName} not found (did you forget to use Costura Fody?)")(Logger, null);

            Contract.Unused(out decompressedAssembly);
            return false;
        }

        using Stream UncompressedStream = new DeflateStream(CompressedStream, CompressionMode.Decompress);
        using MemoryStream TemporaryStream = new();

        int Count;
        byte[] Buffer = new byte[81920];
        while ((Count = UncompressedStream.Read(Buffer, 0, Buffer.Length)) != 0)
            TemporaryStream.Write(Buffer, 0, Count);

        TemporaryStream.Position = 0;

        byte[] array = new byte[TemporaryStream.Length];
        _ = TemporaryStream.Read(array, 0, array.Length);

        decompressedAssembly = Assembly.Load(array);
        return true;
    }

    private static bool GetResourcePath(Assembly resourceAssembly, string resourceName, out string resourcePath)
    {
        if (resourceName.Length > 0)
        {
            // Loads an "Embedded Resource" of type TResource (ex: Bitmap for a PNG file).
            // Make sure the resource is tagged as such in the resource properties.
            string[] ResourceNames = resourceAssembly.GetManifestResourceNames();
            foreach (string Item in ResourceNames)
            {
                if (Item.EndsWith(resourceName, StringComparison.InvariantCulture))
                {
                    resourcePath = Item;
                    return true;
                }
            }
        }

        Contract.Unused(out resourcePath);
        return false;
    }

    private static readonly Dictionary<string, Assembly> DecompressedAssemblyTable = [];
    #endregion
}
