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
        private readonly string _baseUrl;
        private readonly string _soapUrl;
        private readonly string _username;
        private readonly string _password;

        public StudentService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _baseUrl = configuration["BCSettings:BaseUrl"];

            _soapUrl = configuration["BCSettings:SOAPUrl"];

            _username = configuration["BCSettings:Username"];

            _password = configuration["BCSettings:Password"];
        }

        public async Task<ODataResponse<Student>> GetStudents()
        {
            string baseUrl = _configuration["BCSettings:BaseUrl"]!;

            string username = _configuration["BCSettings:Username"]!;

            string password = _configuration["BCSettings:Password"]!;

            string url = $"{baseUrl}/StudentsCard";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            ODataResponse<Student>? result = JsonSerializer.Deserialize<ODataResponse<Student>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result!;
        }

        public async Task<Student?> GetStudent(string studentCode)
        {
            var result = await GetStudents();

            return result.Value.FirstOrDefault(student => student.Code == studentCode);
        }
        public async Task<Student?> GetStudentbyStudentCode(string studentCode)
        {
            var result = await GetStudents();

            return result.Value.FirstOrDefault(student => student.Portal_User_Id.Replace("/", "") == studentCode);
        }
        public async Task<List<StudentFeeEntry>> GetStudentFeeEntries(string customerNo)
        {
            var baseUrl = _configuration["BCSettings:BaseUrl"];
            var endpoint = _configuration["BCSettings:StudentFeesEndpoint"];

            var username = _configuration["BCSettings:Username"];
            var password = _configuration["BCSettings:Password"];

            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var url = $"{baseUrl}/{endpoint}" + $"?$filter=Customer_No eq '{customerNo}'";

            var result = await _httpClient.GetFromJsonAsync<ODataResponse<StudentFeeEntry>>(url);

            return result?.Value ?? new List<StudentFeeEntry>();

        }

        public async Task<StudentFeeSummary> GetStudentFeeSummary(string customerNo)
        {
            var entries = await GetStudentFeeEntries(customerNo);

            var summary = new StudentFeeSummary
            {
                BilledFees = entries.Where(entry => entry.Amount_LCY > 0).Sum(entry => entry.Amount_LCY),
                PaidFees = Math.Abs(entries.Where(entry => entry.Amount_LCY < 0).Sum(entry => entry.Amount_LCY)),
                Balance = entries.Sum(entry => entry.Amount_LCY)

            };
            return summary;
        }

        public async Task<bool> UpdateStudentPhone(string studentCode, string phoneNumber)
        {
            string baseUrl = _configuration["BCSettings:BaseUrl"]!;

            string username = _configuration["BCSettings:Username"]!;

            string password = _configuration["BCSettings:Password"]!;

            var student = await GetStudentbyStudentCode(studentCode);

            if (student == null)
            {
                return false;
            }

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            string url = $"{baseUrl}/StudentsCard('{student.Code}')";

            var updateData = new
            {
                Phone_No = phoneNumber
            };

            string json = JsonSerializer.Serialize(updateData);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, url);

            request.Headers.TryAddWithoutValidation("If-Match", student.ETag);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;


            //now the error from BC
            //        HttpResponseMessage response =
            //await _httpClient.SendAsync(request);

            //        string responseBody =
            //            await response.Content.ReadAsStringAsync();

            //        if (!response.IsSuccessStatusCode)
            //        {
            //            throw new Exception(
            //                $"BC Error {response.StatusCode}: {responseBody}");
            //        }

            //        return true;
        }

        public async Task<List<CourseRegistrationLineCreate>> GetCourseRegistrationLines(string registrationCode)
        {
            string baseUrl = _configuration["BCSettings:BaseUrl"]!;

            string username = _configuration["BCSettings:Username"]!;

            string password = _configuration["BCSettings:Password"]!;

            string url = $"{baseUrl}/CourseRegistrationLines?$filter=Registration_Code eq '{registrationCode}'";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ODataResponse<CourseRegistrationLineCreate>>(json,
                  new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true
                  });


            return result?.Value ?? new List<CourseRegistrationLineCreate>();
        }
        public async Task<List<CourseRegistration>> GetCourseRegistrations(string studentCode, string yearOfStudySemester)
        {
            string baseUrl = _configuration["BCSettings:BaseUrl"]!;

            string username = _configuration["BCSettings:Username"]!;

            string password = _configuration["BCSettings:Password"]!;

            string url = $"{baseUrl}/CourseRegistration?$filter=Student_Code eq '{studentCode}'" + $"and Year_of_Study_Semester eq '{yearOfStudySemester}'";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ODataResponse<CourseRegistration>>(json,
                  new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true
                  });


            return result?.Value ?? new List<CourseRegistration>();
        }


        public async Task<List<UnitLine>> GetAvailableUnits(string programmeCode, string yearOfStudy)
        {
            string baseUrl = _configuration["BCSettings:BaseUrl"]!;

            string username = _configuration["BCSettings:Username"]!;

            string password = _configuration["BCSettings:Password"]!;

            string url = $"{baseUrl}/UnitsLines?$filter=Programme_Code eq '{programmeCode}'" + $" and Year_of_Study eq '{yearOfStudy}'" + $" and Course_Code ne ''";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ODataResponse<UnitLine>>(json,
                  new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true
                  });

            return result?.Value ?? new List<UnitLine>();


        }

        public async Task<CourseRegistration?> CreateCourseRegistration(CreateCourseRegistration registration)
        {
            string baseUrl = _configuration["BCSettings:BaseUrl"]!;

            string username = _configuration["BCSettings:Username"]!;

            string password = _configuration["BCSettings:Password"]!;

            string url = $"{baseUrl}/CourseRegistration";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            request.Content = JsonContent.Create(registration);


            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Create Course Registration Error: " +
                    $"{response.StatusCode} - {json}");
            }

            var result = JsonSerializer.Deserialize<CourseRegistration>(json,
                  new JsonSerializerOptions
                  {
                      PropertyNameCaseInsensitive = true
                  });

            return result;



        }

        public async Task<CourseRegistrationLineCreate?> CreateCourseRegistrationLine(CourseRegistrationLineCreate line)
        {
            string baseUrl = _configuration["BCSettings:BaseUrl"]!;

            string username = _configuration["BCSettings:Username"]!;

            string password = _configuration["BCSettings:Password"]!;

            string url = $"{baseUrl}/CourseRegistrationLines";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            request.Content = JsonContent.Create(line);

            var response = await _httpClient.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Create Course Registration Line Error: " + $"{response.StatusCode} - {json}");
            }

            var result = JsonSerializer.Deserialize<CourseRegistrationLineCreate>(
             json,
             new JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true
             });

            return result;

        }

        public async Task RegisterUnits(string studentNo, string yearOfStudy, string programmeCode)
        {
            var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope
            xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">

            <soap:Body>

                <RegisterUnits
                    xmlns=""urn:microsoft-dynamics-schemas/codeunit/PortalIntegration"">

                    <studentNo>{studentNo}</studentNo>
                    <yearofStudy>{yearOfStudy}</yearofStudy>
                    <programmeCode>{programmeCode}</programmeCode>

                </RegisterUnits>

            </soap:Body>

            </soap:Envelope>";

            var request = new HttpRequestMessage(HttpMethod.Post, _soapUrl);

            request.Headers.Add("SOAPAction", "urn:microsoft-dynamics-schemas/codeunit/PortalIntegration:RegisterUnits");

            request.Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);


            var response = await _httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"BC SOAP Error: {response.StatusCode} - {responseBody}");
            }
        }
    }
}