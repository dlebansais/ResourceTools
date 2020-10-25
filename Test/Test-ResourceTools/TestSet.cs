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
            //System.Diagnostics.Debug.Assert(false);

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
            ResourceLoader.SetLogger(Logger);

            Success = ResourceLoader.Load(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.Load("invalid.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.Load("invalid.ico", "Invalid", out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.Load(string.Empty, "Invalid", out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.LoadStream("main.png", string.Empty, out Stream);
            Assert.That(Success);

            Stream.Dispose();

            Success = ResourceLoader.LoadStream("invalid.png", string.Empty, out Stream);
            Assert.That(!Success);
        }

        [Test]
        public static void TestIcon()
        {
            bool Success;
            Icon Icon;
            ImageSource ImageSource;

            Success = ResourceLoader.LoadIcon(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.LoadIcon(string.Empty, string.Empty, out ImageSource);
            Assert.That(!Success);

            Success = ResourceLoader.LoadIcon("main.ico", string.Empty, out Icon);
            Assert.That(Success);

            Success = ResourceLoader.LoadIcon("compressed.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.LoadIcon("compressed.ico", "Test-Compressed", out Icon);
            Assert.That(Success);

            Success = ResourceLoader.LoadIcon("compressed.ico", "Test-Compressed", out ImageSource);
            Assert.That(Success);

            Success = ResourceLoader.LoadIcon("compressed.ico", "Invalid", out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.LoadIcon("invalid.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.LoadIcon(string.Empty, "Invalid", out Icon);
            Assert.That(!Success);

            Success = ResourceLoader.LoadIcon("invalid.ico", string.Empty, out ImageSource);
            Assert.That(!Success);

            Success = ResourceLoader.LoadIcon(string.Empty, "Invalid", out ImageSource);
            Assert.That(!Success);
        }

        [Test]
        public static void TestBitmap()
        {
            bool Success;
            Bitmap Bitmap;

            Success = ResourceLoader.LoadBitmap(string.Empty, string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = ResourceLoader.LoadBitmap("main.png", string.Empty, out Bitmap);
            Assert.That(Success);

            Success = ResourceLoader.LoadBitmap("compressed.png", string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = ResourceLoader.LoadBitmap("compressed.png", "Test-Compressed", out Bitmap);
            Assert.That(Success);

            Success = ResourceLoader.LoadBitmap("compressed.png", "Invalid", out Bitmap);
            Assert.That(!Success);

            Success = ResourceLoader.LoadBitmap("invalid.png", string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = ResourceLoader.LoadBitmap(string.Empty, "Invalid", out Bitmap);
            Assert.That(!Success);
        }
        #endregion
    }
}
