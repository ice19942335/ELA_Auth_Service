using System;

namespace ELA_Auth_Service.Options
{
    public class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifetime { get; set; }
    }
}
