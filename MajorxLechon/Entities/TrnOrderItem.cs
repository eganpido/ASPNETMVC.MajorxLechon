using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MajorxLechon.Entities
{
    public class TrnOrderItem
    {
        public Int32 Id { get; set; }
        public Int32 OrderId { get; set; }
        public Int32 ItemId { get; set; }
        public String ItemDescription { get; set; }
        public Decimal Price { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal Amount { get; set; }
    }
}