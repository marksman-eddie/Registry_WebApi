using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace testMIC.Database.Model
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int ServiceId { get; set; }
        [ForeignKey("PatientId")]
        public Patient AppointmentPatient { get; set; }
        [ForeignKey("ServiceId")]
        public Service AppointmentService { get; set; }

       

    }
}
