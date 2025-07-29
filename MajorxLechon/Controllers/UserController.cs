using System.Linq;
using System.Web.Mvc;
using MajorxLechon.Models;

namespace MajorxLechon.Controllers
{
    [Authorize]
    public class UserAccountController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                var context = new ApplicationDbContext();
                var username = User.Identity.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    // ======================
                    // Current Logged-In User
                    // ======================
                    var user = context.Users.SingleOrDefault(u => u.UserName == username);

                    // ==========
                    // AspNetUser
                    // ==========
                    string aspNetUserId = user.Id;
                    string userName = user.UserName;
                    string fullName = user.FullName;

                    ViewData.Add("UserId", aspNetUserId);
                    ViewData.Add("UserName", userName);
                    ViewData.Add("FullName", fullName);

                    // =======
                    // MstUser
                    // =======
                    Data.majorxlechondbDataContext db = new Data.majorxlechondbDataContext();

                    var currentUser = from d in db.MstUsers where d.UserId == aspNetUserId select d;
                    if (currentUser.Any())
                    {
                        // ==========================
                        // Current Branch and Company
                        //// ==========================
                        //int branchId = currentUser.FirstOrDefault().BranchId;
                        //string branch = currentUser.FirstOrDefault().MstBranch.Branch;
                        //string branchCode = currentUser.FirstOrDefault().MstBranch.BranchCode + " - " + currentUser.FirstOrDefault().MstBranch.Branch;
                        //int companyId = currentUser.FirstOrDefault().CompanyId;
                        //string company = currentUser.FirstOrDefault().MstCompany.Company;

                        //ViewData.Add("BranchId", branchId);
                        //ViewData.Add("Branch", branch);
                        //ViewData.Add("BranchCode", branchCode);
                        //ViewData.Add("CompanyId", companyId);
                        //ViewData.Add("Company", company);

                        //// ==================
                        //// Defaults (Current)
                        //// ==================
                        //int mstUserId = currentUser.FirstOrDefault().Id;
                        //string salesInvoiceName = currentUser.FirstOrDefault().SalesInvoiceName;
                        //int defaultSalesInvoiceDiscountId = currentUser.FirstOrDefault().DefaultSalesInvoiceDiscountId;
                        //string defaultSalesInvoiceDiscount = currentUser.FirstOrDefault().MstDiscount.Discount;
                        //string officialReceiptName = currentUser.FirstOrDefault().OfficialReceiptName;

                        //ViewData.Add("MstUserId", mstUserId);
                        //ViewData.Add("SalesInvoiceName", salesInvoiceName);
                        //ViewData.Add("defaultSalesInvoiceDiscountId", defaultSalesInvoiceDiscountId);
                        //ViewData.Add("DefaultSalesInvoiceDiscount", defaultSalesInvoiceDiscount);
                        //ViewData.Add("OfficialReceiptName", officialReceiptName);

                        //// ================
                        //// System (Current)
                        //// ================
                        //int netIncomeAccountId = currentUser.FirstOrDefault().IncomeAccountId;
                        //string netIncomeAccount = currentUser.FirstOrDefault().MstAccount.Account;
                        //int supplierAdvancesAccountId = currentUser.FirstOrDefault().SupplierAdvancesAccountId;
                        //string supplierAdvancesAccount = currentUser.FirstOrDefault().MstAccount1.Account;
                        //int customerAdvancesAccountId = currentUser.FirstOrDefault().CustomerAdvancesAccountId;
                        //string customerAdvancesAccount = currentUser.FirstOrDefault().MstAccount2.Account;
                        //string inventoryType = currentUser.FirstOrDefault().InventoryType;
                        //bool isIncludeCostStockReports = currentUser.FirstOrDefault().IsIncludeCostStockReports;
                        //bool allowEditCostPR = currentUser.FirstOrDefault().MstCompany.AllowEditCostPR;
                        //bool allowEdiCostPO = currentUser.FirstOrDefault().MstCompany.AllowEditCostPO;
                        //bool allowEditCostRR = currentUser.FirstOrDefault().MstCompany.AllowEditCostRR;
                        //bool allowEditCostIN = currentUser.FirstOrDefault().MstCompany.AllowEditCostIN;
                        //bool allowEditCostOT = currentUser.FirstOrDefault().MstCompany.AllowEditCostOT;
                        //bool allowEditCostST = currentUser.FirstOrDefault().MstCompany.AllowEditCostST;
                        //bool tagAccountType = currentUser.FirstOrDefault().MstCompany.TagAccountType;
                        //bool disableUpdateCost = currentUser.FirstOrDefault().MstCompany.DisableUpdateCost;
                        //bool poDiscount = currentUser.FirstOrDefault().MstCompany.PODiscount;
                        //bool showRider = currentUser.FirstOrDefault().MstCompany.ShowRider;
                        //bool enableItemPriceCostUpdate = currentUser.FirstOrDefault().MstCompany.EnableItemPriceCostUpdate;
                        //bool showCostInItemList = currentUser.FirstOrDefault().MstCompany.ShowCostInItemList;
                        //bool activatePaging = currentUser.FirstOrDefault().MstCompany.ActivatePaging;
                        //bool useAddressDetails = currentUser.FirstOrDefault().MstCompany.UseAddressDetails;
                        //bool withBreakfast = currentUser.FirstOrDefault().MstCompany.MistWithBreakfastSettings;
                        //bool jmpls = currentUser.FirstOrDefault().MstCompany.ActivateJMPLS;
                        //bool approval = currentUser.FirstOrDefault().MstCompany.ActivateApprovalBeforePost;
                        //bool canApprove = currentUser.FirstOrDefault().CanApprove;
                        //bool canEditSettings = currentUser.FirstOrDefault().CanEditSettings;
                        //bool enablePricePerBranch = currentUser.FirstOrDefault().MstCompany.EnablePricePerBranch;
                        //bool marides = currentUser.FirstOrDefault().MstCompany.ActivateMarides;
                        //bool champion = currentUser.FirstOrDefault().MstCompany.ActivateChampion;
                        //bool purchasePrice = currentUser.FirstOrDefault().MstCompany.HidePurchasePrice;
                        //bool reprint = currentUser.FirstOrDefault().CanReprint;
                        //bool collectionDate = currentUser.FirstOrDefault().CanEditCollectionDate;
                        //bool cost = currentUser.FirstOrDefault().CanEditCost;

                        //ViewData.Add("NetIncomeAccountId", netIncomeAccountId);
                        //ViewData.Add("NetIncomeAccount", netIncomeAccount);
                        //ViewData.Add("SupplierAdvancesAccountId", supplierAdvancesAccountId);
                        //ViewData.Add("SupplierAdvancesAccount", supplierAdvancesAccount);
                        //ViewData.Add("CustomerAdvancesAccountId", customerAdvancesAccountId);
                        //ViewData.Add("CustomerAdvancesAccount", customerAdvancesAccount);
                        //ViewData.Add("InventoryType", inventoryType);
                        //ViewData.Add("IsIncludeCostStockReports", isIncludeCostStockReports);
                        //ViewData.Add("AllowEditCostPR", allowEditCostPR);
                        //ViewData.Add("AllowEditCostPO", allowEdiCostPO);
                        //ViewData.Add("AllowEditCostRR", allowEditCostRR);
                        //ViewData.Add("AllowEditCostIN", allowEditCostIN);
                        //ViewData.Add("AllowEditCostOT", allowEditCostOT);
                        //ViewData.Add("AllowEditCostST", allowEditCostST);
                        //ViewData.Add("TagAccountType", tagAccountType);
                        //ViewData.Add("DisableUpdateCost", disableUpdateCost);
                        //ViewData.Add("PODiscount", poDiscount);
                        //ViewData.Add("ShowRider", showRider);
                        //ViewData.Add("EnableItemPriceCostUpdate", enableItemPriceCostUpdate);
                        //ViewData.Add("ActivatePaging", activatePaging);
                        //ViewData.Add("UseAddressDetails", useAddressDetails);
                        //ViewData.Add("MistWithBreakfastSettings", withBreakfast);
                        //ViewData.Add("ActivateJMPLS", jmpls);
                        //ViewData.Add("ShowCostInItemList", showCostInItemList);
                        //ViewData.Add("ActivateApprovalBeforePost", approval);
                        //ViewData.Add("CanApprove", canApprove);
                        //ViewData.Add("CanEditSettings", canEditSettings);
                        //ViewData.Add("EnablePricePerBranch", enablePricePerBranch);
                        //ViewData.Add("ActivateMarides", marides);
                        //ViewData.Add("ActivateChampion", champion);
                        //ViewData.Add("HidePurchasePrice", purchasePrice);
                        //ViewData.Add("CanReprint", reprint);
                        //ViewData.Add("CanEditCollectionDate", collectionDate);
                        //ViewData.Add("CanEditCost", cost);
                    }
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
