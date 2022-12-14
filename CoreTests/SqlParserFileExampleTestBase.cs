using System.IO;
using Irvin.SqlParser;
using NUnit.Framework;

namespace CoreTests
{
    [TestFixture]
    public abstract class SqlParserFileExampleTestBase
    {
        protected SqlParser _classUnderTest;
        protected static string _fileContents;

        protected abstract string FileName { get; }

        [OneTimeSetUp]
        public void RunFirstOnce()
        {
            _fileContents = File.ReadAllText(FileName);
        }

        [SetUp]
        public void RunBeforeEachTest()
        {
            _classUnderTest = new SqlParser();
            _classUnderTest.BatchSeparator = "GO";
        }

        protected SqlCodeUnit Parse()
        {
            return _classUnderTest.ParseCodeUnit(_fileContents);
        }
    }
}