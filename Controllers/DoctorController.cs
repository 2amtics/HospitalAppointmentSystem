// Import ASP.NET Core MVC classes such as Controller and IActionResult.
using Microsoft.AspNetCore.Mvc;

// Import IMemoryCache functionality for storing data in server memory.
using Microsoft.Extensions.Caching.Memory;

// Import the extension method CalculateTotalCharge().
using HospitalAppointmentSystem.Extensions;

// Import Doctor and Appointment model classes.
using HospitalAppointmentSystem.Models;

// Declare the namespace containing application controllers.
namespace HospitalAppointmentSystem.Controllers
{
    // Define the DoctorController class.
    // Controller provides MVC action functionality.
    public class DoctorController : Controller
    {
        // Declare a private variable to hold the memory cache service.
        private readonly IMemoryCache _memoryCache;

        // Constructor of DoctorController.
        // IMemoryCache is automatically provided by dependency injection.
        public DoctorController(IMemoryCache memoryCache)
        {
            // Store the injected memory cache object in the private variable.
            _memoryCache = memoryCache;
        }

        // Display the list of all doctors.
        public IActionResult Index()
        {
            // Define a unique key used to store and retrieve the doctor list from memory cache.
            const string cacheKey = "DoctorList";

            // Try to retrieve the doctor list from memory cache.
            // If the cache contains the data, it is assigned to the 'doctors' variable.
            if (!_memoryCache.TryGetValue(
                cacheKey,
                out List<Doctor>? doctors))
            {
                // If the doctor list is not present in cache,
                // create the doctor list using the GetDoctors() method.
                doctors = GetDoctors();

                // Create options that control how long the cached data remains valid.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Set the cache to expire automatically after 3 minutes.
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                // Store the doctor list in memory cache using the specified key and options.
                _memoryCache.Set(
                    cacheKey,
                    doctors,
                    cacheOptions);
            }

            // Send the doctor list to the Index.cshtml view.
            return View(doctors);
        }

        // Display details of a selected doctor.
        // Example query string URL:
        // /Doctor/Details?DoctorId=1
        [ResponseCache(Duration = 60)]
        public IActionResult Details(int DoctorId)
        {
            // Define the cache key used for the doctor list.
            const string cacheKey = "DoctorList";

            // Try to retrieve the doctor list from memory cache.
            if (!_memoryCache.TryGetValue(
                cacheKey,
                out List<Doctor>? doctors))
            {
                // If the doctor list is not cached, retrieve the data.
                doctors = GetDoctors();

                // Create memory cache options.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Cache the doctor list for 3 minutes.
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                // Store the doctor list in memory cache.
                _memoryCache.Set(
                    cacheKey,
                    doctors,
                    cacheOptions);
            }

            // Search the doctor list for the doctor whose ID matches the query string value.
            var doctor = doctors?
                .FirstOrDefault(d => d.DoctorId == DoctorId);

            // Check whether the requested doctor was found.
            if (doctor == null)
            {
                // Return HTTP 404 Not Found if the doctor does not exist.
                return NotFound();
            }

            // Calculate the total consultation charge using the extension method.
            ViewBag.TotalCharge = doctor.CalculateTotalCharge();

            // Send the selected doctor to the Details.cshtml view.
            return View(doctor);
        }

        // Display the appointment form for a selected doctor.
        public IActionResult Appointment(int DoctorId)
        {
            // Define the cache key for the doctor list.
            const string cacheKey = "DoctorList";

            // Try to retrieve doctors from memory cache.
            if (!_memoryCache.TryGetValue(
                cacheKey,
                out List<Doctor>? doctors))
            {
                // Load the doctor list if it is not already cached.
                doctors = GetDoctors();

                // Create memory cache options.
                var cacheOptions = new MemoryCacheEntryOptions()
                    // Store the doctor list in cache for 3 minutes.
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                // Save the doctor list in memory cache.
                _memoryCache.Set(
                    cacheKey,
                    doctors,
                    cacheOptions);
            }

            // Find the selected doctor using the DoctorId received from the request.
            var doctor = doctors?
                .FirstOrDefault(d => d.DoctorId == DoctorId);

            // Check whether the selected doctor exists.
            if (doctor == null)
            {
                // Return HTTP 404 if the doctor does not exist.
                return NotFound();
            }

            // Store the selected doctor's specialization in session.
            HttpContext.Session.SetString(
                "SelectedSpecialization",
                doctor.Specialization);

            // Pass the selected doctor to the Razor view using ViewBag.
            ViewBag.Doctor = doctor;

            // Create a new Appointment object.
            return View(new Appointment
            {
                // Automatically set the selected DoctorId in the appointment.
                DoctorId = doctor.DoctorId
            });
        }

        // Handle the appointment form submission.
        [HttpPost]

        // Protect the form against Cross-Site Request Forgery attacks.
        [ValidateAntiForgeryToken]
        public IActionResult Appointment(Appointment appointment)
        {
            // Check whether all required form values are valid.
            if (!ModelState.IsValid)
            {
                // Define the cache key for the doctor list.
                const string cacheKey = "DoctorList";

                // Try to retrieve doctors from memory cache.
                if (!_memoryCache.TryGetValue(
                    cacheKey,
                    out List<Doctor>? doctors))
                {
                    // Load doctors if they are not in cache.
                    doctors = GetDoctors();

                    // Create cache configuration.
                    var cacheOptions = new MemoryCacheEntryOptions()
                        // Set cache expiration to 3 minutes.
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                    // Store the doctor list in memory cache.
                    _memoryCache.Set(
                        cacheKey,
                        doctors,
                        cacheOptions);
                }

                // Find the doctor associated with the submitted DoctorId.
                ViewBag.Doctor = doctors?
                    .FirstOrDefault(
                        d => d.DoctorId == appointment.DoctorId);

                // Return the appointment view with the entered form data.
                return View(appointment);
            }

            // Store the patient's name in a browser cookie.
            Response.Cookies.Append(
                "PatientName",
                appointment.PatientName,
                new CookieOptions
                {
                    // Prevent client-side JavaScript from reading the cookie.
                    HttpOnly = true,

                    // Mark the cookie as essential for the application.
                    IsEssential = true,

                    // Make the cookie expire after 7 days.
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

            // Retrieve the selected specialization from session.
            string? specialization =
                HttpContext.Session.GetString(
                    "SelectedSpecialization");

            // Store a success message temporarily using TempData.
            TempData["SuccessMessage"] =
                $"Appointment request submitted successfully for {appointment.PatientName}.";

            // Store the specialization temporarily using TempData.
            TempData["Specialization"] = specialization;

            // Redirect the user to the appointment success page.
            return RedirectToAction(
                "AppointmentSuccess",
                new
                {
                    // Pass the submitted DoctorId to the next action.
                    DoctorId = appointment.DoctorId
                });
        }

        // Display the appointment success page.
        public IActionResult AppointmentSuccess(int DoctorId)
        {
            // Send the DoctorId to the Razor view using ViewBag.
            ViewBag.DoctorId = DoctorId;

            // Try to retrieve the patient name stored in the cookie.
            if (Request.Cookies.TryGetValue(
                "PatientName",
                out string? patientName))
            {
                // Store the cookie value in ViewBag for display.
                ViewBag.PatientName = patientName;
            }

            // Retrieve the selected specialization from session.
            ViewBag.Specialization =
                HttpContext.Session.GetString(
                    "SelectedSpecialization");

            // Return the AppointmentSuccess view.
            return View();
        }

        // Create and return the sample doctor list.
        private List<Doctor> GetDoctors()
        {
            // Return a list containing multiple Doctor objects.
            return new List<Doctor>
            {
                // Create the first doctor.
                new Doctor
                {
                    // Set the unique ID of the doctor.
                    DoctorId = 1,

                    // Set the doctor's name.
                    DoctorName = "Dr. Rajesh Patel",

                    // Set the doctor's specialization.
                    Specialization = "Cardiologist",

                    // Set the doctor's experience in years.
                    Experience = 15,

                    // Set the doctor's consultation fee.
                    ConsultationFee = 800
                },

                // Create the second doctor.
                new Doctor
                {
                    // Set the unique ID.
                    DoctorId = 2,

                    // Set the doctor's name.
                    DoctorName = "Dr. Priya Shah",

                    // Set the doctor's specialization.
                    Specialization = "Dermatologist",

                    // Set the doctor's experience.
                    Experience = 10,

                    // Set the consultation fee.
                    ConsultationFee = 600
                },

                // Create the third doctor.
                new Doctor
                {
                    // Set the unique ID.
                    DoctorId = 3,

                    // Set the doctor's name.
                    DoctorName = "Dr. Amit Mehta",

                    // Set the doctor's specialization.
                    Specialization = "Neurologist",

                    // Set the doctor's experience.
                    Experience = 12,

                    // Set the consultation fee.
                    ConsultationFee = 1000
                },

                // Create the fourth doctor.
                new Doctor
                {
                    // Set the unique ID.
                    DoctorId = 4,

                    // Set the doctor's name.
                    DoctorName = "Dr. Neha Desai",

                    // Set the doctor's specialization.
                    Specialization = "Pediatrician",

                    // Set the doctor's experience.
                    Experience = 8,

                    // Set the consultation fee.
                    ConsultationFee = 500
                }
            };
        }
    }
}
