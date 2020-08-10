using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using testMIC.Database.Model;
using testMIC.Database;
using testMIC.Models.Appointments;
using testMIC.Models.Events;
using testMIC.Models.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;
using Microsoft.Data.Sqlite;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace testMIC.Controllers
{
    [Route("api/[controller]")]
    public class RegistryController : Controller
    {
        private static Logger _Logger = LogManager.GetCurrentClassLogger();
        private readonly TestDatabaseContext _context;

        public RegistryController(TestDatabaseContext context)
        {
            _context = context;
        }
       
        [HttpGet("getAppointmentsUserId")]
        public List<AppointmentModel> GetAppointmentsPatientId(int PatientID)
        {
            try
            {                
                _Logger.Info($"получить назначения для пациента с ID {PatientID}");
                var appointment = _context.Appointment
                     .Include(p => p.AppointmentPatient)
                     .Include(s => s.AppointmentService)
                     .Where(a => a.PatientId == PatientID).ToList();
                if (appointment.Count ==0)
                {
                    _Logger.Info("список назначений пуст");
                }
                List<AppointmentModel> appointmentsList = new List<AppointmentModel>();
                foreach (var appointmentRow in appointment)
                {
                    if (_context.Event.Any(i => i.Appointmentid == appointmentRow.Id))
                    {
                        continue;
                    }
                    AppointmentModel appointmentModel = new AppointmentModel();

                    appointmentModel.Id = appointmentRow.Id;
                    appointmentModel.PatientId = appointmentRow.PatientId;
                    appointmentModel.ServiceId = appointmentRow.ServiceId;
                    appointmentModel.NamePatient = appointmentRow.AppointmentPatient.Name;
                    appointmentModel.NameService = appointmentRow.AppointmentService.Name;
                    appointmentsList.Add(appointmentModel);
                }
                _Logger.Info($"Получено {appointmentsList.Count} назначений в коллекцию");
                return appointmentsList;

            }

            catch(Exception ex)
            {
                _Logger.Error(ex.Message);
                return null;
            }
        }


        [HttpGet("getAppointmentsUserName")]
        public List<AppointmentModel> GetAppointmentsUserName(string PatientName)
        {            
            try
            {
                _Logger.Info($"получить назначения для пользователя с именем {PatientName}");
                var appointment = _context.Appointment
                   .Include(p => p.AppointmentPatient)
                   .Include(s => s.AppointmentService)
                   .Where(a => EF.Functions.Like(a.AppointmentPatient.Name, $"%{PatientName}%")).ToList();
                if (appointment.Count == 0)
                {
                    _Logger.Info($"Список назначений по пациенту {PatientName} пуст");
                }
                List<AppointmentModel> appointmentsList = new List<AppointmentModel>();
                foreach (var appointmentRow in appointment)
                {
                    if (_context.Event.Any(i => i.Appointmentid == appointmentRow.Id))
                    {
                        continue;
                    }
                    AppointmentModel appointmentModel = new AppointmentModel();
                    appointmentModel.Id = appointmentRow.Id;
                    appointmentModel.PatientId = appointmentRow.PatientId;
                    appointmentModel.ServiceId = appointmentRow.ServiceId;
                    appointmentModel.NamePatient = appointmentRow.AppointmentPatient.Name;
                    appointmentModel.NameService = appointmentRow.AppointmentService.Name;
                    appointmentsList.Add(appointmentModel);
                }

                _Logger.Info($"Получено {appointmentsList.Count} назначений в коллекцию");
                return appointmentsList;
            }

            catch (Exception ex)
            {
                _Logger.Error(ex.Message);
                return null;
            }

}

        [HttpGet("getEventToDate")]
        public EventForPatientAppointmentsModel GetEventToDate(int IdPatient)
        {
            try
            {
                List<AppointmentModel> appointments = new List<AppointmentModel>();
                var appointmentCollection = _context.Appointment
                    .Include(s => s.AppointmentService)
                    .Where(a => a.PatientId == IdPatient).ToList();
                                
                foreach (var patientAppointment in appointmentCollection)
                {
                    AppointmentModel appointmentModel = new AppointmentModel();
                    appointmentModel.Id = patientAppointment.Id;
                    appointmentModel.PatientId = patientAppointment.PatientId;
                    appointmentModel.ServiceId = patientAppointment.ServiceId;
                    appointmentModel.NameService = patientAppointment.AppointmentService.Name;
                    appointments.Add(appointmentModel);

                }
                EventForPatientAppointmentsModel eventForPatient = new EventForPatientAppointmentsModel();
                foreach (var appointment in appointments)
                {
                    var Events = _context.Event
                        .Where(x => x.Serviceid == appointment.ServiceId).ToList();
                    List<EventModel> EventServiceSort = new List<EventModel>();
                    foreach (var _event in Events)
                    {
                        if (_event.Appointmentid == null)
                        {
                            EventModel newEvent = new EventModel
                            {
                                Id = _event.Id,
                                Datetime = _event.Datetime,
                                Serviceid = _event.Serviceid
                            };

                            EventServiceSort.Add(newEvent);

                        }
                        continue;
                    }


                    string minDateEvent = EventServiceSort.Min(a => a.Datetime);
                    _Logger.Info($"Ближайшая дата для записи пациента на {appointment.NameService} - {minDateEvent}");
                    var result = EventServiceSort.FirstOrDefault(a => a.Datetime == minDateEvent);
                    eventForPatient.ListEventForPatient.Add(result);

                }

                return eventForPatient;
            }

            catch (Exception ex)
            {
                _Logger.Error(ex.Message);
                return null;
            }

        }
                        
        [HttpPost("addNewEvent")]
        public void AddNewEvent(int idEvent, int idAppointment)
        {
            try
            {                
                var Event = _context.Event.FirstOrDefault(e => e.Id == idEvent);
                var Appointment = _context.Appointment.FirstOrDefault(a => a.Id == idAppointment);
                _Logger.Info($"{Event.Datetime} записать {Appointment.AppointmentPatient} на {Appointment.AppointmentService}");
                Event.Appointmentid = idAppointment;
                _context.Entry(Event).State = EntityState.Modified;
                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                _Logger.Error(ex.Message);
            }

        }


        
    }
}
