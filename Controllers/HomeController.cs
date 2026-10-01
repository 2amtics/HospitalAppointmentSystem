using System.Text.Json;
using HospitalAppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Posts()
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(
                "https://jsonplaceholder.typicode.com/posts");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<Post>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var posts = JsonSerializer.Deserialize<List<Post>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(posts ?? new List<Post>());
        }
    }
}
