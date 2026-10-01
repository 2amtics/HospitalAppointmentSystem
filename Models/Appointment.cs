// Import the DataAnnotations namespace for validation attributes.
using System.ComponentModel.DataAnnotations;

// Declare the namespace for application models.
namespace HospitalAppointmentSystem.Models
{
    // Define the Appointment model class.
    public class Appointment
    {
        // Mark DoctorId as a required property.
        [Required]

        // Store the ID of the doctor selected by the patient.
        public int DoctorId { get; set; }

        // Mark PatientName as a required property.
        [Required]

        // Store the name of the patient.
        public string PatientName { get; set; } = string.Empty;

        // Mark AppointmentDate as a required property.
        [Required]

        // Store the requested appointment date.
        public string AppointmentDate { get; set; } = string.Empty;

        // Store the reason for requesting the appointment.
        // The '?' means this property can contain a null value.
        public string? Reason { get; set; }
    }
}
