﻿using System.Collections.Generic;

namespace ELA_Auth_Service.Contracts.V1.Responses.Authentication.Password
{
    public class PasswordResetRequestFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public bool CriticalError { get; set; }
    }
}
