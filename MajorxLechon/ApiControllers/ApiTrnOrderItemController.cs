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
    public class ApiTrnOrderItemController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

        // List Order Item
        [Authorize, HttpGet, Route("api/orderItem/list/{OrderId}")]
        public List<Entities.TrnOrderItem> ListOrderItem(String OrderId)
        {
            var orderItems = from d in db.TrnOrderItems
                                  where d.OrderId == Convert.ToInt32(OrderId)
                                  select new Entities.TrnOrderItem
                                  {
                                      Id = d.Id,
                                      OrderId = d.OrderId,
                                      ItemId = d.ItemId,
                                      ItemDescription = d.MstItem.ItemDescription,
                                      Price = d.Price,
                                      Quantity = d.Quantity,
                                      Amount = d.Amount
                                  };

            return orderItems.ToList();
        }

        // Dropdown List Item
        [Authorize, HttpGet, Route("api/orderItem/dropdown/list/item")]
        public List<Entities.MstItem> DropdownListItem()
        {
            var items = from d in db.MstItems.OrderBy(d => d.ItemDescription)
                        where d.IsLocked == true
                        select new Entities.MstItem
                        {
                            Id = d.Id,
                            ItemDescription = d.ItemDescription
                        };

            return items.ToList();
        }

        // Add Order Item
        [Authorize, HttpPost, Route("api/orderItem/add/{OrderId}")]
        public HttpResponseMessage AddOrderItem(Entities.TrnOrderItem objOrderItem, String OrderId)
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
                                     where d.Id == Convert.ToInt32(OrderId)
                                     select d;

                    if (order.Any())
                    {
                        if (!order.FirstOrDefault().IsLocked)
                        {
                            var item = from d in db.MstItems
                                       where d.Id == objOrderItem.ItemId
                                       && d.IsLocked == true
                                       select d;

                            if (item.Any())
                            {
                                Data.TrnOrderItem newOrderItem = new Data.TrnOrderItem
                                {
                                    OrderId = Convert.ToInt32(OrderId),
                                    ItemId = objOrderItem.ItemId,
                                    Quantity = objOrderItem.Quantity,
                                    Price = objOrderItem.Price,
                                    Amount = objOrderItem.Amount,
                                };

                                db.TrnOrderItems.InsertOnSubmit(newOrderItem);
                                db.SubmitChanges();

                                Decimal orderItemTotalAmount = 0;
                                if (order.FirstOrDefault().TrnOrderItems.Any())
                                {
                                    orderItemTotalAmount = order.FirstOrDefault().TrnOrderItems.Sum(d => d.Amount);
                                }

                                var updateOrderAmount = order.FirstOrDefault();
                                updateOrderAmount.Amount = orderItemTotalAmount;
                                db.SubmitChanges();

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "The selected item was not found in the server.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "You cannot add new order item if the current sales order detail is locked.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "These current order details are not found in the server. Please add new sales order first before proceeding.");
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

        // Update Order Item
        [Authorize, HttpPut, Route("api/orderItem/update/{id}/{OrderId}")]
        public HttpResponseMessage UpdateOrderItem(Entities.TrnOrderItem objOrderItem, String id, String OrderId)
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
                                     where d.Id == Convert.ToInt32(OrderId)
                                     select d;

                    if (order.Any())
                    {
                        if (!order.FirstOrDefault().IsLocked)
                        {
                            var orderItem = from d in db.TrnOrderItems
                                                 where d.Id == Convert.ToInt32(id)
                                                 select d;

                            if (orderItem.Any())
                            {
                                var item = from d in db.MstItems
                                           where d.Id == objOrderItem.ItemId
                                           && d.IsLocked == true
                                           select d;

                                if (item.Any())
                                {
                                    var updateOrderItem = orderItem.FirstOrDefault();
                                    updateOrderItem.OrderId = Convert.ToInt32(OrderId);
                                    updateOrderItem.ItemId = objOrderItem.ItemId;
                                    updateOrderItem.Quantity = objOrderItem.Quantity;
                                    updateOrderItem.Price = objOrderItem.Price;
                                    updateOrderItem.Amount = objOrderItem.Amount;
                                    db.SubmitChanges();

                                    Decimal orderItemTotalAmount = 0;

                                    if (order.FirstOrDefault().TrnOrderItems.Any())
                                    {
                                        orderItemTotalAmount = order.FirstOrDefault().TrnOrderItems.Sum(d => d.Amount);
                                    }

                                    var updateOrderAmount = order.FirstOrDefault();
                                    updateOrderAmount.Amount =orderItemTotalAmount;
                                    db.SubmitChanges();

                                    return Request.CreateResponse(HttpStatusCode.OK);
                                }
                                else
                                {
                                    return Request.CreateResponse(HttpStatusCode.BadRequest, "The selected item was not found in the server.");
                                }
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound, "This sales order item detail is no longer exist in the server.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "You cannot edit and update sales order item if the current sales order detail is locked.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "These current sales order details are not found in the server. Please add new sales order first before proceeding.");
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

        // Delete Order Item
        [Authorize, HttpDelete, Route("api/orderItem/delete/{id}/{OrderId}")]
        public HttpResponseMessage DeleteOrderItem(String id, String OrderId)
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
                                     where d.Id == Convert.ToInt32(OrderId)
                                     select d;

                    if (order.Any())
                    {
                        if (!order.FirstOrDefault().IsLocked)
                        {
                            var orderItem = from d in db.TrnOrderItems
                                                 where d.Id == Convert.ToInt32(id)
                                                 select d;

                            if (orderItem.Any())
                            {
                                db.TrnOrderItems.DeleteOnSubmit(orderItem.First());
                                db.SubmitChanges();

                                Decimal orderItemTotalAmount = 0;

                                if (order.FirstOrDefault().TrnOrderItems.Any())
                                {
                                    orderItemTotalAmount = order.FirstOrDefault().TrnOrderItems.Sum(d => d.Amount);
                                }

                                var updateOrderAmount = order.FirstOrDefault();
                                updateOrderAmount.Amount = orderItemTotalAmount;
                                db.SubmitChanges();

                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.NotFound, "This sales order item detail is no longer exist in the server.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "You cannot delete sales order item if the current sales order detail is locked.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "These current sales order details are not found in the server. Please add new sales order first before proceeding.");
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
