using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Enhanzer.Backend.Models
{
    public class ExternalLoginResponse
    {
        public int Status_Code { get; set; }
        public string Message { get; set; }
        public List<LoginResponseBody> Response_Body { get; set; }
    }

    public class LoginResponseBody
    {
        public List<LocationDetail> User_Locations { get; set; }
    }
}
