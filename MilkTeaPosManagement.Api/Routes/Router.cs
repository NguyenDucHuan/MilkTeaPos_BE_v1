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

        public static class UserRoute
        {
            public const string User = $"{prefix}user";
            public const string Profile = $"{User}/profile";
            public const string UpdateAvatar = $"{User}/update-avatar";
            public const string ChangePassword = $"{User}/change-password";
            public const string GetAllUsers = $"{User}/all-users";
            public const string GetUserById = $"{User}/{Id}";
            public const string CreateUser = $"{User}/create-user";
            public const string UpdateUser = $"{User}/update-user/{Id}";
            public const string UpdateUserStatus = $"{User}/update-user-status/{Id}";
        }
        public static class CategoryRoute
        {
            public const string GetAll = $"{prefix}categories";
            public const string GetById = $"{prefix}categories/{Id}";
            public const string Create = $"{GetAll}/create-category";
            public const string Update = $"{GetAll}/update-category/{Id}";
            public const string Delete = $"{GetAll}/update-status/{Id}";
        }
        public static class ProductRoute
        {
            public const string GetAll = $"{prefix}products";
            public const string GetById = $"{prefix}products/{Id}";
            public const string Create_Mater_Producr = $"{GetAll}/create-product";
            public const string Create_Extra_Product = $"{GetAll}/create-extra-product";
            public const string Create_Combo = $"{GetAll}/create-combo-product";
            public const string Update = $"{GetAll}/update-product/{Id}";
            public const string Delete = $"{GetAll}/update-status/{Id}";
        }
    }
}
