using System;
namespace testMIC.Models.Appointments
{
    public class AppointmentModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ServiceId { get; set; }
        public string NamePatient { get; set; }
        public string NameService { get; set; }
    }
}
