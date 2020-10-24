namespace ResourceTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EmbeddedResourceLoader<TResource>
        where TResource : class
    {
        public static bool Load(out TResource value)
        {
            value = null!;
            return false;
        }
    }
}
