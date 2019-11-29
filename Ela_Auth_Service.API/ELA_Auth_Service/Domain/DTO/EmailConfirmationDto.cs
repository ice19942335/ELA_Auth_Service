using System.Collections.Generic;

namespace ELA_Auth_Service.Domain.DTO
{
    public class EmailConfirmationDto
    {
        public IEnumerable<string> Errors { get; set; }

        public bool Success { get; set; }

        public bool CriticalError { get; set; }
    }
}
