using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Contract
    {
        public int Id { get; set; }
        public int Counterparties_id { get; set; }
        public DateTime Date { get; set; }
        public double Summ { get; set; }
        public int Products_id { get; set; }
        public int Count { get; set; }
    }
}
