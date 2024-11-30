namespace ResourceTools.Test;

using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

[TestFixture]
internal static class TestSet
{
    [OneTimeSetUp]
    public static void InitTestSession()
    {
        CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
        CultureInfo.DefaultThreadCurrentCulture = enUS;
        CultureInfo.DefaultThreadCurrentUICulture = enUS;
        Thread.CurrentThread.CurrentCulture = enUS;
        Thread.CurrentThread.CurrentUICulture = enUS;

        Assembly? ResourceToolsAssembly = Assembly.Load("ResourceTools");

        Assume.That(ResourceToolsAssembly is not null);
    }

    #region No Logger Tests
    [Test]
    public static void TestBasic()
    {
        bool Success;

        bool Constant = Dependency.GetConstant(true);
        Assert.That(Constant, Is.True);

        TestCompanion.TestSet.ClearLogger();

        Success = TestCompanion.TestSet.Load(string.Empty, string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.Load("invalid.ico", string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.Load("invalid.ico", "Invalid", out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.Load(string.Empty, "Invalid", out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadStream("main.png", string.Empty, out Stream Stream1);
        Assert.That(Success);

        Stream1.Dispose();

        Success = TestCompanion.TestSet.Load("compressed.png", "Test-Compressed", out Bitmap Bitmap1);
        Assert.That(Success);

        Bitmap1.Dispose();

        Success = TestCompanion.TestSet.LoadStream("invalid.png", string.Empty, out _);
        Assert.That(!Success);

        Assert.That(TestCompanion.TestSet.Logger is null);
    }

    [Test]
    public static void TestIcon()
    {
        bool Success;

        TestCompanion.TestSet.ClearLogger();

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, string.Empty, out ImageSource _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("main.ico", string.Empty, out Icon _);
        Assert.That(Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Test-Compressed", out Icon _);
        Assert.That(Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Test-Compressed", out ImageSource _);
        Assert.That(Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Invalid", out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("invalid.ico", string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, "Invalid", out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("invalid.ico", string.Empty, out ImageSource _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, "Invalid", out ImageSource _);
        Assert.That(!Success);

        Assert.That(TestCompanion.TestSet.Logger is null);
    }

    [Test]
    public static void TestBitmap()
    {
        bool Success;

        TestCompanion.TestSet.ClearLogger();

        Success = TestCompanion.TestSet.LoadBitmap(string.Empty, string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap("main.png", string.Empty, out Bitmap Bitmap1);
        Assert.That(Success);

        Bitmap1.Dispose();

        Success = TestCompanion.TestSet.LoadBitmap("compressed.png", string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap("compressed.png", "Test-Compressed", out Bitmap Bitmap2);
        Assert.That(Success);

        Bitmap2.Dispose();

        Success = TestCompanion.TestSet.LoadBitmap("compressed.png", "Invalid", out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap("invalid.png", string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap(string.Empty, "Invalid", out _);
        Assert.That(!Success);

        Assert.That(TestCompanion.TestSet.Logger is null);
    }
    #endregion

    #region With Logger Tests
    [Test]
    public static void TestBasicWithLogger()
    {
        bool Success;

        bool Constant = Dependency.GetConstant(true);
        Assert.That(Constant, Is.True);

        using LoggerFactory LoggerFactory = new();
        ILogger Logger = LoggerFactory.CreateLogger("Test");
        TestCompanion.TestSet.SetLogger(Logger);

        Success = TestCompanion.TestSet.Load<Icon>(string.Empty, string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.Load<Icon>("invalid.ico", string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.Load<Icon>("invalid.ico", "Invalid", out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.Load<Icon>(string.Empty, "Invalid", out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadStream("main.png", string.Empty, out Stream Stream1);
        Assert.That(Success);

        Stream1.Dispose();

        Success = TestCompanion.TestSet.Load("compressed.png", "Test-Compressed", out Bitmap Bitmap1);
        Assert.That(Success);

        Bitmap1.Dispose();

        Success = TestCompanion.TestSet.LoadStream("invalid.png", string.Empty, out _);
        Assert.That(!Success);

        Assert.That(TestCompanion.TestSet.Logger == Logger);
    }

    [Test]
    public static void TestIconWithLogger()
    {
        bool Success;

        using LoggerFactory LoggerFactory = new();
        ILogger Logger = LoggerFactory.CreateLogger("Test");
        TestCompanion.TestSet.SetLogger(Logger);

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, string.Empty, out ImageSource _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("main.ico", string.Empty, out Icon _);
        Assert.That(Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Test-Compressed", out Icon _);
        Assert.That(Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Test-Compressed", out ImageSource _);
        Assert.That(Success);

        Success = TestCompanion.TestSet.LoadIcon("compressed.ico", "Invalid", out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("invalid.ico", string.Empty, out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, "Invalid", out Icon _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon("invalid.ico", string.Empty, out ImageSource _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadIcon(string.Empty, "Invalid", out ImageSource _);
        Assert.That(!Success);

        Assert.That(TestCompanion.TestSet.Logger == Logger);
    }

    [Test]
    public static void TestBitmapWithLogger()
    {
        bool Success;

        using LoggerFactory LoggerFactory = new();
        ILogger Logger = LoggerFactory.CreateLogger("Test");
        TestCompanion.TestSet.SetLogger(Logger);

        Success = TestCompanion.TestSet.LoadBitmap(string.Empty, string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap("main.png", string.Empty, out _);
        Assert.That(Success);

        Success = TestCompanion.TestSet.LoadBitmap("compressed.png", string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap("compressed.png", "Test-Compressed", out Bitmap Bitmap1);
        Assert.That(Success);

        Bitmap1.Dispose();

        Success = TestCompanion.TestSet.LoadBitmap("compressed.png", "Invalid", out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap("invalid.png", string.Empty, out _);
        Assert.That(!Success);

        Success = TestCompanion.TestSet.LoadBitmap(string.Empty, "Invalid", out _);
        Assert.That(!Success);

        Assert.That(TestCompanion.TestSet.Logger == Logger);
    }
    #endregion
}
