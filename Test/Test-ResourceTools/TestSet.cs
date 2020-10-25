namespace TestResourceTools
{
    using NUnit.Framework;
    using ResourceTools;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Media;
    using Tracing;

    [TestFixture]
    public class TestSet
    {
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            Assembly? ResourceToolsAssembly;

            try
            {
                ResourceToolsAssembly = Assembly.Load("ResourceTools");
            }
            catch
            {
                ResourceToolsAssembly = null;
            }
            Assume.That(ResourceToolsAssembly != null);
        }

        #region Basic Tests
        [Test]
        public static void TestBasic()
        {
            bool Success;
            Icon Icon;
            Stream Stream;

            bool Constant = Dependency.GetConstant();
            Assert.That(Constant == true);

            ITracer Logger = Tracer.Create("Test");
            Loader.SetLogger(Logger);

            Success = Loader.Load(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            Success = Loader.Load("invalid.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = Loader.Load("invalid.ico", "Invalid", out Icon);
            Assert.That(!Success);

            Success = Loader.Load(string.Empty, "Invalid", out Icon);
            Assert.That(!Success);

            Success = Loader.LoadStream("main.png", string.Empty, out Stream);
            Assert.That(Success);

            Stream.Dispose();

            Success = Loader.LoadStream("invalid.png", string.Empty, out Stream);
            Assert.That(!Success);
        }

        [Test]
        public static void TestIcon()
        {
            bool Success;
            Icon Icon;
            ImageSource ImageSource;

            Success = Loader.LoadIcon(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            Success = Loader.LoadIcon(string.Empty, string.Empty, out ImageSource);
            Assert.That(!Success);

            Success = Loader.LoadIcon("main.ico", string.Empty, out Icon);
            Assert.That(Success);

            Success = Loader.LoadIcon("compressed.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = Loader.LoadIcon("compressed.ico", "Test-Compressed", out Icon);
            Assert.That(Success);

            Success = Loader.LoadIcon("compressed.ico", "Test-Compressed", out ImageSource);
            Assert.That(Success);

            Success = Loader.LoadIcon("compressed.ico", "Invalid", out Icon);
            Assert.That(!Success);

            Success = Loader.LoadIcon("invalid.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = Loader.LoadIcon(string.Empty, "Invalid", out Icon);
            Assert.That(!Success);

            Success = Loader.LoadIcon("invalid.ico", string.Empty, out ImageSource);
            Assert.That(!Success);

            Success = Loader.LoadIcon(string.Empty, "Invalid", out ImageSource);
            Assert.That(!Success);
        }

        [Test]
        public static void TestBitmap()
        {
            bool Success;
            Bitmap Bitmap;

            Success = Loader.LoadBitmap(string.Empty, string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = Loader.LoadBitmap("main.png", string.Empty, out Bitmap);
            Assert.That(Success);

            Success = Loader.LoadBitmap("compressed.png", string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = Loader.LoadBitmap("compressed.png", "Test-Compressed", out Bitmap);
            Assert.That(Success);

            Success = Loader.LoadBitmap("compressed.png", "Invalid", out Bitmap);
            Assert.That(!Success);

            Success = Loader.LoadBitmap("invalid.png", string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = Loader.LoadBitmap(string.Empty, "Invalid", out Bitmap);
            Assert.That(!Success);
        }
        #endregion
    }
}
