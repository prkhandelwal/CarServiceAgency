using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAgency.Models
{
    public record Appointment(
        [property: BsonId] string AppointmentId,
        [field: Required(AllowEmptyStrings = false)] string CustomerId,
        [field: Required(AllowEmptyStrings = false)] string OperatorId,
        DateTime AppointmentTime,
        bool isCancelled)
    {
        public DateTime EndTime = AppointmentTime.AddHours(1);
    }

    public class AppointmentResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; } 
    }
}
