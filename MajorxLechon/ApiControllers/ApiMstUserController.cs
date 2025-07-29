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
    public class ApiMstUserController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

        // List Users
        [Authorize, HttpGet, Route("api/user/list")]
        public List<Entities.MstUser> ListUser()
        {
            var currentUser = from d in db.MstUsers
                              where d.UserId == User.Identity.GetUserId()
                              select d;

            var rawUsers = from d in db.MstUsers.OrderByDescending(d => d.Id)
                           select new
                           {
                               Id = d.Id,
                               UserId = d.UserId,
                               UserName = d.UserName,
                               Password = d.Password,
                               FullName = d.FullName,
                               IsLocked = d.IsLocked,
                               CreatedById = d.CreatedById,
                               CreatedBy = GetCreatedBy(d.CreatedById),
                               CreatedDateTime = d.CreatedDateTime, // Keep as DateTime
                               UpdatedById = d.UpdatedById,
                               UpdatedBy = GetUpdatedBy(d.UpdatedById),
                               UpdatedDateTime = d.UpdatedDateTime, // Keep as DateTime
                           };
            if (currentUser.FirstOrDefault().Id != 1)
            {
                rawUsers = from d in db.MstUsers.OrderByDescending(d => d.Id)
                           where d.Id != 1
                           select new
                           {
                               Id = d.Id,
                               UserId = d.UserId,
                               UserName = d.UserName,
                               Password = d.Password,
                               FullName = d.FullName,
                               IsLocked = d.IsLocked,
                               CreatedById = d.CreatedById,
                               CreatedBy = GetCreatedBy(d.CreatedById),
                               CreatedDateTime = d.CreatedDateTime, // Keep as DateTime
                               UpdatedById = d.UpdatedById,
                               UpdatedBy = GetUpdatedBy(d.UpdatedById),
                               UpdatedDateTime = d.UpdatedDateTime, // Keep as DateTime
                           };
            }

                // Step 2: Materialize and format in memory
                var users = rawUsers.ToList() // Execute query on database
                                      .Select(d => new Entities.MstUser
                                      {
                                          Id = d.Id,
                                          UserId = d.UserId,
                                          UserName = d.UserName,
                                          Password = d.Password,
                                          FullName = d.FullName,
                                          IsLocked = d.IsLocked,
                                          CreatedById = d.CreatedById,
                                          CreatedBy = d.CreatedBy,
                                          CreatedDateTime = d.CreatedDateTime.ToShortDateString(), // Format in memory
                                          UpdatedById = d.UpdatedById,
                                          UpdatedBy = d.UpdatedBy,
                                          UpdatedDateTime = d.UpdatedDateTime.ToShortDateString(), // Format in memory
                                      })
                                      .ToList();

            return users;
        }
        public String GetCreatedBy(int userId)
        {
            var user = from d in db.MstUsers
                       where d.Id == userId
                       select d;
            return user.FirstOrDefault().FullName;
        }
        public String GetUpdatedBy(int userId)
        {
            var user = from d in db.MstUsers
                       where d.Id == userId
                       select d;
            return user.FirstOrDefault().FullName;
        }

        // Detail User
        [Authorize, HttpGet, Route("api/user/detail/{id}")]
        public Entities.MstUser DetailUser(String id)
        {
            int userId = Convert.ToInt32(id);

            var dbUser = db.MstUsers.FirstOrDefault(d => d.Id == userId);

            if (dbUser == null) return null;

            var createdBy = GetCreatedBy(dbUser.CreatedById) ?? "";
            var updatedBy = GetUpdatedBy(dbUser.UpdatedById) ?? "";

            return new Entities.MstUser
            {
                Id = dbUser.Id,
                UserId = dbUser.UserId,
                UserName = dbUser.UserName,
                Password = dbUser.Password,
                FullName = dbUser.FullName,
                IsLocked = dbUser.IsLocked,
                CreatedById = dbUser.CreatedById,
                CreatedBy = createdBy,
                CreatedDateTime = dbUser.CreatedDateTime.ToShortDateString(),
                UpdatedById = dbUser.UpdatedById,
                UpdatedBy = updatedBy,
                UpdatedDateTime = dbUser.UpdatedDateTime.ToShortDateString()
            };

        }

        // Save User
        [Authorize, HttpPut, Route("api/user/save/{id}")]
        public HttpResponseMessage SaveUser(Entities.MstUser objUser, String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var user = from d in db.MstUsers
                               where d.Id == Convert.ToInt32(id)
                               select d;

                    if (user.Any())
                    {
                        if (!user.FirstOrDefault().IsLocked)
                        {
                            var saveUser = user.FirstOrDefault();
                            saveUser.FullName = objUser.FullName;
                            saveUser.UpdatedById = currentUserId;
                            saveUser.UpdatedDateTime = DateTime.Now;
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

        // Lock User
        [Authorize, HttpPut, Route("api/user/lock/{id}")]
        public HttpResponseMessage LockUser(Entities.MstUser objUser, String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var user = from d in db.MstUsers
                               where d.Id == Convert.ToInt32(id)
                               select d;

                    if (user.Any())
                    {
                        if (!user.FirstOrDefault().IsLocked)
                        {
                            var lockUser = user.FirstOrDefault();
                            lockUser.FullName = objUser.FullName;
                            lockUser.IsLocked = true;
                            lockUser.UpdatedById = currentUserId;
                            lockUser.UpdatedDateTime = DateTime.Now;

                            db.SubmitChanges();


                            return Request.CreateResponse(HttpStatusCode.OK);
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

        // Unlock User
        [Authorize, HttpPut, Route("api/user/unlock/{id}")]
        public HttpResponseMessage UnlockUser(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var user = from d in db.MstUsers
                                   where d.Id == Convert.ToInt32(id)
                                   select d;

                    if (user.Any())
                    {
                        if (user.FirstOrDefault().IsLocked)
                        {
                            var unlockUser = user.FirstOrDefault();
                            unlockUser.IsLocked = false;
                            unlockUser.UpdatedById = currentUserId;
                            unlockUser.UpdatedDateTime = DateTime.Now;

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

        // Delete User
        [Authorize, HttpDelete, Route("api/user/delete/{id}")]
        public HttpResponseMessage DeleteBank(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.UserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var currentUserId = currentUser.FirstOrDefault().Id;

                    var user = from d in db.MstUsers
                                   where d.Id == Convert.ToInt32(id)
                                   select d;

                    if (user.Any())
                    {
                        db.MstUsers.DeleteOnSubmit(user.First());

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
