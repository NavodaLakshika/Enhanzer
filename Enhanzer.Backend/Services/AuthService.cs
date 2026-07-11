using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Enhanzer.Backend.Models;
using System;

namespace Enhanzer.Backend.Services
{
    public interface IAuthService
    {
        Task<ExternalLoginResponse?> AuthenticateExternallyAsync(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ExternalLoginResponse?> AuthenticateExternallyAsync(string username, string password)
        {
            var requestBody = new
            {
                API_Action = "GetLoginData",
                Device_Id = "D001",
                Sync_Time = "",
                Company_Code = username, // Using username as company code per instructions
                API_Body = new
                {
                    Username = username,
                    Pw = password
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[EXTERNAL API RESPONSE] {responseString}");
                    var loginResponse = JsonSerializer.Deserialize<ExternalLoginResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return loginResponse;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[EXTERNAL API ERROR {response.StatusCode}] {errorResponse}");
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
    }
}
