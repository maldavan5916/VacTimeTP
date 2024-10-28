using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Sale
    {
        public int Id { get; set; }
        public int Products_id { get; set; }
        public int Contracts_id { get; set; }
        public double Summ { get; set; }
        public DateTime Date { get; set; }
    }
}
