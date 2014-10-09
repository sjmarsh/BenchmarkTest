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
            // TODO: tokenize report template and incorporate data

            return reportTemplate;
        }

        private static void SaveReport(string report, string reportPath)
        {
            File.WriteAllText(reportPath, report);
        }
    }
}
