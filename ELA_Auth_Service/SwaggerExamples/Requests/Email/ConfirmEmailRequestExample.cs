using ELA_Auth_Service.Contracts.V1.Requests.Authentication.Email;
using Swashbuckle.AspNetCore.Filters;

namespace ELA_Auth_Service.SwaggerExamples.Requests.Email
{
    public class ConfirmEmailRequestExample : IExamplesProvider<ConfirmEmailRequest>
    {
        public ConfirmEmailRequest GetExamples()
        {
            return new ConfirmEmailRequest
            {
                UserId = "0bf8b111-408f-4abd-b249-2ad3e1a50102",
                Code = "CfDJ8LLATCsIIItHq%2BcvMuCVLPG%2FuDTReH9N35crsu9t%2FqfG6XMqtAzh5r%2BC7IkBlgdPvS9QziM5Huo%2BKlBAHoi4pFtVIVz46kOpdLfiPw11S%2BrHo8GDSiJU7GJNv1dZ77FtZSABTtUUyOxCXHizdCqZZFwxOAu0EBC1kP5C6PnW0rWE%2Bseq8Pnpx5Tpw0kGaHFHgeSaUG9749%2FbbR%2FgsAmxibnAH004lM8ECIM1qADifaYD"
            };
        }
    }
}
