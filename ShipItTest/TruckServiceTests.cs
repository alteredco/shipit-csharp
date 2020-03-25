using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using ShipIt.Repositories;
using ShipIt.Services;

namespace ShipItTest
{
    public class TruckServiceTests
    {
        private TruckService _truckService;
        private IProductRepository _productRepository;
        
        private readonly ProductDataModel TestProduct1 = new ProductDataModel
        {
            Id=17,
            Weight = 100,
            Name = "Test Item",
            Gtin = "Test Id"
        };

        private readonly ProductDataModel TestProduct2 = new ProductDataModel
        {
            Id = 9,
            Weight = 300,
            Name = "Test Item2",
            Gtin = "Test Id2"
        };

        [SetUp]

        public void SetUp()
        {
            _productRepository = A.Fake<IProductRepository>();
            A.CallTo(() => _productRepository.GetProductById(17)).Returns(TestProduct1);
            A.CallTo(() => _productRepository.GetProductById(9)).Returns(TestProduct2);
            _truckService = new TruckService(_productRepository);
        }

        [Test]

        public void SmallOrderGetsPlacesOnSingleTruck()
        {
            var lineItems = new List<StockAlteration>
            {
                new StockAlteration(17, 3)
            };

            var trucks = _truckService.CreateTruckLoad(lineItems);
            var truckList = trucks.ToList();
            
            Assert.AreEqual(truckList.Count, 1);
            Assert.AreEqual(truckList[0].TotalWeight, 300);
            Assert.AreEqual(truckList[0].Batches.Count, 1);
            Assert.AreEqual(truckList[0].Batches[0].Name, "Test Item");
        }
        
        [Test]
        public void MultipleOrdersGetsPlacedOnSingleTruck()
        {
            var lineItems = new List<StockAlteration>
            {
                new StockAlteration(17, 3),
                new StockAlteration(9, 3)
            };

            var trucks = _truckService.CreateTruckLoad(lineItems);
            var truckList = trucks.ToList();
            
            Assert.AreEqual(truckList.Count, 1);
            Assert.AreEqual(truckList[0].TotalWeight, 1200);
            Assert.AreEqual(truckList[0].Batches.Count, 2);
            Assert.AreEqual(truckList[0].Batches[0].Name, "Test Item");
            Assert.AreEqual(truckList[0].Batches[1].Name, "Test Item2");
        }

    }
}