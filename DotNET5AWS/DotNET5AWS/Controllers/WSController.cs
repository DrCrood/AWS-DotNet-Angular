using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using DotNet5_Angular_AWS.Model;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DotNET5AWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WSController : ControllerBase
    {
        // GET: api/<WSController>
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return new Product[]
            {
                new Product(){ Id = 1, Name = "Bike", Inventory = 20, Price = 199},
                new Product(){ Id = 2, Name = "TV", Inventory = 10, Price = 599},
                new Product(){ Id = 3, Name = "PC", Inventory = 30, Price = 899},
                new Product(){ Id = 4, Name = "Laptop", Inventory = 25, Price = 1299},
                new Product(){ Id = 5, Name = "Monitor", Inventory = 15, Price = 399},
                new Product(){ Id = 6, Name = "Car", Inventory = 15, Price = 25999},
                new Product(){ Id = 7, Name = "Truck", Inventory = 25, Price = 35999},
                new Product(){ Id = 8, Name = "SUV", Inventory = 35, Price = 45999},
                new Product(){ Id = 9, Name = "Big Truck", Inventory = 5, Price = 65999},
                new Product(){ Id = 10, Name = "Large SUV", Inventory = 18, Price = 75999},
            };
        }

        // GET api/<WSController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WSController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            string message = value;
        }

        // PUT api/<WSController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WSController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
