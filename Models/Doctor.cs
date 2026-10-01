// Declare the namespace that contains all application models.
namespace HospitalAppointmentSystem.Models
{
    // Define the Doctor model class.
    public class Doctor
    {
        // Store the unique identification number of the doctor.
        public int DoctorId { get; set; }

        // Store the full name of the doctor.
        public string DoctorName { get; set; } = string.Empty;

        // Store the medical specialization of the doctor.
        public string Specialization { get; set; } = string.Empty;

        // Store the number of years of experience of the doctor.
        public int Experience { get; set; }

        // Store the consultation fee charged by the doctor.
        public decimal ConsultationFee { get; set; }
    }
}
