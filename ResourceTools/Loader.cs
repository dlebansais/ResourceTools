namespace ResourceTools
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Contracts;
    using Tracing;

    /// <summary>
    /// This class provides an interface to load embedded resources of a specific type, including those compressed with Costura Fody.
    /// </summary>
    public static class Loader
    {
        #region Logger
        /// <summary>
        /// Sets a tracing object for diagnostic purpose.
        /// </summary>
        /// <param name="logger">The tracing object.</param>
        public static void SetLogger(ITracer logger)
        {
            Contract.RequireNotNull(logger, out ITracer Logger);

            Loader.Logger = Logger;
        }

        /// <summary>
        /// Gets the tracing object.
        /// </summary>
        public static ITracer? Logger { get; private set; }
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
            if (!LoadInternalStream(ResourceName, AssemblyName, CallingAssembly, out Stream ResourceStream))
            {
                ResourceStream.Dispose();

                Contract.Unused(out value);
                return false;
            }
            else
            {
                // Decode the icon from the stream and set the first frame to the BitmapSource
                BitmapDecoder decoder = IconBitmapDecoder.Create(ResourceStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                ImageSource Result = decoder.Frames[0];

                Logger?.Write(Category.Debug, $"Resource '{resourceName}' loaded");

                value = Result;
                return true;
            }
        }
        #endregion

        #region Icon
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
            if (LoadInternalStream(ResourceName, AssemblyName, CallingAssembly, out resourceStream))
                return true;

            Contract.Unused(out resourceStream);
            return false;
        }
        #endregion

        #region Implementation
        private static bool LoadInternal<TResource>(string resourceName, string assemblyName, Assembly callingAssembly, out TResource value)
            where TResource : class
        {
            if (!LoadInternalStream(resourceName, assemblyName, callingAssembly, out Stream ResourceStream))
            {
                ResourceStream.Dispose();

                Contract.Unused(out value);
                return false;
            }
            else
            {
                TResource Result = (TResource)Activator.CreateInstance(typeof(TResource), ResourceStream);
                Logger?.Write(Category.Debug, $"Resource '{resourceName}' loaded");

                value = Result;
                return true;
            }
        }

        private static bool LoadInternalStream(string resourceName, string assemblyName, Assembly callingAssembly, out Stream resourceStream)
        {
            if (assemblyName.Length > 0)
                if (!DecompressedAssemblyTable.ContainsKey(assemblyName))
                    if (LoadEmbeddedAssemblyStream(assemblyName, callingAssembly, out Assembly DecompressedAssembly))
                        DecompressedAssemblyTable.Add(assemblyName, DecompressedAssembly);

            Assembly ResourceAssembly = DecompressedAssemblyTable.ContainsKey(assemblyName) ? DecompressedAssemblyTable[assemblyName] : callingAssembly;

            if (!GetResourcePath(ResourceAssembly, resourceName, out string ResourcePath))
            {
                // If not found, it could be because it's not tagged as "Embedded Resource".
                Logger?.Write(Category.Error, $"Resource '{resourceName}' not found (is it tagged as \"Embedded Resource\"?)");

                Contract.Unused(out resourceStream);
                return false;
            }
            else
            {
                resourceStream = ResourceAssembly.GetManifestResourceStream(ResourcePath);
                return true;
            }
        }

        private static bool LoadEmbeddedAssemblyStream(string assemblyName, Assembly callingAssembly, out Assembly decompressedAssembly)
        {
            string EmbeddedAssemblyResourcePath = $"costura.{assemblyName}.dll.compressed";
#pragma warning disable CA1308 // Normalize strings to uppercase
            EmbeddedAssemblyResourcePath = EmbeddedAssemblyResourcePath.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

            using Stream CompressedStream = callingAssembly.GetManifestResourceStream(EmbeddedAssemblyResourcePath);
            if (CompressedStream == null)
            {
                Logger?.Write(Category.Error, $"Assembly {assemblyName} not found (did you forget to use Costura Fody?)");

                Contract.Unused(out decompressedAssembly);
                return false;
            }

            using Stream UncompressedStream = new DeflateStream(CompressedStream, CompressionMode.Decompress);
            using MemoryStream TemporaryStream = new MemoryStream();

            int Count;
            var Buffer = new byte[81920];
            while ((Count = UncompressedStream.Read(Buffer, 0, Buffer.Length)) != 0)
                TemporaryStream.Write(Buffer, 0, Count);

            TemporaryStream.Position = 0;

            byte[] array = new byte[TemporaryStream.Length];
            TemporaryStream.Read(array, 0, array.Length);

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
                    if (Item.EndsWith(resourceName, StringComparison.InvariantCulture))
                    {
                        resourcePath = Item;
                        return true;
                    }
            }

            Contract.Unused(out resourcePath);
            return false;
        }

        private static Dictionary<string, Assembly> DecompressedAssemblyTable = new Dictionary<string, Assembly>();
        #endregion
    }
}
