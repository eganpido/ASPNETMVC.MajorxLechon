using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MajorxLechon.Entities
{
    public class TrnOrder
    {
        public Int32 Id { get; set; }
        public String OrderNumber { get; set; }
        public String SalesDate { get; set; }
        public String DeliveryDate { get; set; }
        public String DeliveryTime { get; set; }
        public String CustomerName { get; set; }
        public Decimal Amount { get; set; }
        public String ContactNumber { get; set; }
        public String Address { get; set; }
        public String Landmark { get; set; }
        public String LookFor { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedById { get; set; }
        public String CreatedBy { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedById { get; set; }
        public String UpdatedBy { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}