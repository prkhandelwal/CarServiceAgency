using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAgency.Models
{
    //public abstract class User
    //{
    //    [BsonId]
    //    public string UserId { get; set; }
    //    public string UserName { get; set; }
    //}

    public record Operator([property: BsonId] string OperatorId, string OperatorName);

    public record Customer([property: BsonId] string CustomerId, string CustomerName);
}
