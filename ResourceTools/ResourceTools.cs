namespace ResourceTools
{
    using System.Drawing;

    public static class ResourceTools
    {
        public static bool LoadIcon(out Icon value)
        {
            return EmbeddedResourceLoader<Icon>.Load(out value);
        }
    }
}
