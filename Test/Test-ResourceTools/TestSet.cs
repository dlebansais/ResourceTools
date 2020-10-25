namespace TestResourceTools
{
    using ResourceTools;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.IO;
    using System.Drawing;
    using Tracing;
    using System.Diagnostics;
    using System.Windows;

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

            bool Constant = Dependency.Constant;
            Assert.That(Constant == true);

            Success = Loader.Load(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            Success = Loader.LoadIcon(string.Empty, string.Empty, out Icon);
            Assert.That(!Success);

            ITracer Logger = Tracer.Create("Test");
            Loader.SetLogger(Logger);

            Success = Loader.Load("main.ico", string.Empty, out Icon);
            Assert.That(Success);

            Success = Loader.LoadIcon("main.ico", string.Empty, out Icon);
            Assert.That(Success);

            Success = Loader.LoadIcon("compressed.ico", string.Empty, out Icon);
            Assert.That(!Success);

            Success = Loader.LoadIcon("compressed.ico", "Test-Compressed", out Icon);
            Assert.That(Success);
        }
        #endregion
    }
}
