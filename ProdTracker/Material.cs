using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Material
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Units_id { get; set; }
        public int Locations_id { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
