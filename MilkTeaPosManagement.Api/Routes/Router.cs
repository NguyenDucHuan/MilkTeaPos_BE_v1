namespace MilkTeaPosManagement.Api.Routes
{
    public static class Router
    {
        public const string Id = "id";
        public const string prefix = "api/";
        public static class AtuthenticationRoute
        {
            public const string Authentication = $"{prefix}authen";
            public const string Login = $"{Authentication}/login";
        }
    }
}
