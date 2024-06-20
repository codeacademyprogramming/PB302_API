using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Text;
using System.Text.Json;
using UniversityApp.UI.Filters;
using UniversityApp.UI.Models;
using UniversityApp.UI.Services;

namespace UniversityApp.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class StudentController : Controller
    {
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public StudentController(ICrudService crudService)
        {
            _client  = new HttpClient();
            _crudService = crudService;
        }
        public async Task<IActionResult> Index(int page=1,int size=4)
        {
            //_client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);
            //var queryString = new StringBuilder();
            //queryString.Append("?page=").Append(Uri.EscapeDataString(page.ToString()));
            //queryString.Append("&size=").Append(Uri.EscapeDataString(size.ToString()));

            //// Append query string to base URL
            //string requestUrl = "https://localhost:7061/api/students" + queryString;
            //using (var response = await _client.GetAsync(requestUrl))
            //{
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            //        var data = JsonSerializer.Deserialize<PaginatedResponse<StudentListItemGetResponse>>(await response.Content.ReadAsStringAsync(),options);
            //        if (data.TotalPages < page) return RedirectToAction("index", new { page = data.TotalPages });

            //        return View(data);
            //    }
            //    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //    {
            //        return RedirectToAction("login", "account");
            //    }
            //    else
            //    {
            //        return RedirectToAction("error", "home");
            //    }
            //}

            return View(await _crudService.GetAllPaginated<StudentListItemGetResponse>("students", page));
        }

        public async Task<IActionResult> Create()
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);

            ViewBag.Groups = await getGroups();
            
            if (ViewBag.Groups == null) return RedirectToAction("error", "home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentCreateRequest createRequest)
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]); MultipartFormDataContent content = new MultipartFormDataContent();

            var fileContent = new StreamContent(createRequest.File.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(createRequest.File.ContentType);

            content.Add(new StringContent(createRequest.FullName), "FullName");
            content.Add(new StringContent(createRequest.GroupId.ToString()), "GroupId");
            content.Add(new StringContent(createRequest.Email), "Email");
            content.Add(new StringContent(createRequest.BirthDate.ToLongDateString()), "BirthDate");
            //content.Add(new StringContent(JsonSerializer.Serialize(createRequest),System.Text.Encoding.UTF8,"application/json"),"json");
            content.Add(fileContent, "File", createRequest.File.FileName);


            using(var response = await _client.PostAsync("https://localhost:7061/api/students", content))
            {

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ViewBag.Groups = await getGroups();

                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);

                    foreach (var item in errorResponse.Errors)
                        ModelState.AddModelError(item.Key, item.Message);
                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }
            return View(createRequest);
        }

        private async Task<List<GroupListItemGetResponse>> getGroups()
        {
            using (var response = await _client.GetAsync("https://localhost:7061/api/groups/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<GroupListItemGetResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }
    }
}
