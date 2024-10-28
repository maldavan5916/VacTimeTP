using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class Counterpartie
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string LegalAddress { get; set; }
        public required string PhoneNomber { get; set; }
        public required string PostalAddress { get; set; }
        public required string Unp { get; set; }
        public string Type { get; set; } = "fiz";
        public string? Okulp { get; set; }
        public string? Okpo { get; set; }
        public string? Oked { get; set; }
    }
}
