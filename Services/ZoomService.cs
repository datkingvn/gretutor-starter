using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using GreTutor.Models.Entities;
using System.Net.Http.Headers;

namespace GreTutor.Services
{
    public class ZoomService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accountId;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public ZoomService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _accountId = configuration["Zoom:AccountId"];
            _clientId = configuration["Zoom:ClientId"];
            _clientSecret = configuration["Zoom:ClientSecret"];
        }


        /// <summary>
        /// Lấy Access Token từ Zoom Server-to-Server OAuth
        /// </summary>
        private async Task<string> GetAccessToken()
        {
            try
            {
                string tokenUrl = "https://zoom.us/oauth/token";
                var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

                // Encode Client ID và Client Secret theo chuẩn Basic Auth
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));

                // Sử dụng "account_credentials" thay vì "client_credentials"
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "account_credentials" },
            { "account_id", _accountId }
        });

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[DEBUG] Zoom Token Response: {responseString}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"[ERROR] Không lấy được Zoom Access Token: {response.StatusCode} - {responseString}");
                }

                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
                return jsonResponse?.access_token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi lấy Access Token: {ex.Message}");
                throw;
            }
        }


        public async Task<string> CreateMeeting(Meeting meeting)
        {
            try
            {
                if (meeting == null || meeting.StartTime == default)
                {
                    throw new ArgumentException("[ERROR] Thông tin cuộc họp không hợp lệ!");
                }

                string accessToken = await GetAccessToken();
                Console.WriteLine($"[DEBUG] Zoom Access Token: {accessToken}");

                string zoomUserId = "me"; // Hoặc ID cụ thể của host
                string url = $"https://api.zoom.us/v2/users/{zoomUserId}/meetings";

                string startTimeUtc = meeting.StartTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                Console.WriteLine($"[DEBUG] StartTime gửi lên Zoom: {startTimeUtc}");

                var requestBody = new
                {
                    topic = meeting.Title ?? "No Title",
                    type = 2,
                    start_time = startTimeUtc,
                    timezone = "UTC",
                    agenda = $"{meeting.Note ?? "No Note"}\nRecording: https://drive.google.com/drive/folders/1O-DOOziPi7tzHbn6H0Xnfi3J4N-hAQBf?usp=sharing",
                    settings = new
                    {
                        host_video = true,
                        participant_video = true,
                        join_before_host = false,
                        mute_upon_entry = true,
                        approval_type = 0,
                        waiting_room = true
                    }
                };



                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                Console.WriteLine($"[DEBUG] Zoom API Request: {jsonRequest}");

                var jsonContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[DEBUG] Zoom API Response: {responseString}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Zoom API Error: {response.StatusCode} - {responseString}");
                }

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return responseData?.join_url;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi trong CreateMeeting: {ex.Message}");
                throw;
            }
        }


    }
}
