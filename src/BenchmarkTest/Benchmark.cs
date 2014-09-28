﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTest
{
    public static class Benchmark
    {
        private const string defaultBenchmarkFile = @"..\BenchmarkData.json";     
        private static Dictionary<string, PerformanceData> benchmarks;

        /// <summary>
        /// The full path to the benchmark file. Defaults to 'BenchmarkData.json' in Test bin folder if not provided.
        /// </summary>
        public static string BenchmarkFile { get; set; }

        /// <summary>
        /// Specify an amount of seconds to tolerate if operation takes longer than benchmark.  Default is zero.
        /// </summary>
        public static int ToleranceSeconds { get; set; }

        /// <summary>
        /// Asserts the amount of time a given operation takes against a recorded benchmark.
        /// First run against the operation will create the benchmark and record it for future runs.
        /// </summary>
        /// <param name="operationToBenchmark">Action delegate for the operation to benchmark</param>
        /// <param name="scenarioName">Name of the scenario.  Defaults to the calling test method name. Should be a unique name.</param>
        public static void BenchmarkAssert(Action operationToBenchmark, [CallerMemberName]string scenarioName = "")
        {
            InitializeDefaults();
            GetExistingBenchmarks();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            operationToBenchmark();

            stopwatch.Stop();
            var timeTaken = stopwatch.Elapsed;
            Console.WriteLine("Time taken :" + timeTaken);

            AssertTimeAgainstBenchmark(scenarioName, timeTaken);
        }

        private static void InitializeDefaults()
        {
            if(string.IsNullOrEmpty(BenchmarkFile))
            {
                BenchmarkFile = defaultBenchmarkFile;
            }
        }

        private static void GetExistingBenchmarks()
        {
            if (benchmarks == null)
            {
                if (File.Exists(BenchmarkFile))
                {
                    var benchmarksJson = File.ReadAllText(BenchmarkFile);
                    benchmarks = JsonConvert.DeserializeObject<Dictionary<string, PerformanceData>>(benchmarksJson);
                }
                else
                {
                    Console.WriteLine(string.Format("Benchmark File does not exist. Creating: {0}", BenchmarkFile));
                    Directory.CreateDirectory(Path.GetDirectoryName(BenchmarkFile));
                    benchmarks = new Dictionary<string, PerformanceData>();
                    File.WriteAllText(BenchmarkFile, JsonConvert.SerializeObject(benchmarks));
                }
            }
        }

        private static void AssertTimeAgainstBenchmark(string scenario, TimeSpan timeTaken)
        {
            if (!benchmarks.ContainsKey(scenario))
            {
                Console.WriteLine(string.Format("Scenario '{0}' not previously tested. Adding to benchmarks.", scenario));
                benchmarks.Add(scenario, new PerformanceData { ScenarioName = scenario, TimeSpan = timeTaken });
                var jsonString = JsonConvert.SerializeObject(benchmarks);
                File.WriteAllText(BenchmarkFile, jsonString);
            }
            else
            {
                if(timeTaken >= benchmarks[scenario].TimeSpan.Add(new TimeSpan(0, 0, ToleranceSeconds)))
                {
                    throw new BenchmarkAssertionFailure(string.Format("Scenario '{0}' Failed. \nExpected time: {1}. Actual time: {2}", scenario, existingPerfData.TimeSpan, timeTaken));
                }
            }
        }
    }
}
