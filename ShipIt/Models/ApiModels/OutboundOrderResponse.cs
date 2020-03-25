using System.Collections.Generic;
using System.Linq;

namespace ShipIt.Models.ApiModels
{
    public class OutboundOrderResponse
    {
        public IEnumerable<Truck> Trucks { get; set; }
        public double NumberOfTrucks => Trucks.Count();
    }
    
    public class Truck
    {
        public double TotalWeight => Batches.Sum(batch => batch.TotalWeight);
        public List<Batch> Batches { get; set; } = new List<Batch>();
    }

    public class Batch
    {
        public string Name { get; set; }
        public string Gtin { get; set; }
        public int Quantity { get; set; }
        public double ItemWeight { get; set; }
        public double TotalWeight => ItemWeight * Quantity;
    }
}