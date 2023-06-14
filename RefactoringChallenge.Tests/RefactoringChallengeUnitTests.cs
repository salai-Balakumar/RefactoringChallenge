using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RefactoringChallenge.Controllers;
using RefactoringChallenge.Entities;

namespace RefactoringChallenge.Tests
{
    public class OrdersControllerTests
    {
        private readonly OrdersController _controller;
        private readonly Mock<NorthwindDbContext>? _dbContextMock;
        private readonly Mock<IMapper>? _mapperMock;        
        private readonly List<Order> orderlist;

        public OrdersControllerTests()
        {
            //_dbContextMock = new Mock<NorthwindDbContext>();
            //_mapperMock = new Mock<IMapper>();
            // Create a new instance of Mapper and configure it
            orderlist = new List<Order>
            {
                new Order{ OrderId=1,CustomerId="A1", EmployeeId=1,OrderDate=DateTime.Now,RequiredDate=null,ShippedDate=null,ShipVia=1,Freight=1, ShipName="DHL1", ShipAddress="22", ShipCity="BR",ShipPostalCode="B23",ShipRegion="North",ShipCountry="UK" },
                new Order{ OrderId=2,CustomerId="A2", EmployeeId=2,OrderDate=DateTime.Now,RequiredDate=null,ShippedDate=null,ShipVia=1,Freight=1, ShipName="DHL1", ShipAddress="32", ShipCity="BR",ShipPostalCode="B33",ShipRegion="East",ShipCountry="UK" },
                new Order{ OrderId=3,CustomerId="A3", EmployeeId=3,OrderDate=DateTime.Now,RequiredDate=null,ShippedDate=null,ShipVia=1,Freight=1, ShipName="DHL1", ShipAddress="42", ShipCity="BR",ShipPostalCode="B43",ShipRegion="West",ShipCountry="UK" },
                new Order{ OrderId=4,CustomerId="A4", EmployeeId=4,OrderDate=DateTime.Now,RequiredDate=null,ShippedDate=null,ShipVia=1,Freight=1, ShipName="DHL1", ShipAddress="52", ShipCity="BR",ShipPostalCode="B73",ShipRegion="South",ShipCountry="UK" },
            };

            var orderQuery = orderlist.AsQueryable();

            var dbSetMock = new Mock<DbSet<Order>>();
            dbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orderQuery.Provider);
            dbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orderQuery.Expression);
            dbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orderQuery.ElementType);
            dbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orderQuery.GetEnumerator());

            var _dbContextMock = new Mock<NorthwindDbContext>();
            _dbContextMock.Setup(db => db.Orders).Returns(dbSetMock.Object);

            var mapperConfig = new TypeAdapterConfig();
            mapperConfig.ForType<Order, OrderResponse>().Map(dest => dest.OrderId, src => src.OrderId);

            #region OrderDetail
            var orderDetailList = new List<OrderDetail>();
            
            var orderDetailQuery = orderDetailList.AsQueryable();

            var dbSetMock1 = new Mock<DbSet<OrderDetail>>();
            dbSetMock1.As<IQueryable<OrderDetail>>().Setup(m => m.Provider).Returns(orderDetailQuery.Provider);
            dbSetMock1.As<IQueryable<OrderDetail>>().Setup(m => m.Expression).Returns(orderDetailQuery.Expression);
            dbSetMock1.As<IQueryable<OrderDetail>>().Setup(m => m.ElementType).Returns(orderDetailQuery.ElementType);
            dbSetMock1.As<IQueryable<OrderDetail>>().Setup(m => m.GetEnumerator()).Returns(orderDetailQuery.GetEnumerator());

            _dbContextMock.Setup(db => db.OrderDetails).Returns(dbSetMock1.Object);
            
            #endregion

            // Create an IMapper instance using the configured Mapper
            var mapper = new Mapper(mapperConfig);
            _controller = new OrdersController(_dbContextMock.Object, mapper);
        }

        [Fact]
        public void Get_WithSkipAndTake_ReturnsOrders()
        {
            // Arrange
            var skip = 0;
            var take = 4;

            // Act
            var result = _controller.Get(skip, take);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orderResponses = Assert.IsAssignableFrom<IEnumerable<OrderResponse>>(okResult.Value);
            Assert.Equal(take, orderResponses.Count());
        }

        [Fact]
        public void GetById_WithExistingOrderId_ReturnsOrder()
        {
            // Arrange            
            var orderId = 1;
            var expectedOrder = orderlist.First(o => o.OrderId == orderId);
            
            // Act
            var result = _controller.GetById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orderResponse = Assert.IsType<OrderResponse>(okResult.Value);
            Assert.Equal(expected: expectedOrder.Adapt<OrderResponse>().OrderId,actual: orderResponse.OrderId);
        }

        [Fact]
        public void GetById_WithNonExistingOrderId_ReturnsNotFound()
        {
            // Arrange            
            var orderId = 999;            

            // Act
            var result = _controller.GetById(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }        

        [Fact]
        public void Create_ReturnsNewOrder()
        {
            // Arrange
            var orderDetails = new List<OrderDetailRequest> { new OrderDetailRequest { ProductId = 1, Quantity = 5 } };            

            // Act
            var result = _controller.Create("C001",1,null,null,null,"DHL1","122","BR","Northern East","LR1","UK",orderDetails);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orderResponse = Assert.IsType<OrderResponse>(okResult.Value);
            Assert.Equal("C001", orderResponse.CustomerId);
            Assert.Equal(1, orderResponse.EmployeeId);
            Assert.Single(orderResponse.OrderDetails);
        }

        [Fact]
        public void AddProductsToOrder_ReturnsNewOrderDetails()
        {
            // Arrange
            var orderId = 1;
            
            var orderDetails = new List<OrderDetailRequest> { new OrderDetailRequest { ProductId = 1, Quantity = 5 } };

            // Act
            var result = _controller.AddProductsToOrder(orderId, orderDetails);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orderDetailResponses = Assert.IsAssignableFrom<IEnumerable<OrderDetailResponse>>(okResult.Value);
            Assert.Single(orderDetailResponses);
        }

        [Fact]
        public void Delete_ReturnsOk_WhenOrderDeleted()
        {
            // Arrange
            var orderId = 1;            
            
            // Act
            var result = _controller.Delete(orderId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }    
}