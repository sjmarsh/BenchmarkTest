using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTest.Reports
{
    public class ReportBuilder
    {
        public static void Build(string reportPath, Dictionary<string, PerformanceData> reportData)
        {
            var reportTemplate = GetReportTemplate();
            var report = GenerateReport(reportTemplate, reportData);
            SaveReport(report, reportPath);
        }

        private static string GetReportTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "BenchmarkTest.Reports.ReportTemplate.html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return  reader.ReadToEnd();
            }
        }

        private static string GenerateReport(string reportTemplate, Dictionary<string, PerformanceData> reportData)
        {
            const string jsonDataToken = "#data-token#";
            const int lastRunSeconds = 5; // todo capture actual last run

            /*
           * required format
           * [
        ['Scenario', 'Benchmark', 'Time Taken'],
        ['1',  10,      11],
        ['2',  12,      11],
        ['3',  15,       14],
        ['4',  13,      12]
      ]
           */

            var jsonString = new StringBuilder();
            jsonString.Append("[ ['Scenario', 'Benchmark', 'Time Taken'],");

            var reportDataCount = reportData.Values.Count;
            for (var i = 0; i < reportDataCount; i++)
            {
                var performanceData = reportData.ElementAt(i).Value;
                if (i == (reportDataCount - 1))
                {
                    jsonString.Append(string.Format("['{0}', {1}, {2}]", performanceData.ScenarioName, performanceData.TimeSpan.Seconds, lastRunSeconds));
                }
                else
                {
                    jsonString.Append(string.Format("['{0}', {1}, {2}],", performanceData.ScenarioName, performanceData.TimeSpan.Seconds, lastRunSeconds));
                }
            }
            
            jsonString.Append("]");

            reportTemplate = reportTemplate.Replace(jsonDataToken, jsonString.ToString());

            return reportTemplate;
        }

        private static void SaveReport(string report, string reportPath)
        {
            File.WriteAllText(reportPath, report);
        }
    }
}
