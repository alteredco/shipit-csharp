using System.Collections.Generic;
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
            var batches = new List<Batch>();

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
                batches.Add(batch);
            }
            
            return new List<Truck>
            {
                new Truck
                {
                    Batches =  batches
                }
            };
        }
    }
}