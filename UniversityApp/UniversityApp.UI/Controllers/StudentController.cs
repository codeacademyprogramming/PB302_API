﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using UniversityApp.UI.Filters;
using UniversityApp.UI.Models;

namespace UniversityApp.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class StudentController : Controller
    {
        private HttpClient _client;
        public StudentController()
        {
            _client  = new HttpClient();    
        }
        public async Task<IActionResult> Index(int page=1,int size=4)
        {
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, Request.Cookies["token"]);
            var queryString = new StringBuilder();
            queryString.Append("?page=").Append(Uri.EscapeDataString(page.ToString()));
            queryString.Append("&size=").Append(Uri.EscapeDataString(size.ToString()));

            // Append query string to base URL
            string requestUrl = "https://localhost:7061/api/students" + queryString;
            using (var response = await _client.GetAsync(requestUrl))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<PaginatedResponse<StudentListItemGetResponse>>(await response.Content.ReadAsStringAsync(),options);
                    if (data.TotalPages < page) return RedirectToAction("index", new { page = data.TotalPages });

                    return View(data);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
        }
    }
}
