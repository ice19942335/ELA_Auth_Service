namespace ELA_Auth_Service.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Authentication
        {
            public const string Login = Base + "/auth/login";

            public const string Register = Base + "/auth/register";

            public const string Refresh = Base + "/auth/refresh";
        }

        public static class UserManager
        {
            public const string PasswordResetRequest = Base + "/usermanager/PasswordResetRequest";

            public const string UpdatePassword = Base + "/usermanager/UpdatePassword";

            public const string EmailConfirmationRequest = Base + "/usermanager/EmailConfirmationRequest";

            public const string ConfirmEmail = Base + "/usermanager/ConfirmEmail";
        }
    }
}
