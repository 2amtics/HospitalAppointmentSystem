using HospitalAppointmentSystem.Extensions;
using HospitalAppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HospitalAppointmentSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public DoctorController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // Doctor list with IMemoryCache
        public IActionResult Index()
        {
            const string cacheKey = "DoctorList";

            if (!_memoryCache.TryGetValue(cacheKey, out List<Doctor>? doctors))
            {
                doctors = GetDoctors();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                _memoryCache.Set(cacheKey, doctors, cacheOptions);
            }

            return View(doctors);
        }

        // Doctor details using Query String
        // Example:
        // /Doctor/Details?DoctorId=1
        [ResponseCache(Duration = 60)]
        public IActionResult Details(int DoctorId)
        {
            const string cacheKey = "DoctorList";

            if (!_memoryCache.TryGetValue(cacheKey, out List<Doctor>? doctors))
            {
                doctors = GetDoctors();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                _memoryCache.Set(cacheKey, doctors, cacheOptions);
            }

            var doctor = doctors?.FirstOrDefault(d => d.DoctorId == DoctorId);

            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.TotalCharge = doctor.CalculateTotalCharge();

            return View(doctor);
        }

        // Display appointment form
        public IActionResult Appointment(int DoctorId)
        {
            const string cacheKey = "DoctorList";

            if (!_memoryCache.TryGetValue(cacheKey, out List<Doctor>? doctors))
            {
                doctors = GetDoctors();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                _memoryCache.Set(cacheKey, doctors, cacheOptions);
            }

            var doctor = doctors?.FirstOrDefault(d => d.DoctorId == DoctorId);

            if (doctor == null)
            {
                return NotFound();
            }

            // Session: store selected specialization
            HttpContext.Session.SetString(
                "SelectedSpecialization",
                doctor.Specialization);

            ViewBag.Doctor = doctor;

            return View(new Appointment
            {
                DoctorId = doctor.DoctorId
            });
        }

        // Process appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Appointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                const string cacheKey = "DoctorList";

                if (!_memoryCache.TryGetValue(cacheKey, out List<Doctor>? doctors))
                {
                    doctors = GetDoctors();

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(3));

                    _memoryCache.Set(cacheKey, doctors, cacheOptions);
                }

                ViewBag.Doctor = doctors?
                    .FirstOrDefault(d => d.DoctorId == appointment.DoctorId);

                return View(appointment);
            }

            // Cookie: store patient name
            Response.Cookies.Append(
                "PatientName",
                appointment.PatientName,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

            // Get specialization from Session
            string? specialization =
                HttpContext.Session.GetString("SelectedSpecialization");

            TempData["SuccessMessage"] =
                $"Appointment request submitted successfully for {appointment.PatientName}.";

            TempData["Specialization"] = specialization;

            return RedirectToAction(
                "AppointmentSuccess",
                new { DoctorId = appointment.DoctorId });
        }

        public IActionResult AppointmentSuccess(int DoctorId)
        {
            ViewBag.DoctorId = DoctorId;

            // Read patient name from Cookie
            if (Request.Cookies.TryGetValue("PatientName", out string? patientName))
            {
                ViewBag.PatientName = patientName;
            }

            // Read specialization from Session
            ViewBag.Specialization =
                HttpContext.Session.GetString("SelectedSpecialization");

            return View();
        }

        private List<Doctor> GetDoctors()
        {
            return new List<Doctor>
            {
                new Doctor
                {
                    DoctorId = 1,
                    DoctorName = "Dr. Rajesh Patel",
                    Specialization = "Cardiologist",
                    Experience = 15,
                    ConsultationFee = 800
                },

                new Doctor
                {
                    DoctorId = 2,
                    DoctorName = "Dr. Priya Shah",
                    Specialization = "Dermatologist",
                    Experience = 10,
                    ConsultationFee = 600
                },

                new Doctor
                {
                    DoctorId = 3,
                    DoctorName = "Dr. Amit Mehta",
                    Specialization = "Neurologist",
                    Experience = 12,
                    ConsultationFee = 1000
                },

                new Doctor
                {
                    DoctorId = 4,
                    DoctorName = "Dr. Neha Desai",
                    Specialization = "Pediatrician",
                    Experience = 8,
                    ConsultationFee = 500
                }
            };
        }
    }
}
