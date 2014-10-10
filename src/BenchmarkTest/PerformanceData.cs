using System;

namespace BenchmarkTest
{
    public class PerformanceData
    {
        public string ScenarioName { get; set; }
        public TimeSpan BenchmarkedTime { get; set; }
        public TimeSpan LastRunTime { get; set; }
    }
}
