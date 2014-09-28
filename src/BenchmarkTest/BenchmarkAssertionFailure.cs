using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkTest
{
    public class BenchmarkAssertionFailure : Exception
    {
        public BenchmarkAssertionFailure() : base()
        {
        }

        public BenchmarkAssertionFailure(string message) : base(message)
        {
        }
    }
}
