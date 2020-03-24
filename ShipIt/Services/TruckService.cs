using System.Collections.Generic;
using ShipIt.Models.ApiModels;

namespace ShipIt.Services
{
    public interface ITruckService
    {
        int GetLoadedItemsCount();
        void RemoveItems(int warehouseId, List<StockAlteration> lineItems);
        void AddItems(int warehouseId, List<StockAlteration> lineItems);
    }

    public class TruckService
    {
        public Truck CreateTruckLoad(Product product, OrderLine orderLine)
        {
            //as product is added to the order, a new truck is created with that product registered to the truck list
            var truck = new Truck();
            truck.totalWeight = product.Weight;
            truck.gtin = orderLine.gtin;
            truck.quantity = orderLine.quantity;
            truck.orderLine = orderLine.ToString();
            return truck;
        }
    }
}