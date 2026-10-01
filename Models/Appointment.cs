using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models
{
    public class Appointment
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public string AppointmentDate { get; set; } = string.Empty;

        public string? Reason { get; set; }
    }
}
