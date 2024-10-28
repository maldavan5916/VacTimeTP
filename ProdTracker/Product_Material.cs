using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Product_Material
    {
        public int Id { get; set; }
        public int Products_id { get; set; }
        public int Materials_id { get; set; }
        public int Quantity { get; set; }
    }
}
