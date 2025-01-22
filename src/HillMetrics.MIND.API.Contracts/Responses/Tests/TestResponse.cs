using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Tests
{
    public class TestResponse(TestValue data) : ApiResponseBase<TestValue>(data)
    {
    }

    public class TestValue
    {
        public string TestString { get; set; } = "dfds";
        public DateTime DateTime { get; set; }
    }
}
