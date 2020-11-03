namespace TestResourceTools
{
    using NUnit.Framework;
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
            TestCompanion.TestSet.SetLogger(Logger);

            Success = TestCompanion.TestSet.Load(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.Load("invalid.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.Load("invalid.ico", "Invalid", out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.Load(string.Empty, "Invalid", out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadStream("main.png", string.Empty, out Stream);
            Assert.That(Success);

            Stream.Dispose();

            Success = TestCompanion.TestSet.LoadStream("invalid.png", string.Empty, out Stream);
            Assert.That(!Success);
        }

        [Test]
        public static void TestIcon()
        {
            bool Success;
            Icon Icon;
            ImageSource ImageSource;

            ITracer Logger = Tracer.Create("Test");
            TestCompanion.TestSet.SetLogger(Logger);

            Success = TestCompanion.TestSet.LoadIcon(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadIcon(string.Empty, string.Empty, out ImageSource);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadIcon("main.ico", string.Empty, out Icon);
            Assert.That(Success);

            Success = TestCompanion.TestSet.LoadIcon("compressed.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Test-Compressed", out Icon);
            Assert.That(Success);

            Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Test-Compressed", out ImageSource);
            Assert.That(Success);

            Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Invalid", out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadIcon("invalid.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadIcon(string.Empty, "Invalid", out Icon);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadIcon("invalid.ico", string.Empty, out ImageSource);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadIcon(string.Empty, "Invalid", out ImageSource);
            Assert.That(!Success);
        }

        [Test]
        public static void TestBitmap()
        {
            bool Success;
            Bitmap Bitmap;

            ITracer Logger = Tracer.Create("Test");
            TestCompanion.TestSet.SetLogger(Logger);

            Success = TestCompanion.TestSet.LoadBitmap(string.Empty, string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadBitmap("main.png", string.Empty, out Bitmap);
            Assert.That(Success);

            Success = TestCompanion.TestSet.LoadBitmap("compressed.png", string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadBitmap("compressed.png", "Test-Compressed", out Bitmap);
            Assert.That(Success);

            Success = TestCompanion.TestSet.LoadBitmap("compressed.png", "Invalid", out Bitmap);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadBitmap("invalid.png", string.Empty, out Bitmap);
            Assert.That(!Success);

            Success = TestCompanion.TestSet.LoadBitmap(string.Empty, "Invalid", out Bitmap);
            Assert.That(!Success);
        }
        #endregion
    }
}
