using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Models.ViewModels;
using ReservationApp.Services;
using ReservationApp.Services.Interfaces;
using System.Security.Claims;

namespace ReservationApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "CompanyManager,Admin")]
public class ReportController(IUnitOfWork unitOfWork, IReportService reportService) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IReportService reportService = reportService;
    private const string _reportExtenxion = ".csv";
    public IActionResult Index()
    {
        var userId = new Guid();
        if (!RoleService.IsAdmin(User))
        {
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }

        var reportFormVM = new ReportFormVM
        {
            CompanyList = _unitOfWork.Companies.GetAll(u => u.OwnerId == userId).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList(),
            StartRangeDate = DateOnly.FromDateTime(DateTime.Today),
            EndRangeDate = DateOnly.FromDateTime(DateTime.Today)
        };
        return View(reportFormVM);
    }
    [HttpPost]
    public IActionResult Result(ReportFormVM reportFormVM)
    {
        var report = _unitOfWork.Report.Get(p => p.CompanyId == reportFormVM.CompanyId && p.StartRangeDate == reportFormVM.StartRangeDate && p.EndRangeDate == reportFormVM.EndRangeDate);
        if (report == null)
        {
            report = reportService.GetReport(reportFormVM.CompanyId, reportFormVM.StartRangeDate, reportFormVM.EndRangeDate);
            _unitOfWork.Report.Add(report);
            _unitOfWork.Save();
        }
        var reportVm = new ReportVm(report);
        return View(reportVm);
    }
    [HttpGet]
    public IActionResult DownloadReport(int reportId)
    {
        var report = _unitOfWork.Report.Get(p=>p.Id == reportId, includeProperties: nameof(Company), tracked: true);
        if(report == null)
        {
            return NotFound();
        }

        string fileName = $"{report.StartRangeDate}_{report.EndRangeDate}" + Path.GetExtension(_reportExtenxion);
        
        if (!System.IO.File.Exists(report.ReportUrl))
        {
            var reportVm = new ReportVm(report);
            string path = reportService.GetReportPath(report.CompanyId) + @"\" + fileName;
            reportService.WriteReportToCSV(reportVm, path);
            report.ReportUrl = path;
            _unitOfWork.Save();
        }
        return File(System.IO.File.ReadAllBytes(report.ReportUrl!), "text/csv",fileName);
    }


}
