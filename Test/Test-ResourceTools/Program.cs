namespace TestResourceTools
{
    using System.Diagnostics;
    using System.Drawing;
    using ResourceTools;

    public class Program
    {
        public static void Main(string[] arguments)
        {
            bool IsLoaded;
            Icon Icon;

            IsLoaded = Loader.Load(string.Empty, string.Empty, out Icon);
            Debug.Assert(!IsLoaded);

            IsLoaded = Loader.LoadIcon(string.Empty, string.Empty, out Icon);
            Debug.Assert(!IsLoaded);
        }
    }
}
