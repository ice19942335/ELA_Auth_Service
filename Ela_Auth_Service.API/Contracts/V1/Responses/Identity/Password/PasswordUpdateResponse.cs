﻿using System.Collections.Generic;

namespace ELA_Auth_Service.Contracts.V1.Responses.Identity.Password
{
    public class PasswordUpdateResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public bool CriticalError { get; set; }
    }
}
