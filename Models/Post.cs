// Declare the namespace containing the application models.
namespace HospitalAppointmentSystem.Models
{
    // Define the Post model to represent API post information.
    public class Post
    {
        // Store the user ID returned by the API.
        public int userId { get; set; }

        // Store the unique post ID returned by the API.
        public int id { get; set; }

        // Store the title of the post returned by the API.
        public string title { get; set; } = string.Empty;

        // Store the body/content of the post returned by the API.
        public string body { get; set; } = string.Empty;
    }
}
