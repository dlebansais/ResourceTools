namespace ResourceTools
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using Contracts;
    using Tracing;

    /// <summary>
    /// This class provides an interface to load embedded resources of a specific type, including those compressed with Costura Fody.
    /// </summary>
    public static class Loader
    {
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

        /// <summary>
        /// Loads an icon resource. If <paramref name="assemblyName"/> is provided, tries to load it from the specified compressed assembly, and falls back to the calling assembly if not found.
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

        private static bool LoadInternal<TResource>(string resourceName, string assemblyName, Assembly callingAssembly, out TResource value)
            where TResource : class
        {
            if (assemblyName.Length > 0)
                if (!DecompressedAssemblyTable.ContainsKey(assemblyName))
                    if (LoadEmbeddedAssemblyStream(assemblyName, out Assembly DecompressedAssembly))
                        DecompressedAssemblyTable.Add(assemblyName, DecompressedAssembly);

            Assembly UsingAssembly = DecompressedAssemblyTable.ContainsKey(assemblyName) ? DecompressedAssemblyTable[assemblyName] : callingAssembly;
            string ResourcePath = string.Empty;

            if (resourceName.Length > 0)
            {
                // Loads an "Embedded Resource" of type TResource (ex: Bitmap for a PNG file).
                // Make sure the resource is tagged as such in the resource properties.
                string[] ResourceNames = UsingAssembly.GetManifestResourceNames();
                foreach (string Item in ResourceNames)
                    if (Item.EndsWith(resourceName, StringComparison.InvariantCulture))
                    {
                        ResourcePath = Item;
                        break;
                    }
            }

            // If not found, it could be because it's not tagged as "Embedded Resource".
            if (ResourcePath.Length == 0)
            {
                Logger?.Write(Category.Error, $"Resource '{resourceName}' not found (is it tagged as \"Embedded Resource\"?)");

                Contract.Unused(out value);
                return false;
            }

            using Stream ResourceStream = UsingAssembly.GetManifestResourceStream(ResourcePath);

            TResource Result = (TResource)Activator.CreateInstance(typeof(TResource), ResourceStream);
            Logger?.Write(Category.Debug, $"Resource '{resourceName}' loaded");

            value = Result;
            return true;
        }

        private static bool LoadEmbeddedAssemblyStream(string assemblyName, out Assembly decompressedAssembly)
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            string EmbeddedAssemblyResourcePath = $"costura.{assemblyName}.dll.compressed";
#pragma warning disable CA1308 // Normalize strings to uppercase
            EmbeddedAssemblyResourcePath = EmbeddedAssemblyResourcePath.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

            using Stream CompressedStream = assembly.GetManifestResourceStream(EmbeddedAssemblyResourcePath);
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

        private static Dictionary<string, Assembly> DecompressedAssemblyTable = new Dictionary<string, Assembly>();
    }
}
