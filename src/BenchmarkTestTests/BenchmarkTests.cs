using NUnit.Framework;
using BenchmarkTest;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace BenchmarkTestTests
{
    [TestFixture]
    public class BenchmarkTests
    {
        public const string BenchmarkFile =  @"c:\temp\benchmarks.json";

        [SetUp]
        public void TestSetUp()
        {
            // clear the static things
            Benchmark.BenchmarkFile = null;
            Benchmark.ToleranceSeconds = 0;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Benchmark.GenerateReport(string.Format(@"c:\temp\report_{0}.html", DateTime.Now.ToString("dd_MM_yy_HH_mm_ss")));
            CleanUpBenchmarkFile(BenchmarkFile);
        }
               
        [Test]
        public void ShouldCreateDefaultBenchmarkFileIfNotSpecified()
        {   
            const string defaultBenchmarkFile = @"..\BenchmarkData.json";

            Benchmark.BenchmarkAssert(() => Thread.Sleep(1000));

            Assert.That(File.Exists(defaultBenchmarkFile), Is.True);

            CleanUpBenchmarkFile(defaultBenchmarkFile);
        }

        [Test]
        public void ShouldCreateSpecifiedBenchmarkFileIfDoesNotExist()
        {
            Benchmark.BenchmarkFile = BenchmarkFile;

            Benchmark.BenchmarkAssert(() => Thread.Sleep(1000));

            Assert.That(File.Exists(BenchmarkFile), Is.True);
        }

        [Test]
        public void ShouldRecordBenchmarkForScenario()
        {
            const string scenarioName = "RecordScenario";
            Benchmark.BenchmarkFile = BenchmarkFile;

            Benchmark.BenchmarkAssert(() => Thread.Sleep(1000), scenarioName);

            var benchmarksJson = File.ReadAllText(BenchmarkFile);
            var benchmarks = JsonConvert.DeserializeObject<Dictionary<string, PerformanceData>>(benchmarksJson);
            var benchmark = benchmarks[scenarioName];
            Assert.That(benchmark, Is.Not.Null);
            Assert.That(benchmark.TimeSpan.Seconds, Is.EqualTo(1));
        }

        [Test]
        public void ShouldFailWhenTimeTakenLongerThenBenchmark()
        {
            Benchmark.BenchmarkFile = BenchmarkFile;

            // record initial benchmark
            Benchmark.BenchmarkAssert(() => Thread.Sleep(1000));

            // run again with longer time
            Assert.Throws<BenchmarkAssertionFailure>(() => Benchmark.BenchmarkAssert(() => Thread.Sleep(2000)));            
        }

        [Test]
        public void ShouldNotFailWhenTimeTakenLongerThenBenchmarkButWithinSpecifiedTolerance()
        {
            Benchmark.BenchmarkFile = BenchmarkFile;
            Benchmark.ToleranceSeconds = 2;

            // record initial benchmark
            Benchmark.BenchmarkAssert(() => Thread.Sleep(1000));

            // run again with longer time
            Benchmark.BenchmarkAssert(() => Thread.Sleep(2000));
        }

        private void CleanUpBenchmarkFile(string benchmarkFile)
        {
            if(File.Exists(benchmarkFile))
            {
                File.Delete(benchmarkFile);
            }
        }
    }
}
