using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using RefactoringChallenge.Entities;

namespace RefactoringChallenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly NorthwindDbContext _northwindDbContext;
        private readonly IMapper _mapper;

        public OrdersController(NorthwindDbContext northwindDbContext, IMapper mapper)
        {
            _northwindDbContext = northwindDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int? skip = null, int? take = null)
        {
            var query = _northwindDbContext.Orders;
            if (skip != null)
            {
                query.Skip(skip.Value);
            }
            if (take != null)
            {
                query.Take(take.Value);
            }
            //var result = _mapper.From(query).ProjectToType<OrderResponse>().ToList();
            //return Json(result);

            var result = query.ProjectToType<OrderResponse>(_mapper.Config).ToList();
            //var result = query.Adapt<List<OrderResponse>>((TypeAdapterConfig)_mapper);
            return Ok(result);
        }


        [HttpGet("{orderId}")]
        public IActionResult GetById([FromRoute] int orderId)
        {
            //var result = _mapper.From(_northwindDbContext.Orders).ProjectToType<OrderResponse>().FirstOrDefault(o => o.OrderId == orderId);
            var result = _northwindDbContext.Orders
                .Where(o => o.OrderId == orderId)
                .ProjectToType<OrderResponse>(_mapper.Config)
                .FirstOrDefault();

            if (result == null)
                return NotFound();

            //return Json(result);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult Create(
            string customerId,
            int? employeeId,
            DateTime? requiredDate,
            int? shipVia,
            decimal? freight,
            string shipName,
            string shipAddress,
            string shipCity,
            string shipRegion,
            string shipPostalCode,
            string shipCountry,
            IEnumerable<OrderDetailRequest> orderDetails
            )
        {
            var newOrderDetails = new List<OrderDetail>();
            foreach (var orderDetail in orderDetails)
            {
                newOrderDetails.Add(new OrderDetail
                {
                    ProductId = orderDetail.ProductId,
                    Discount = orderDetail.Discount,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                });
            }

            var newOrder = new Order
            {
                CustomerId = customerId,
                EmployeeId = employeeId,
                OrderDate = DateTime.Now,
                RequiredDate = requiredDate,
                ShipVia = shipVia,
                Freight = freight,
                ShipName = shipName,
                ShipAddress = shipAddress,
                ShipCity = shipCity,
                ShipRegion = shipRegion,
                ShipPostalCode = shipPostalCode,
                ShipCountry = shipCountry,
                OrderDetails = newOrderDetails,
            };
            _northwindDbContext.Orders.Add(newOrder);
            _northwindDbContext.SaveChanges();

            //return Json(newOrder.Adapt<OrderResponse>());
            return Ok(newOrder.Adapt<OrderResponse>(_mapper.Config));
        }

        [HttpPost("{orderId}/[action]")]
        public IActionResult AddProductsToOrder([FromRoute] int orderId, IEnumerable<OrderDetailRequest> orderDetails)
        {
            var order = _northwindDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
                return NotFound();

            //var newOrderDetails = new List<OrderDetail>();
            //foreach (var orderDetail in orderDetails)
            //{
            //    newOrderDetails.Add(new OrderDetail
            //    {
            //        OrderId = orderId,
            //        ProductId = orderDetail.ProductId,
            //        Discount = orderDetail.Discount,
            //        Quantity = orderDetail.Quantity,
            //        UnitPrice = orderDetail.UnitPrice,
            //    });
            //}
            var newOrderDetails = orderDetails.Select(orderDetail => new OrderDetail
            {
                OrderId = orderId,
                ProductId = orderDetail.ProductId,
                Discount = orderDetail.Discount,
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
            }).ToList();

            _northwindDbContext.OrderDetails.AddRange(newOrderDetails);
            _northwindDbContext.SaveChanges();

            //return Json(newOrderDetails.Select(od => od.Adapt<OrderDetailResponse>()));
            return Ok(newOrderDetails.Select(od => od.Adapt<OrderDetailResponse>(_mapper.Config)));
        }

        [HttpPost("{orderId}/[action]")]
        public IActionResult Delete([FromRoute] int orderId)
        {
            var order = _northwindDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
                return NotFound();

            var orderDetails = _northwindDbContext.OrderDetails.Where(od => od.OrderId == orderId);

            _northwindDbContext.OrderDetails.RemoveRange(orderDetails);
            _northwindDbContext.Orders.Remove(order);
            _northwindDbContext.SaveChanges();

            return Ok();
        }
    }
}
