using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class OrdersController : ControllerBase //Used ControllerBase class specifically designed for API controllers in ASP.NET Core
    {
        private readonly NorthwindDbContext _northwindDbContext;
        private readonly IMapper _mapper;

        public OrdersController(NorthwindDbContext northwindDbContext, IMapper mapper)
        {
            _northwindDbContext = northwindDbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Method to Retrieve list of orders
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
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
            // Used _mapper.Config when calling ProjectToType(), To ensures consistency and clarity in the mapping process.
            var result = query.ProjectToType<OrderResponse>(_mapper.Config).ToList();
            //Ok() is more appropriate and clearer for indicating a successful response.
            return Ok(result);
        }

        /// <summary>
        /// Method to retrieve order based on Id
        /// </summary>
        /// <param name="orderId">Input param Orderid</param>
        /// <returns></returns>
        [HttpGet("{orderId}")]
        public IActionResult GetById([FromRoute] int orderId)
        {
            // LINQ query along with Where() method is used to filter the orders based on the orderId directly to improve efficiency
            var result = _northwindDbContext.Orders
                .Where(o => o.OrderId == orderId)
                .ProjectToType<OrderResponse>(_mapper.Config)
                .FirstOrDefault();

            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        /// <summary>
        /// Method to create Order
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="employeeId"></param>
        /// <param name="requiredDate"></param>
        /// <param name="shipVia"></param>
        /// <param name="freight"></param>
        /// <param name="shipName"></param>
        /// <param name="shipAddress"></param>
        /// <param name="shipCity"></param>
        /// <param name="shipRegion"></param>
        /// <param name="shipPostalCode"></param>
        /// <param name="shipCountry"></param>
        /// <param name="orderDetails"></param>
        /// <returns></returns>
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
            //Here there is a possibility to create another model class combining all the input parameters for this method but left it for now.
            var newOrderDetails = orderDetails.Select(orderDetail => new OrderDetail
            {
                ProductId = orderDetail.ProductId,
                Discount = orderDetail.Discount,
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
            }).ToList();            

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

            return Ok(newOrder.Adapt<OrderResponse>(_mapper.Config));
        }

        /// <summary>
        /// Method to Add products to an existing order
        /// </summary>
        /// <param name="orderId">Existing Order Id</param>
        /// <param name="orderDetails">New Order details</param>
        /// <returns></returns>
        [HttpPost("{orderId}/[action]")]
        public IActionResult AddProductsToOrder([FromRoute] int orderId, IEnumerable<OrderDetailRequest> orderDetails)
        {
            //Find the order from the DB using OrderId
            var order = _northwindDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
                return NotFound();
            //Get the new order details and create the new list
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
                        
            return Ok(newOrderDetails.Select(od => od.Adapt<OrderDetailResponse>(_mapper.Config)));
        }

        /// <summary>
        /// Method to delete the order based on OrderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost("{orderId}/[action]")]
        public IActionResult Delete([FromRoute] int orderId)
        {
            var order = _northwindDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
                return NotFound();
            //Filter the order based on input orderid
            var orderDetails = _northwindDbContext.OrderDetails.Where(od => od.OrderId == orderId);

            _northwindDbContext.OrderDetails.RemoveRange(orderDetails);
            _northwindDbContext.Orders.Remove(order);
            _northwindDbContext.SaveChanges();

            return Ok();
        }
    }
}
