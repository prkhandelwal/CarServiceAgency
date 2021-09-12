using CarServiceAgency.Enums;
using CarServiceAgency.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAgency.Services
{
    public interface ICustomerService
    {
        Task<bool> IsCustomerValid(string customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly IMongoCollection<Customer> _customers;

        public CustomerService(IMongoClient client)
        {
            var database = client.GetDatabase(nameof(Databases.CarServiceDb));
            _customers = database.GetCollection<Customer>(nameof(Customer));
        }

        public async Task<bool> IsCustomerValid(string customerId)
        {
            return await _customers.Find(a => a.CustomerId.Equals(customerId)).AnyAsync();
        }
    }
}
