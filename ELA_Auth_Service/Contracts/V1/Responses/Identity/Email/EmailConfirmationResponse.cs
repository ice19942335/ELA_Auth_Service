using System.Collections.Generic;

namespace ELA_Auth_Service.Contracts.V1.Responses.Identity.Email
{
    public class EmailConfirmationResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public bool CriticalError { get; set; }
    }
}
