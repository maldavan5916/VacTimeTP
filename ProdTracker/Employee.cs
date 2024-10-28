using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Employee
    {
        public int Id { get; set; }
        public required string Fio { get; set; }
        public int Divisions_id { get; set; }
        public int Posts_id { get; set; }
        public DateTime DateHire { get; set; }
        public DateTime DateDismissal { get; set; }
    }
}
