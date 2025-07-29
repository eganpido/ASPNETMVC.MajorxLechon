using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MajorxLechon.Entities
{
    public class MstItem
    {
        public Int32 Id { get; set; }
        public String ItemCode { get; set; }
        public String ItemDescription { get; set; }
        public String Cost { get; set; }
        public String Price { get; set; }
        public Boolean IsLocked { get; set; }
        public Int32 CreatedById { get; set; }
        public String CreatedBy { get; set; }
        public String CreatedDateTime { get; set; }
        public Int32 UpdatedById { get; set; }
        public String UpdatedBy { get; set; }
        public String UpdatedDateTime { get; set; }
    }
}