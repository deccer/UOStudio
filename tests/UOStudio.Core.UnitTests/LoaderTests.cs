using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Serilog;
using Xunit;

namespace UOStudio.Core.UnitTests
{
    public class LoaderTests
    {
        [Fact]
        public void Loading_Configuration_From_NotExistingFile_Will_Return_Null()
        {
            var logger = Substitute.For<ILogger>();
            var loader = new Loader(logger);

            loader
                .Load<TestConfig.Test>("FileDoesNotExist")
                .Should().BeNull();
        }

        [Fact]
        public void Loading_Configuration_From_ExistingFile_Will_Not_Return_Null()
        {
            var logger = Substitute.For<ILogger>();
            var configurationLoader = new Loader(logger);

            using var testConfig = new TestConfig();
            configurationLoader
                .Load<TestConfig.Test>(testConfig.TempFileName)
                .Should().NotBeNull();
        }

        [Fact]
        public void Loading_Configuration_From_ExistingFile_Will_Contain_Valid_Values()
        {
            var logger = Substitute.For<ILogger>();
            var configurationLoader = new Loader(logger);

            using var testConfig = new TestConfig();
            configurationLoader
                .Load<TestConfig.Test>(testConfig.TempFileName)
                .Port.Should()
                .Be(1000);
        }

        private class TestConfig : IDisposable
        {
            public class Test
            {
                public int Port { get; set; }
            }

            public string TempFileName { get; }

            public TestConfig()
            {
                TempFileName = Path.GetTempFileName();
                var json = JsonConvert.SerializeObject(new Test { Port = 1000 });
                File.WriteAllText(TempFileName, json);
            }

            public void Dispose()
            {
                File.Delete(TempFileName);
            }
        }
    }
}
