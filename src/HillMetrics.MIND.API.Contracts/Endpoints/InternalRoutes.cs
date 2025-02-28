namespace HillMetrics.MIND.API.Endpoints
{
    public static class InternalRoutes
    {
        public const string Version = "1";
        public const string Base = $"api/v{Version}";

        public static class Test
        {
            public const string Get = "test/get";
            public const string Error = "test/error";
        }

        public static class Authentication
        {
            public const string Login = "auth/login";
            public const string Callback = "auth/callback";
            public const string Logout = "auth/logout";
            public const string LogoutCallback = "auth/logoutcallback";
            public const string Refresh = "auth/refresh";
        }

        public static class Flux
        {
            public const string Get = "flux";
        }

        public static class Sector
        {
            public const string Get = "";
            public const string GetById = "{id}";
            public const string Create = "";
        }

        public static class Llm
        {
            public const string Get = "llm/models";

            public static class Prompts
            {
                public const string Get = "llm/prompts/{promptId}";
                public const string Create = "llm/prompts";
                public const string Update = "llm/prompts/{promptId}";
                public const string Delete = "llm/prompts/{promptId}";
                public const string Search = "llm/prompts/search";
            }
        }
    }
}
