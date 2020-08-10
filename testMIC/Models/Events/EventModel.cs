using System;
namespace testMIC.Models.Events
{
    public class EventModel
    {
        public int Id { get; set; }
        public string Datetime { get; set; }
        public int Serviceid { get; set; }
        public int? Appointmentid { get; set; }
    }
}
