using System;

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
