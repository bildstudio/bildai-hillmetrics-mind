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
    }
}
