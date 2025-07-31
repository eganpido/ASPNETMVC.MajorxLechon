using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Diagnostics;
using System.Reflection;

namespace MajorxLechon.ModifiedApiControllers
{
    public class ApiTrnOrderController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

        // List Orders
        [Authorize, HttpGet, Route("api/order/list/{startDate}/{endDate}")]
        public List<Entities.TrnOrder> ListOrder(String startDate, String endDate)
        {
            var currentUser = from d in db.MstUsers
                              where d.UserId == User.Identity.GetUserId()
                              select d;

            var orders = from d in db.TrnOrders.OrderByDescending(d => d.Id)
                         where d.SalesDate >= Convert.ToDateTime(startDate)
                         && d.SalesDate <= Convert.ToDateTime(endDate)
                         select new Entities.TrnOrder
                         {
                             Id = d.Id,
                             OrderNumber = d.OrderNumber,
                             SalesDate = d.SalesDate.ToShortDateString(),
                             DeliveryDate = d.DeliveryDate.ToShortDateString(),
                             DeliveryTime = d.DeliveryTime,
                             CustomerName = d.CustomerName,
                             ContactNumber = d.ContactNumber,
                             Address = d.Address,
                             Landmark = d.Landmark,
                             LookFor = d.LookFor,
                             Amount = d.Amount,
                             IsLocked = d.IsLocked,
                             CreatedBy = d.MstUser.FullName,
                             CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                             UpdatedBy = d.MstUser1.FullName,
                             UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                         };

            return orders.ToList();
        }

        // Detail Order
        [Authorize, HttpGet, Route("api/order/detail/{id}")]
        public Entities.TrnOrder DetailOrder(String id)
        {
            var currentUser = from d in db.MstUsers
                              where d.UserId == User.Identity.GetUserId()
                              select d;

            var order = from d in db.TrnOrders.OrderByDescending(d => d.Id)
                        where d.Id == Convert.ToInt32(id)
                        select new Entities.TrnOrder
                        {
                            Id = d.Id,
                            OrderNumber = d.OrderNumber,
                            SalesDate = d.SalesDate.ToShortDateString(),
                            DeliveryDate = d.DeliveryDate.ToShortDateString(),
                            DeliveryTime = d.DeliveryTime,
                            CustomerName = d.CustomerName,
                            ContactNumber = d.ContactNumber,
                            Address = d.Address,
                            Landmark = d.Landmark,
                            LookFor = d.LookFor,
                            Amount = d.Amount,
                            IsLocked = d.IsLocked,
                            CreatedBy = d.MstUser.FullName,
                            CreatedDateTime = d.CreatedDateTime.ToShortDateString(),
                            UpdatedBy = d.MstUser1.FullName,
                            UpdatedDateTime = d.UpdatedDateTime.ToShortDateString()
                        };

            if (order.Any())
            {
                return order.FirstOrDefault();
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

        // Add Order
        [Authorize, HttpPost, Route("api/order/add")]
        public HttpResponseMessage AddOrder()
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var defaultOrderNumber = "000001";
                    var lastOrder = from d in db.TrnOrders.OrderByDescending(d => d.Id)
                                    select d;

                    if (lastOrder.Any())
                    {
                        var orderNumber = Convert.ToInt32(lastOrder.FirstOrDefault().OrderNumber) + 000001;
                        defaultOrderNumber = FillLeadingZeroes(orderNumber, 10);
                    }

                    Data.TrnOrder newOrder = new Data.TrnOrder
                    {
                        OrderNumber = defaultOrderNumber,
                        SalesDate = DateTime.Today,
                        DeliveryDate = DateTime.Today,
                        CustomerName = "NA",
                        ContactNumber = "NA",
                        Address = "NA",
                        Landmark = "NA",
                        LookFor = "NA",
                        Amount = 0,
                        IsLocked = false,
                        CreatedById = currentUserId,
                        CreatedDateTime = DateTime.Now,
                        UpdatedById = currentUserId,
                        UpdatedDateTime = DateTime.Now
                    };

                    db.TrnOrders.InsertOnSubmit(newOrder);
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, newOrder.Id);
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

        // Save Order
        [Authorize, HttpPut, Route("api/order/save/{id}")]
        public HttpResponseMessage SaveOrder(Entities.TrnOrder objOrder, String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.UserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var order = from d in db.TrnOrders where d.Id == Convert.ToInt32(id) select d;
                    if (order.Any())
                    {
                        if (!order.FirstOrDefault().IsLocked)
                        {
                            Decimal amount = 0;
                            var orderItems = from d in db.TrnOrderItems where d.OrderId == Convert.ToInt32(id) select d;
                            if (orderItems.Any()) { amount = orderItems.Sum(d => d.Amount); }

                            var saveOrder = order.FirstOrDefault();
                            saveOrder.SalesDate = Convert.ToDateTime(objOrder.SalesDate);
                            saveOrder.DeliveryDate = Convert.ToDateTime(objOrder.DeliveryDate);
                            saveOrder.DeliveryTime = objOrder.DeliveryTime;
                            saveOrder.CustomerName = objOrder.CustomerName;
                            saveOrder.ContactNumber = objOrder.ContactNumber;
                            saveOrder.Address = objOrder.Address;
                            saveOrder.Landmark = objOrder.Landmark;
                            saveOrder.LookFor = objOrder.LookFor;
                            saveOrder.Amount = amount;
                            saveOrder.UpdatedById = currentUserId;
                            saveOrder.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Saving Error. These order details are already locked.");
                        }
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

        // Lock Order
        [Authorize, HttpPut, Route("api/order/lock/{id}")]
        public HttpResponseMessage LockOrder(Entities.TrnOrder objOrder, String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var order = from d in db.TrnOrders
                                     where d.Id == Convert.ToInt32(id)
                                     select d;

                    if (order.Any())
                    {
                        if (!order.FirstOrDefault().IsLocked)
                        {
                            Decimal amount = 0;
                            var orderItems = from d in db.TrnOrderItems where d.OrderId == Convert.ToInt32(id) select d;
                            if (orderItems.Any()) { amount = orderItems.Sum(d => d.Amount); }

                            var lockOrder = order.FirstOrDefault();
                            lockOrder.SalesDate = Convert.ToDateTime(objOrder.SalesDate);
                            lockOrder.DeliveryDate = Convert.ToDateTime(objOrder.DeliveryDate);
                            lockOrder.DeliveryTime = objOrder.DeliveryTime;
                            lockOrder.CustomerName = objOrder.CustomerName;
                            lockOrder.ContactNumber = objOrder.ContactNumber;
                            lockOrder.Address = objOrder.Address;
                            lockOrder.Landmark = objOrder.Landmark;
                            lockOrder.LookFor = objOrder.LookFor;
                            lockOrder.Amount = amount;
                            lockOrder.UpdatedById = currentUserId;
                            lockOrder.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Locking Error. These order details are already locked.");
                        }
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

        // Unlock Order
        [Authorize, HttpPut, Route("api/order/unlock/{id}")]
        public HttpResponseMessage UnlockOrder(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var order = from d in db.TrnOrders
                                     where d.Id == Convert.ToInt32(id)
                                     select d;

                    if (order.Any())
                    {
                        if (order.FirstOrDefault().IsLocked)
                        {
                            var unlockOrder = order.FirstOrDefault();
                            unlockOrder.IsLocked = false;
                            unlockOrder.UpdatedById = currentUserId;
                            unlockOrder.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Unlocking Error. These order details are already unlocked.");
                        }
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

        // Delete Order
        [Authorize, HttpDelete, Route("api/order/delete/{id}")]
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

                    var order = from d in db.TrnOrders
                                     where d.Id == Convert.ToInt32(id)
                                     select d;

                    if (order.Any())
                    {
                        if (!order.FirstOrDefault().IsLocked)
                        {
                            db.TrnOrders.DeleteOnSubmit(order.First());
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Delete Error. You cannot delete order if the current sales order record is locked.");
                        }
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
