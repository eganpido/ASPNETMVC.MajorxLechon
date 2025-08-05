using Microsoft.AspNet.Identity;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace MajorxLechon.ModifiedApiControllers
{
    public class ApiTrnReservationController : ApiController
    {
        // Data Context
        private Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

        // List Reservations
        [Authorize, HttpGet, Route("api/reservation/list/{startDate}/{endDate}")]
        public List<Entities.TrnReservation> ListReservation(String startDate, String endDate)
        {
            var currentUser = from d in db.MstUsers
                              where d.UserId == User.Identity.GetUserId()
                              select d;

            var reservations = from d in db.TrnReservations.OrderByDescending(d => d.Id)
                         where d.ReservedDate >= Convert.ToDateTime(startDate)
                         && d.ReservedDate <= Convert.ToDateTime(endDate)
                         select new Entities.TrnReservation
                         {
                             Id = d.Id,
                             ReservationNumber = d.ReservationNumber,
                             ReservedDate = d.ReservedDate.ToShortDateString(),
                             DeliveryDate = d.DeliveryDate.ToShortDateString(),
                             DeliveryTime = d.DeliveryTime,
                             CustomerName = d.CustomerName,
                             ItemOrder = d.ItemOrder,
                             ContactNumber = d.ContactNumber,
                             Address = d.Address,
                             Landmark = d.Landmark,
                             LookFor = d.LookFor
                         };

            return reservations.ToList();
        }

        // Detail Reservation
        [Authorize, HttpGet, Route("api/reservation/detail/{id}")]
        public Entities.TrnReservation DetailReservation(String id)
        {
            var currentUser = from d in db.MstUsers
                              where d.UserId == User.Identity.GetUserId()
                              select d;

            var reservation = from d in db.TrnReservations.OrderByDescending(d => d.Id)
                        where d.Id == Convert.ToInt32(id)
                        select new Entities.TrnReservation
                        {
                            Id = d.Id,
                            ReservationNumber = d.ReservationNumber,
                            ReservedDate = d.ReservedDate.ToShortDateString(),
                            DeliveryDate = d.DeliveryDate.ToShortDateString(),
                            DeliveryTime = d.DeliveryTime,
                            CustomerName = d.CustomerName,
                            ItemOrder = d.ItemOrder,
                            ContactNumber = d.ContactNumber,
                            Address = d.Address,
                            Landmark = d.Landmark,
                            LookFor = d.LookFor
                        };

            if (reservation.Any())
            {
                return reservation.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        // ===================
        // Fill Leading Zeroes
        // ===================
        public String FillLeadingZeroes(Int32 number, Int32 length)
        {
            var result = number.ToString();
            var pad = length - result.Length;
            while (pad > 0)
            {
                result = '0' + result;
                pad--;
            }

            return result;
        }

        // Add Reservation
        [Authorize, HttpPost, Route("api/reservation/add")]
        public HttpResponseMessage AddReservation(Entities.TrnReservation objReservation)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var defaultResNumber = "000001";
                    var lastReserved = from d in db.TrnReservations.OrderByDescending(d => d.Id)
                                    select d;

                    if (lastReserved.Any())
                    {
                        var resNumber = Convert.ToInt32(lastReserved.FirstOrDefault().ReservationNumber) + 000001;
                        defaultResNumber = FillLeadingZeroes(resNumber, 6);
                    }

                    Data.TrnReservation newReservation = new Data.TrnReservation
                    {
                        ReservationNumber = defaultResNumber,
                        ReservedDate = DateTime.Today,
                        DeliveryDate = Convert.ToDateTime(objReservation.DeliveryDate),
                        DeliveryTime = objReservation.DeliveryTime,
                        CustomerName = objReservation.CustomerName,
                        ItemOrder = objReservation.ItemOrder,
                        ContactNumber = objReservation.ContactNumber,
                        Address = objReservation.Address,
                        Landmark = objReservation.Landmark,
                        LookFor = objReservation.LookFor
                    };

                    db.TrnReservations.InsertOnSubmit(newReservation);
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Theres no current user logged in.");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something's went wrong from the server.");
            }
        }

        // Delete Reservations
        [Authorize, HttpDelete, Route("api/reservation/delete/{id}")]
        public HttpResponseMessage DeleteOrder(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var reservation = from d in db.TrnReservations
                                     where d.Id == Convert.ToInt32(id)
                                     select d;

                    if (reservation.Any())
                    {
                        db.TrnReservations.DeleteOnSubmit(reservation.First());
                        db.SubmitChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Data not found. These order details are not found in the server.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Theres no current user logged in.");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something's went wrong from the server.");
            }
        }
    }
}
