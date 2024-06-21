using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Text;
using System.Text.Json;
using UniversityApp.UI.Exceptions;
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
            try
            {
                await _crudService.CreateFromForm(createRequest, "students");
                return RedirectToAction("index");
            }
            catch (ModelException ex)
            {
                foreach (var item in ex.Error.Errors)
                    ModelState.AddModelError(item.Key, item.Message);

                ViewBag.Groups = await _crudService.Get<List<GroupListItemGetResponse>>("groups/all");

                return View();
            }
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
