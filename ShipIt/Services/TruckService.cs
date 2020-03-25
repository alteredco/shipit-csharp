using System.Collections.Generic;
using System.Linq;
using ShipIt.Models.ApiModels;
using ShipIt.Repositories;

namespace ShipIt.Services
{
    public interface ITruckService
    { 
       IEnumerable<Truck> CreateTruckLoad(List<StockAlteration> lineItems);
    }

    public class TruckService: ITruckService
    {
        private readonly IProductRepository _productRepository;

        public TruckService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public IEnumerable<Truck> CreateTruckLoad(List<StockAlteration> lineItems)
        {
            var truckList = new List<Truck>
            {
                new Truck()
            };

            foreach (var item in lineItems)
            {
                var product = _productRepository.GetProductById(item.ProductId);
                var batch = new Batch
                {
                    Name = product.Name,
                    Gtin = product.Gtin,
                    Quantity = item.Quantity,
                    ItemWeight = product.Weight
                };
                var openTruck = GetOpenTruck(truckList, batch);

                if (openTruck != null)
                {
                    openTruck.Batches.Add(batch);
                }
                else
                {
                    var newTruck = new Truck();
                    newTruck.Batches.Add(batch);
                    truckList.Add(newTruck);
                }
            }

            return truckList;
        }

        private Truck GetOpenTruck(List<Truck> truckList, Batch batch)
        {
            var maxCapacity = 2000;
            var openTruck = truckList.FirstOrDefault(truck => truck.TotalWeight + batch.TotalWeight < maxCapacity);

            return openTruck;
        }
    }
}