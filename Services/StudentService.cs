using PORTAL.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PORTAL.Services
{
    public class StudentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public StudentService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ODataResponse<Student>> GetStudents()
        {
            string baseUrl =
                _configuration["BCSettings:BaseUrl"]!;

            string username =
                _configuration["BCSettings:Username"]!;

            string password =
                _configuration["BCSettings:Password"]!;

            string url = $"{baseUrl}/StudentsCard";

            string credentials =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        $"{username}:{password}"));

            using HttpRequestMessage request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    credentials);

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync();

            ODataResponse<Student>? result =
             JsonSerializer.Deserialize<ODataResponse<Student>>(
                 json,
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });
            return result!;
        }

        public async Task<Student?> GetStudent(string studentCode)
        {
            var result = await GetStudents();

            return result.Value.FirstOrDefault(
                student => student.Code == studentCode);
        }
        public async Task<Student?> GetStudentbyStudentCode(string studentCode)
        {
            var result = await GetStudents();

            return result.Value.FirstOrDefault(
                student => student.Portal_User_Id.Replace("/", "") == studentCode);
        }
        public async Task<List<StudentFeeEntry>> GetStudentFeeEntries(
    string customerNo)
        {
            var baseUrl = _configuration["BCSettings:BaseUrl"];
            var endpoint = _configuration["BCSettings:StudentFeesEndpoint"];

            var username = _configuration["BCSettings:Username"];
            var password = _configuration["BCSettings:Password"];

            var credentials =
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes(
                        $"{username}:{password}"));

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);

            var url =
                $"{baseUrl}/{endpoint}" +
                $"?$filter=Customer_No eq '{customerNo}'";

            var result =
                await _httpClient.GetFromJsonAsync<
                    ODataResponse<StudentFeeEntry>>(url);

            return result?.Value ?? new List<StudentFeeEntry>();

        }


    }
}