using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders.Physical;
using static HalloDoc_Project.Extensions.Enumerations;

namespace HalloDoc_Project.Controllers
{
    [CustomAuthorize("Physician")]

    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminActions _adminActions;
        private readonly IAdminTables _adminTables;
        public ProviderController(ApplicationDbContext context, IAdminActions adminActions, IAdminTables adminTables)
        {
            _context = context;
            _adminActions = adminActions;
            _adminTables = adminTables;

        }
        public IActionResult ProviderDashboard()
        {
            var id = HttpContext.Session.GetString("AspnetuserId");
            AdminDashboardViewModel model=_adminTables.ProviderDashboard(id);
            return View(model);
        }
        public DashboardFilter SetDashboardFilterValues(int page, int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            return filter;
        }
        public IActionResult GetNewTable(int page, int region, int type, string search)
        {
            var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
            Physician physician = _context.Physicians.FirstOrDefault(x=>x.Aspnetuserid==aspnetuserid);
            var filter = SetDashboardFilterValues(page, region, type, search);
            AdminDashboardViewModel model = _adminTables.ProviderNewTable(filter,physician.Physicianid);
            model.currentPage = filter.pageNumber;

            return PartialView("PartialTables/ProviderNewTable", model);
        }
        public IActionResult GetPendingTable(int page, int region, int type, string search)
        {
            var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
            Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
            var filter = SetDashboardFilterValues(page, region, type, search);
            AdminDashboardViewModel model = _adminTables.ProviderPendingTable(filter, physician.Physicianid);
            model.currentPage = filter.pageNumber;

            return PartialView("PartialTables/ProviderPendingTable", model);
        }
        public IActionResult GetActiveTable(int page, int region, int type, string search)
        {
            var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
            Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
            var filter = SetDashboardFilterValues(page, region, type, search);
            AdminDashboardViewModel model = _adminTables.ProviderActiveTable(filter, physician.Physicianid);
            model.currentPage = filter.pageNumber;

            return PartialView("PartialTables/ProviderActiveTable", model);
        }
        public IActionResult GetConcludeTable(int page, int region, int type, string search)
        {
            var aspnetuserid = HttpContext.Session.GetString("AspnetuserId");
            Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
            var filter = SetDashboardFilterValues(page, region, type, search);
            AdminDashboardViewModel model = _adminTables.ProviderConcludeTable(filter, physician.Physicianid);
            model.currentPage = filter.pageNumber;

            return PartialView("PartialTables/ProviderConcludeTable", model);
        }
        public IActionResult AcceptCase(int requestid)
        {
            _adminActions.ProviderAcceptCase(requestid);
            return RedirectToAction("ProviderDashboard");
        }
        public IActionResult ProviderViewCase(int requestid)
        {
            if (ModelState.IsValid)
            {
                ViewCaseViewModel vc = _adminActions.ViewCaseAction(requestid);
                return View("ActionViews/ProviderViewCase",vc);
            }
            return View("ActionViews/ProviderViewCase");
        }
        public IActionResult ProviderViewNotes()
        {
            return View("ActionViews/ProviderViewNotes");
        }
    }

}
