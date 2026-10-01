// Import classes required for JSON serialization and deserialization.
using System.Text.Json;

// Import ASP.NET Core MVC functionality.
using Microsoft.AspNetCore.Mvc;

// Import the Post model.
using HospitalAppointmentSystem.Models;

// Declare the controller namespace.
namespace HospitalAppointmentSystem.Controllers
{
    // Define the HomeController class.
    public class HomeController : Controller
    {
        // Declare a private variable to store IHttpClientFactory.
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor receives IHttpClientFactory through dependency injection.
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            // Store the injected HttpClientFactory.
            _httpClientFactory = httpClientFactory;
        }

        // Define an asynchronous action method for retrieving posts.
        public async Task<IActionResult> Posts()
        {
            // Create an HttpClient using the registered HttpClientFactory.
            var client = _httpClientFactory.CreateClient();

            // Send an asynchronous GET request to the JSONPlaceholder API.
            var response = await client.GetAsync(
                "https://jsonplaceholder.typicode.com/posts");

            // Check whether the API returned an unsuccessful status code.
            if (!response.IsSuccessStatusCode)
            {
                // Return an empty post list if the request fails.
                return View(new List<Post>());
            }

            // Read the API response content asynchronously as a string.
            var json = await response.Content.ReadAsStringAsync();

            // Convert the JSON string into a list of Post objects.
            var posts = JsonSerializer.Deserialize<List<Post>>(
                json,
                new JsonSerializerOptions
                {
                    // Allow JSON property names to match C# properties without case sensitivity.
                    PropertyNameCaseInsensitive = true
                });

            // Return the Posts view with the retrieved posts.
            // If deserialization returns null, send an empty list instead.
            return View(posts ?? new List<Post>());
        }
    }
}
