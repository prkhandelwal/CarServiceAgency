using CarServiceAgency.Enums;
using CarServiceAgency.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAgency.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> CreateAppointment(Appointment appointment);
        Task<List<Appointment>> GetByCustomerId(string customerId, string operatorId = "", string appointmentId = "");
        Task<bool> IsAppointmentValid(string appointmentId);
        Task<AppointmentResponse> UpdateAppointment(Appointment appointment);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IMongoCollection<Appointment> _appointments;
        private ICustomerService _customerService;
        private IOperatorService _operatorService;

        public AppointmentService(IMongoClient client, ICustomerService customerService, IOperatorService operatorService)
        {
            var database = client.GetDatabase(nameof(Databases.CarServiceDb));
            _appointments = database.GetCollection<Appointment>(nameof(Appointment));

            _customerService = customerService;
            _operatorService = operatorService;
        }

        public async Task<List<Appointment>> GetByCustomerId(string customerId, string operatorId = "", string appointmentId = "")
        {
            return await _appointments.Find<Appointment>(appointment =>
                appointment.CustomerId == customerId &&
                (string.IsNullOrEmpty(operatorId) || appointment.OperatorId.Equals(operatorId)) &&
                (string.IsNullOrEmpty(appointmentId) || appointment.AppointmentId.Equals(appointmentId))
            ).ToListAsync();
        }

        public async Task<AppointmentResponse> CreateAppointment(Appointment appointment)
        {
            var response = new AppointmentResponse();
            // Need to handle concurrency issues here
            var isSlotAvailable = await IsSlotAvailable(appointment.OperatorId, appointment.AppointmentTime, appointment.EndTime);
            if (isSlotAvailable)
            {
                var record = appointment with { AppointmentId = Guid.NewGuid().ToString() };
                await _appointments.InsertOneAsync(record);
                response.StatusCode = StatusCodes.Status201Created;
                response.Message = "Appointment successfully created";
            }
            else
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Message = "Slot not available for the selected operator";
            }
            return response;
        }

        public async Task<AppointmentResponse> UpdateAppointment(Appointment appointment)
        {
            var response = new AppointmentResponse();
            var isAppointmentValid = await IsAppointmentValid(appointment.AppointmentId);
            if (!isAppointmentValid)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Appointment is not valid";
                return response;
            }
            var isSlotUpdateAvailable = await IsSlotUpdateAvailable(appointment);
            if (isSlotUpdateAvailable)
            {
                await _appointments.ReplaceOneAsync(app => app.AppointmentId.Equals(appointment.AppointmentId), appointment);
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Appointment successfully updated";
            }
            else
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Slot update is not available";
            }
            return response;

        }
        public async Task<bool> IsAppointmentValid(string appointmentId)
        {
            return await _appointments.Find(a => a.AppointmentId.Equals(appointmentId)).AnyAsync();
        }

        private async Task<bool> IsSlotUpdateAvailable(Appointment appointment)
        {
            return !await _appointments.Find(a => !a.AppointmentId.Equals(appointment.AppointmentId)
            && a.OperatorId.Equals(appointment.OperatorId)
            && (appointment.AppointmentTime >= a.AppointmentTime && appointment.AppointmentTime <= a.EndTime)
            && (appointment.EndTime >= a.AppointmentTime && appointment.EndTime <= a.EndTime))
            .AnyAsync();
        }

        private async Task<bool> IsSlotAvailable(string operatorId, DateTime startTime, DateTime endTime)
        {
            return !await _appointments.Find(a => a.OperatorId.Equals(operatorId)
            && (startTime >= a.AppointmentTime && startTime <= a.EndTime)
            && (endTime >= a.AppointmentTime && endTime <= a.EndTime)
            && !a.isCancelled)
            .AnyAsync();
        }
    }
}
