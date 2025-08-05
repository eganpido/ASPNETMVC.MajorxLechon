using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MajorxLechon.Entities
{
    public class TrnReservation
    {
        public Int32 Id { get; set; }
        public String ReservationNumber { get; set; }
        public String ReservedDate { get; set; }
        public String DeliveryDate { get; set; }
        public String DeliveryTime { get; set; }
        public String CustomerName { get; set; }
        public String ItemOrder { get; set; }
        public String ContactNumber { get; set; }
        public String Address { get; set; }
        public String Landmark { get; set; }
        public String LookFor { get; set; }
    }
}