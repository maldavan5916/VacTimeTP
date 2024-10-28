using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Receipt
    {
        public int Id { get; set; }
        public int Materials_id { get; set; }
        public double Summ { get; set; }
        public DateTime Sate { get; set; }
        public int Count { get; set; }
        public int Counterparties_id { get; set; }
    }
}
