using System;
namespace testMIC.Database.Model
{
    public class Event
    {
        public int Id { get; set; }
        public string Datetime { get; set; }
        public int Serviceid { get; set; }
        public int? Appointmentid { get; set; }
    }
}
