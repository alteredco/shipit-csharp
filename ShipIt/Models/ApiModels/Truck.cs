using System;

namespace ShipIt.Models.ApiModels
{
    public class Truck
    {
        public float totalWeight { get; set; }
        public String gtin { get; set; }
        public int quantity { get; set; }
        public string orderLine { get; set; }
    }
}