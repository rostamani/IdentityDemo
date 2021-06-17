using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitySample.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }

        public string City { get; set; }

        public enum gender
        {
            Male,Female
        }
        public gender Gender { get; set; }
    }
}
