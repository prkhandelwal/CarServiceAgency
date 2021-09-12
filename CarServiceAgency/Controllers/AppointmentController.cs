using CarServiceAgency.Models;
using CarServiceAgency.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
        }

        // For getting all appointnts by customerId and filter
        [HttpGet]
        public async Task<ActionResult<List<Appointment>>> Get(string customerId, string operatorId = "", string appointmentId = "")
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    return BadRequest("Customer id cannot be empty");
                }
                var appointments = await _appointmentService.GetByCustomerId(customerId, operatorId, appointmentId);
                return appointments;
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Result = "Unexpected exception occured", e.Message });
            }
        }

        //For creating appointment
        [HttpPost]
        public async Task<ActionResult<AppointmentResponse>> Post(Appointment appointment)
        {
            try
            {
                if (appointment.CustomerId is null || appointment.OperatorId is null)
                {
                    return new BadRequestResult();
                }
                var res = await _appointmentService.CreateAppointment(appointment);
                return StatusCode(res.StatusCode, res);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new {Result = "Unexpected exception occured", e.Message });
            }
        }

        //For modifying appointment: rescheduling / Cancelling
        [HttpPut]
        public async Task<ActionResult<AppointmentResponse>> Put(Appointment appointment)
        {
            try
            {
                if (appointment.CustomerId is null || appointment.OperatorId is null)
                {
                    return new BadRequestResult();
                }
                var res = await _appointmentService.UpdateAppointment(appointment);
                return StatusCode(res.StatusCode, res);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { Result = "Unexpected exception occured", e.Message });
            }
        }
    }
}
