using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Diagnostics;

namespace MajorxLechon.Controllers
{
    [Authorize]
    public class SoftwareController : UserAccountController
    {
        // ============
        // Data Context
        // ============
        private Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

        // ===========
        // Page Access
        // ===========
        public String PageAccess(String page)
        {
            String form = "";

            var currentUser = from d in db.MstUsers where d.UserId == User.Identity.GetUserId() select d;
            if (currentUser.Any())
            {
                //var userForms = from d in db.MstUserForms
                //                where d.UserId == currentUser.FirstOrDefault().Id
                //                select new Entities.MstUserForm
                //                {
                //                    Id = d.Id,
                //                    UserId = d.UserId,
                //                    User = d.MstUser.FullName,
                //                    FormId = d.FormId,
                //                    Form = d.SysForm.FormName,
                //                    Particulars = d.SysForm.Particulars,
                //                    CanAdd = d.CanAdd,
                //                    CanEdit = d.CanEdit,
                //                    CanDelete = d.CanDelete,
                //                    CanLock = d.CanLock,
                //                    CanUnlock = d.CanUnlock,
                //                    CanCancel = d.CanCancel,
                //                    CanPrint = d.CanPrint,
                //                };

                //foreach (var userForm in userForms)
                //{
                //    if (page.Equals(userForm.Form))
                //    {
                //        ViewData.Add("CanAdd", userForm.CanAdd);
                //        ViewData.Add("CanEdit", userForm.CanEdit);
                //        ViewData.Add("CanDelete", userForm.CanDelete);
                //        ViewData.Add("CanLock", userForm.CanLock);
                //        ViewData.Add("CanUnlock", userForm.CanUnlock);
                //        ViewData.Add("CanCancel", userForm.CanCancel);
                //        ViewData.Add("CanPrint", userForm.CanPrint);

                //        form = userForm.Form;
                //        break;
                //    }
                //}
            }

            return form;
        }

        // ========================
        // Formas With Detail Pages
        // ========================
        public String AccessToDetail(String page)
        {
            String form = "";

            var currentUser = from d in db.MstUsers where d.UserId == User.Identity.GetUserId() select d;
            //var company = from d in db.MstCompanies where d.Id == currentUser.FirstOrDefault().CompanyId select d;
            //if (currentUser.Any())
            //{
            //    var userForms = from d in db.MstUserForms
            //                    where d.UserId == currentUser.FirstOrDefault().Id
            //                    select new Entities.MstUserForm
            //                    {
            //                        Id = d.Id,
            //                        UserId = d.UserId,
            //                        User = d.MstUser.FullName,
            //                        FormId = d.FormId,
            //                        Form = d.SysForm.FormName,
            //                        Particulars = d.SysForm.Particulars,
            //                        CanAdd = d.CanAdd,
            //                        CanEdit = d.CanEdit,
            //                        CanDelete = d.CanDelete,
            //                        CanLock = d.CanLock,
            //                        CanUnlock = d.CanUnlock,
            //                        CanCancel = d.CanCancel,
            //                        CanPrint = d.CanPrint,
            //                    };

            //    foreach (var userForm in userForms)
            //    {
            //        if (page.Equals(userForm.Form))
            //        {
            //            form = userForm.Form;
            //            break;
            //        }
            //    }
            //}

            return form;
        }

        // ===========
        // User Rights
        // ===========
        public ActionResult UserRights(String formName)
        {
            var currentUser = from d in db.MstUsers where d.UserId == User.Identity.GetUserId() select d;
            //var company = from d in db.MstCompanies where d.Id == currentUser.FirstOrDefault().CompanyId select d;
            //var branch = from d in db.MstBranches where d.Id == currentUser.FirstOrDefault().BranchId select d;
            //DateTime expiryDate = branch.FirstOrDefault().ExpiryDate;
            //Boolean isExpired = false;
            //DateTime currentDate = DateTime.Today;
            //if(expiryDate <= currentDate)
            //{
            //    isExpired = true;
            //}
            if (currentUser.Any())
            {
                return View();
                //var userForms = from d in db.MstUserForms where d.UserId == currentUser.FirstOrDefault().Id && d.SysForm.FormName.Equals(formName) select d;
                //if (userForms.Any())
                //{
                //    var userFormsRights = userForms.FirstOrDefault();
                //    var model = new Entities.MstUserForm
                //    {
                //        CanAdd = userFormsRights.CanAdd,
                //        CanEdit = userFormsRights.CanEdit,
                //        CanDelete = userFormsRights.CanDelete,
                //        CanLock = userFormsRights.CanLock,
                //        CanUnlock = userFormsRights.CanUnlock,
                //        CanCancel = userFormsRights.CanCancel,
                //        CanPrint = userFormsRights.CanPrint,
                //    };

                //    if (!isExpired)
                //    {
                //        return View(model);
                //    }
                //    else
                //    {
                //        return RedirectToAction("SubscriptionExpired", "Software");
                //    }
                //}
                //else
                //{
                //    return RedirectToAction("Forbidden", "Software");
                //}
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // Software Pages
        public ActionResult PageNotFound() { return View(); }
        public ActionResult Forbidden() { return View(); }
        public ActionResult SubscriptionExpired() { return View(); }
        public ActionResult Index() { return View(); }

        // Setup
        public ActionResult ItemList() { return View(); }
        public ActionResult ItemDetail() { return View(); }

        // Activity
        public ActionResult OrderList() { return View(); }
        public ActionResult OrderDetail() { return View(); ; }

        // ======
        // System
        // ======
        public ActionResult UserList() { return View(); }
        public ActionResult UserDetail() { return View(); }

        // Reports
        public ActionResult ReportList() { return View(); }
    }
}