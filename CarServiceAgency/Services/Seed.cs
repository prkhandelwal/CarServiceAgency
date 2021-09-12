using CarServiceAgency.Enums;
using CarServiceAgency.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceAgency.Services
{
    public static class Seed
    {
        public static async Task InitializeAsync(IMongoClient client)
        {
            await client.DropDatabaseAsync(nameof(Databases.CarServiceDb));
            var database = client.GetDatabase(nameof(Databases.CarServiceDb));
            await database.CreateCollectionAsync(nameof(Appointment));
            await database.CreateCollectionAsync(nameof(Operator));
            await database.CreateCollectionAsync(nameof(Customer));

            var operatorCollection = database.GetCollection<Operator>(nameof(Operator));
            var customerCollection = database.GetCollection<Customer>(nameof(Customer));

            var operators = new List<Operator>() 
            {
                new Operator(Guid.NewGuid().ToString(), "Toby"),
                new Operator(Guid.NewGuid().ToString(), "Andrew"),
                new Operator(Guid.NewGuid().ToString(), "Tom"),
            };
            await operatorCollection.InsertManyAsync(operators);

            var customers = new List<Customer>()
            {
                new Customer(Guid.NewGuid().ToString(), "Emma"),
                new Customer(Guid.NewGuid().ToString(), "Zendaya"),
            };
            await customerCollection.InsertManyAsync(customers);
        }
    }
}
