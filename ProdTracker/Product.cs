using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string SerialNo { get; set; }
        public int Units_id { get; set; }
        public int Locations_id { get; set; }
    }
}
