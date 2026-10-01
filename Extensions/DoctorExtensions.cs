// Import the Doctor model so the extension method can work with Doctor objects.
using HospitalAppointmentSystem.Models;

// Declare the namespace for extension methods.
namespace HospitalAppointmentSystem.Extensions
{
    // Define a static class because extension methods must be declared inside a static class.
    public static class DoctorExtensions
    {
        // Define an extension method named CalculateTotalCharge.
        // The 'this Doctor doctor' parameter allows this method to be called directly on a Doctor object.
        public static decimal CalculateTotalCharge(this Doctor doctor)
        {
            // Add the fixed ₹100 service charge to the doctor's consultation fee.
            decimal totalCharge = doctor.ConsultationFee + 100;

            // Return the final consultation charge.
            return totalCharge;
        }
    }
}
