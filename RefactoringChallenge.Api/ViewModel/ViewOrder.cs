using System;
using System.Collections.Generic;

namespace RefactoringChallenge.ViewModel
{
    public partial class ViewOrder
    {
        public ViewOrder()
        {
            //OrderDetails = new HashSet<OrderDetail>();
        }
        public string CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? RequiredDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
    }
}
