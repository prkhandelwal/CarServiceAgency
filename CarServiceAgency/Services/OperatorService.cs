using CarServiceAgency.Enums;
using CarServiceAgency.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAgency.Services
{
    public interface IOperatorService
    {
        Task<bool> IsOperatorValid(string operatorId);
    }

    public class OperatorService : IOperatorService
    {
        private readonly IMongoCollection<Operator> _operator;

        public OperatorService(IMongoClient client)
        {
            var database = client.GetDatabase(nameof(Databases.CarServiceDb));
            _operator = database.GetCollection<Operator>(nameof(Operator));
        }

        public async Task<bool> IsOperatorValid(string operatorId)
        {
            return await _operator.Find(a => a.OperatorId.Equals(operatorId)).AnyAsync();
        }
    }
}
