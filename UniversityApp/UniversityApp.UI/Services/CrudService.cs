using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using UniversityApp.UI.Exceptions;
using UniversityApp.UI.Models;

namespace UniversityApp.UI.Services
{
    public class CrudService : ICrudService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _client;
        private const string baseUrl = "https://localhost:7061/api/";
        public CrudService(IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaginatedResponse<TResponse>> GetAllPaginated<TResponse>(string path, int page)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);
          
            using (var response = await _client.GetAsync(baseUrl+path + "?page=" + page))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<PaginatedResponse<TResponse>>(await response.Content.ReadAsStringAsync(),options);
                }
                else throw new HttpException(response.StatusCode);
            }
        }

        public async Task<TResponse> Get<TResponse>(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else { throw new HttpException(response.StatusCode);}
            }
        }

        public async Task<CreateResponse> Create<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PostAsync(baseUrl+path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponse>(await response.Content.ReadAsStringAsync(),options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }

        public async Task Update<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PutAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode,errorResponse.Message);
                }
            }
        }

        public async Task Delete(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.DeleteAsync(baseUrl + path))
            {
                if (!response.IsSuccessStatusCode)
                    throw new HttpException(response.StatusCode);
            }
        }

        public async Task<CreateResponse> CreateFromForm<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            MultipartFormDataContent content = new MultipartFormDataContent();
            foreach (var prop in request.GetType().GetProperties())
            {
                var val = prop.GetValue(request);

                if(val is IFormFile file)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, prop.Name, file.FileName);
                }
                else if(val is DateTime dateTime)
                    content.Add(new StringContent(dateTime.ToLongDateString()), prop.Name);
                else if (val is not null)
                    content.Add(new StringContent(val.ToString()),prop.Name);
            }

            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }
    }
}
