using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Endpoints
{
    public static class InternalRoutes
    {
        public const string Version = "1";
        public const string Prefix = "api/v{v:apiVersion}";

        public static class Test
        {
            public const string Get = "test/get";
            public const string Error = "test/error";
        }
    }
}
