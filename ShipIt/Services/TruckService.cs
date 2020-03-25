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
            
            var truckBatch = GetFilledBatches(batches);
            
            return new List<Truck>
            {
                new Truck
                {
                    Batches = truckBatch
                }
            };
        }

        private List<Batch> GetFilledBatches(List<Batch> batchList)
        {
            var openBatches = new List<Batch>();
            var openBatchesWeight = openBatches.Sum(batch => batch.TotalWeight);

            foreach (var batch in batchList)
            {
                if (batch.TotalWeight < 2000 && openBatchesWeight <= 2000)
                {
                    openBatches.Add(batch);
                }
                //need something for overflow here
            }
            return openBatches;
        }
    }
}