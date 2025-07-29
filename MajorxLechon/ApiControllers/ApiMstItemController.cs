using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.UI;
using System.Web.Util;
using static iTextSharp.text.pdf.AcroFields;

namespace MajorxLechon.ApiControllers
{
    public class ApiMstItemController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

        // List Items
        [Authorize, HttpGet, Route("api/item/list")]
        public List<Entities.MstItem> ListItem()
        {
            var rawItems = from d in db.MstItems.OrderByDescending(d => d.ItemCode)
                           select new
                           {
                               Id = d.Id,
                               ItemCode = d.ItemCode,
                               ItemDescription = d.ItemDescription,
                               Cost = d.Cost, // Keep as decimal
                               Price = d.Price, // Keep as decimal
                               IsLocked = d.IsLocked,
                               CreatedById = d.CreatedById,
                               CreatedBy = d.MstUser.FullName,
                               CreatedDateTime = d.CreatedDateTime, // Keep as DateTime
                               UpdatedById = d.UpdatedById,
                               UpdatedBy = d.MstUser1.FullName,
                               UpdatedDateTime = d.UpdatedDateTime, // Keep as DateTime
                           };

            // Step 2: Materialize and format in memory
            var items = rawItems.ToList() // Execute query on database
                                      .Select(d => new Entities.MstItem
                                      {
                                          Id = d.Id,
                                          ItemCode = d.ItemCode,
                                          ItemDescription = d.ItemDescription,
                                          Cost = d.Cost.ToString("#,##0.00"), // Format in memory
                                          Price = d.Price.ToString("#,##0.00"), // Format in memory
                                          IsLocked = d.IsLocked,
                                          CreatedById = d.CreatedById,
                                          CreatedBy = d.CreatedBy,
                                          CreatedDateTime = d.CreatedDateTime.ToShortDateString(), // Format in memory
                                          UpdatedById = d.UpdatedById,
                                          UpdatedBy = d.UpdatedBy,
                                          UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(), // Format in memory
                                      })
                                      .ToList();

            return items;
        }

        // Detail Item
        [Authorize, HttpGet, Route("api/item/detail/{id}")]
        public Entities.MstItem DetailItem(String id)
        {
            var item = (from d in db.MstItems
                        where d.Id == Convert.ToInt32(id)
                        select new
                        {
                            d.Id,
                            d.ItemCode,
                            d.ItemDescription,
                            d.Cost,
                            d.Price,
                            d.IsLocked,
                            d.CreatedById,
                            CreatedBy = d.MstUser != null ? d.MstUser.FullName : "",
                            d.CreatedDateTime,
                            d.UpdatedById,
                            UpdatedBy = d.MstUser1 != null ? d.MstUser1.FullName : "",
                            d.UpdatedDateTime
                        }).FirstOrDefault();

            if (item != null)
            {
                return new Entities.MstItem
                {
                    Id = item.Id,
                    ItemCode = item.ItemCode,
                    ItemDescription = item.ItemDescription,
                    Cost = item.Cost.ToString("#,##0.00"),
                    Price = item.Price.ToString("#,##0.00"),
                    IsLocked = item.IsLocked,
                    CreatedById = item.CreatedById,
                    CreatedBy = item.CreatedBy,
                    CreatedDateTime = item.CreatedDateTime.ToShortDateString(),
                    UpdatedById = item.UpdatedById,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDateTime = item.UpdatedDateTime.ToShortDateString()
                };
            }

            return null;

        }

        // Fill Leading Zeroes
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

        // Add Item
        [Authorize, HttpPost, Route("api/item/add")]
        public HttpResponseMessage AddProduct()
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var defaultItemCode = "001";
                    var lastItem = from d in db.MstItems.OrderByDescending(d => d.Id)
                                   select d;

                    if (lastItem.Any())
                    {
                        var itemCode = Convert.ToInt32(lastItem.FirstOrDefault().ItemCode) + 001;
                        defaultItemCode = FillLeadingZeroes(itemCode, 3);
                    }

                    Data.MstItem newItem = new Data.MstItem
                    {
                        ItemCode = defaultItemCode,
                        ItemDescription = "NA",
                        Cost = 0,
                        Price = 0,
                        IsLocked = false,
                        CreatedById = currentUserId,
                        CreatedDateTime = DateTime.Now,
                        UpdatedById = currentUserId,
                        UpdatedDateTime = DateTime.Now,
                    };

                    db.MstItems.InsertOnSubmit(newItem);
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, newItem.Id);
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

        // Save Item
        [Authorize, HttpPut, Route("api/item/save/{id}")]
        public HttpResponseMessage SaveItem(Entities.MstItem objItem, String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var item = from d in db.MstItems
                               where d.Id == Convert.ToInt32(id)
                               select d;

                    if (item.Any())
                    {
                        if (!item.FirstOrDefault().IsLocked)
                        {
                            var saveItem = item.FirstOrDefault();
                            saveItem.ItemCode = objItem.ItemCode;
                            saveItem.ItemDescription = objItem.ItemDescription;
                            saveItem.Price = Convert.ToDecimal(objItem.Price);
                            saveItem.Cost = Convert.ToDecimal(objItem.Cost);
                            saveItem.UpdatedById = currentUserId;
                            saveItem.UpdatedDateTime = DateTime.Now;
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Saving Error. These details are already locked.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Data not found. These details are not found in the server.");
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

        // Lock Item
        [Authorize, HttpPut, Route("api/item/lock/{id}")]
        public HttpResponseMessage LockItem(Entities.MstItem objItem, String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var item = from d in db.MstItems
                               where d.Id == Convert.ToInt32(id)
                               select d;

                    if (item.Any())
                    {
                        if (!item.FirstOrDefault().IsLocked)
                        {
                            var itemByCode = from d in db.MstItems
                                             where d.ItemCode.Equals(objItem.ItemCode)
                                             && d.IsLocked == true
                                             select d;

                            if (!itemByCode.Any())
                            {
                                var lockItem = item.FirstOrDefault();
                                lockItem.ItemCode = objItem.ItemCode;
                                lockItem.ItemDescription = objItem.ItemDescription;
                                lockItem.Cost = Convert.ToDecimal(objItem.Cost);
                                lockItem.Price = Convert.ToDecimal(objItem.Price);
                                lockItem.IsLocked = true;
                                lockItem.UpdatedById = currentUserId;
                                lockItem.UpdatedDateTime = DateTime.Now;

                                db.SubmitChanges();


                                return Request.CreateResponse(HttpStatusCode.OK);
                            }
                            else
                            {
                                return Request.CreateResponse(HttpStatusCode.BadRequest, "Product Code is already taken.");
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Locking Error. These details are already locked.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Data not found. These details are not found in the server.");
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

        // Unlock Item
        [Authorize, HttpPut, Route("api/item/unlock/{id}")]
        public HttpResponseMessage UnlockItem(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var item = from d in db.MstItems
                               where d.Id == Convert.ToInt32(id)
                               select d;

                    if (item.Any())
                    {
                        if (item.FirstOrDefault().IsLocked)
                        {
                            var unlockItem = item.FirstOrDefault();
                            unlockItem.IsLocked = false;
                            unlockItem.UpdatedById = currentUserId;
                            unlockItem.UpdatedDateTime = DateTime.Now;

                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Unlocking Error. These details are already unlocked.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Data not found. These details are not found in the server.");
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

        // Delete Item
        [Authorize, HttpDelete, Route("api/item/delete/{id}")]
        public HttpResponseMessage DeleteItem(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var item = from d in db.MstItems
                               where d.Id == Convert.ToInt32(id)
                               select d;

                    if (item.Any())
                    {
                        db.MstItems.DeleteOnSubmit(item.First());

                        db.SubmitChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Data not found. This selected record is not found in the server.");
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
