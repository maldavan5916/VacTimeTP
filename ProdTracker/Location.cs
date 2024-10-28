using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Location
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Employees_id { get; set; }
    }
}
