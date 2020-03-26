using System;
using System.Collections.Generic;
using System.Linq;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Repositories;

namespace ShipIt.Services
{
    public interface ITruckService
    {
        IEnumerable<Truck> CreateTruckLoad(List<StockAlteration> lineItems);
    }

    public class TruckService : ITruckService
    {
        private readonly IProductRepository _productRepository;
        private const double MaxTruckWeight = 2000;

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
                var maxItemsPerTruck = GetMaxItemsPerTruck(product);
                var quantityRemaining = item.Quantity;

                while (quantityRemaining > 0)
                {
                    var quantityAdded = (int)Math.Min((decimal) quantityRemaining, maxItemsPerTruck);
                    var batch = new Batch
                    {
                        Name = product.Name,
                        Gtin = product.Gtin,
                        Quantity = quantityAdded,
                        ItemWeight = product.Weight
                    };
                    quantityRemaining -= quantityAdded;

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
            }

            return truckList;
        }
        
        private Truck GetOpenTruck(List<Truck> truckList, Batch batch)
        {
            var openTruck = truckList.FirstOrDefault(truck => truck.TotalWeight + batch.TotalWeight <= MaxTruckWeight);

            return openTruck;
        }
        

        private int GetMaxItemsPerTruck(ProductDataModel product)
        {
            return (int) Math.Floor(MaxTruckWeight / product.Weight);
        }
    }
}