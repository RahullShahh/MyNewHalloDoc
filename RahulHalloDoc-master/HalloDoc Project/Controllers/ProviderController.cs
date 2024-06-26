﻿using AspNetCoreHero.ToastNotification.Abstractions;
using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders.Physical;
using Rotativa.AspNetCore;
using static HalloDoc_Project.Extensions.Enumerations;

namespace HalloDoc_Project.Controllers
{
    [CustomAuthorize("Physician")]

    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminActions _adminActions;
        private readonly IAdminTables _adminTables;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileOperations _fileOperations;
        private readonly INotyfService _notyf;
        private readonly IEncounterForm _encounterForm;
        public ProviderController(ApplicationDbContext context, IAdminActions adminActions, IAdminTables adminTables, IEmailService emailService, IWebHostEnvironment environment, IFileOperations fileOperations,INotyfService notyf, IEncounterForm encounterForm)
        {
            _context = context;
            _adminActions = adminActions;
            _adminTables = adminTables;
            _emailService = emailService;
            _environment = environment;
            _fileOperations = fileOperations;
            _notyf = notyf;
            _encounterForm = encounterForm;
        }
        public IActionResult ProviderDashboard()
        {
            var id = HttpContext.Session.GetString("AspnetuserId");
            AdminDashboardViewModel model = _adminTables.ProviderDashboard(id);
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
            Physician physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == aspnetuserid);
            var filter = SetDashboardFilterValues(page, region, type, search);
            AdminDashboardViewModel model = _adminTables.ProviderNewTable(filter, physician.Physicianid);
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
                return View("ActionViews/ProviderViewCase", vc);
            }
            return View("ActionViews/ProviderViewCase");
        }
        public IActionResult ProviderViewNotes()
        {
            return View("ActionViews/ProviderViewNotes");
        }
        [HttpPost]
        public IActionResult SendAgreement(int RequestId, string PhoneNo, string email)
        {
            if (ModelState.IsValid)
            {
                var AgreementLink = Url.Action("ReviewAgreement", "Guest", new { ReqId = RequestId }, Request.Scheme);
                _emailService.SendAgreementLink(RequestId, AgreementLink, email);
                return RedirectToAction("AdminDashboard", "Guest");
            }
            return View();
        }
        #region Send Link
        [HttpPost]
        public void SendLink(string FirstName, string LastName, string Email)
        {
            var WebsiteLink = Url.Action("patient_submit_request_screen", "Guest", new { }, Request.Scheme);
            _emailService.SendEmailWithLink(FirstName, LastName, Email, WebsiteLink);
        }
        #endregion

        #region Transfer Case Methods

        [HttpPost]
        public IActionResult TransferCase(int RequestId, string TransferPhysician, string TransferDescription)
        {
            var phyId = HttpContext.Session.GetString("AspnetuserId");
            var physician = _context.Physicians.FirstOrDefault(x => x.Aspnetuserid == phyId);
            _adminActions.ProviderTransferCase(RequestId, TransferPhysician, TransferDescription, physician.Physicianid);
            return Ok();
        }
        #endregion

        #region Send Orders Methods
        public List<Healthprofessional> filterVenByPro(string ProfessionId)
        {
            var result = _context.Healthprofessionals.Where(u => u.Profession == int.Parse(ProfessionId)).ToList();
            return result;
        }
        public IActionResult BusinessData(int BusinessId)
        {
            var result = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == BusinessId);
            return Json(result);
        }
        public IActionResult SendOrders(int requestid)
        {
            List<Healthprofessional> healthprofessionals = _context.Healthprofessionals.ToList();
            List<Healthprofessionaltype> healthprofessionaltypes = _context.Healthprofessionaltypes.ToList();
            SendOrderViewModel model = new()
            {
                requestid = requestid,
                healthprofessionals = healthprofessionals,
                healthprofessionaltype = healthprofessionaltypes
            };
            return View("ActionViews/ProviderSendOrders",model);
        }

        [HttpPost]
        public IActionResult SendOrders(int requestid, SendOrderViewModel sendOrder)
        {
            _adminActions.SendOrderAction(requestid, sendOrder);
            return RedirectToAction("ProviderDashboard");
        }
        #endregion

        #region LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("login_page", "Guest");
        }
        #endregion

        #region CONCLUDE CARE
        public IActionResult ConcludeCareDeleteFile(int fileid, int requestid)
        {
            var fileRequest = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileid);
            fileRequest.Isdeleted = true;

            _context.Requestwisefiles.Update(fileRequest);
            _context.SaveChanges();

            return RedirectToAction("ActionViews/ProviderConcludeCare", new { requestid = requestid });
        }
        public IActionResult ProviderConcludeCare(int requestid)
        {

            var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            var requestFile = _context.Requestwisefiles.Where(r => r.Requestid == requestid).ToList();
            var requests = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            //var encounterform = _context.Encounterforms.FirstOrDefault(r=>r.Requestid==requestid);

            ViewUploadsViewModel uploads = new()
            {
                ConfirmationNo = requests.Confirmationnumber,
                Patientname = user.Firstname + " " + user.Lastname,
                RequestID = requestid,
                Requestwisefiles = requestFile,
                //isFinalized = encounterform.Isfinalize
            };
            
            return View("ActionViews/ProviderConcludeCare", uploads);
        }

        #endregion

        #region View Uploads 
        public IActionResult DeleteFile(int fileid, int requestid)
        {
            var fileRequest = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileid);
            fileRequest.Isdeleted = true;

            _context.Requestwisefiles.Update(fileRequest);
            _context.SaveChanges();

            return RedirectToAction("ViewUploads", new { requestid = requestid });
        }
        public IActionResult DeleteAllFiles(int requestid)
        {
            var request = _context.Requestwisefiles.Where(r => r.Requestid == requestid && r.Isdeleted != true).ToList();
            for (int i = 0; i < request.Count; i++)
            {
                request[i].Isdeleted = true;
                _context.Update(request[i]);
            }
            _context.SaveChanges();
            return RedirectToAction("ViewUploads", new { requestid = requestid });
        }
        public IActionResult ViewUploads(int requestid)
        {

            var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            var requestFile = _context.Requestwisefiles.Where(r => r.Requestid == requestid).ToList();
            var requests = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

            ViewUploadsViewModel uploads = new()
            {
                ConfirmationNo = requests.Confirmationnumber,
                Patientname = user.Firstname + " " + user.Lastname,
                RequestID = requestid,
                Requestwisefiles = requestFile
            };
            return View("ActionViews/ProviderViewUploads", uploads);
        }
        [HttpPost]
        public IActionResult ViewUploads(ViewUploadsViewModel uploads)
        {
            if (uploads.File != null)
            {
                var uniqueid = Guid.NewGuid().ToString();
                var path = _environment.WebRootPath;
                _fileOperations.insertfilesunique(uploads.File, uniqueid, path);

                var filestring = Path.GetFileNameWithoutExtension(uploads.File.FileName);
                var extensionstring = Path.GetExtension(uploads.File.FileName);
                Requestwisefile requestwisefile = new()
                {
                    Filename = uniqueid + "$" + uploads.File.FileName,
                    Requestid = uploads.RequestID,
                    Createddate = DateTime.Now
                };
                _context.Update(requestwisefile);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewUploads", new { requestid = uploads.RequestID });
        }
        public IActionResult SendMail(int requestid)
        {
            var path = _environment.WebRootPath;
            _emailService.SendEmailWithAttachments(requestid, path);
            return RedirectToAction("ViewUploads", "Provider", new { requestid = requestid });
        }
        #endregion

        #region Encounter Form Methods
        [HttpGet]
        public IActionResult FinalizeDownload(int requestid)
        {
            var EncounterModel = _encounterForm.EncounterFormGet(requestid);
            if (EncounterModel == null)
            {
                return NotFound();
            }
            return new ViewAsPdf("ActionViews/EncounterFormFinalizeView", EncounterModel)
            {
                FileName = "FinalizedEncounterForm.pdf"
            };
        }
        public IActionResult FinalizeForm(int requestid)
        {
            Encounterform encounterRecord = _context.Encounterforms.FirstOrDefault(x => x.Requestid == requestid);
            if (encounterRecord != null)
            {
                encounterRecord.Isfinalize = true;
                _context.Encounterforms.Update(encounterRecord);
            }
            _context.SaveChanges();
            return RedirectToAction("ProviderDashboard", "Provider");
        }
        public IActionResult EncounterForm(int requestId, EncounterFormViewModel EncModel)
        {
            EncModel = _encounterForm.EncounterFormGet(requestId);
            var RequestExistStatus = _context.Encounterforms.FirstOrDefault(x => x.Requestid == requestId);
            if (RequestExistStatus == null)
            {
                EncModel.IfExists = false;
            }
            if (RequestExistStatus != null)
            {
                EncModel.IfExists = true;
            }
            return View("ActionViews/ProviderEncounterForm", EncModel);
        }
        [HttpPost]
        public IActionResult EncounterForm(EncounterFormViewModel model)
        {
            _encounterForm.EncounterFormPost(model.requestId, model);
            return EncounterForm(model.requestId, model);
        }

        //set consultancy type
        public IActionResult SetConsultancyType(int typeId,int requestId)
        {
            Request request = _context.Requests.FirstOrDefault(x=>x.Requestid==requestId);
            if(request != null)
            {
                //for housecall
                if (typeId == 1)
                {
                    request.Calltype = 1;
                    request.Status = 5;

                    _context.Requests.Update(request);
                }
                //for consult
                else
                {
                    request.Status = 6;
                    _context.Requests.Update(request);
                }
                _context.SaveChanges();
            }
            return RedirectToAction("ProviderDashboard","Provider");
        }

        public void HousecallConcluded(int Requestid)
        { 
            Request request=_context.Requests.FirstOrDefault(x=>x.Requestid==Requestid);
            if(request != null)
            {
                request.Status= 6;
            }
            _notyf.Information("Request concluded");
        }
        #endregion
    }

}
