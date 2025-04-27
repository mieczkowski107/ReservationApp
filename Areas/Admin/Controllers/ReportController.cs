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
    private readonly IReportService _reportService = reportService;
    private const string ReportExtension = ".csv";
    public IActionResult Index()
    {
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
        var reportFormVm = new ReportFormVM
        {
            CompanyList = _unitOfWork.Companies.GetAll( u => RoleService.IsAdmin(User)
                                                             || u.OwnerId == userId).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList(),
            StartRangeDate = DateOnly.FromDateTime(DateTime.Today),
            EndRangeDate = DateOnly.FromDateTime(DateTime.Today)
        };
        return View(reportFormVm);
    }
    [HttpPost]
    public IActionResult Result(ReportFormVM reportFormVm)
    {
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
        var report = _unitOfWork.Report.Get(p => (RoleService.IsAdmin(User) || p.Company!.OwnerId == userId)
                                                 && p.CompanyId == reportFormVm.CompanyId
                                                 && p.StartRangeDate == reportFormVm.StartRangeDate && p.EndRangeDate == reportFormVm.EndRangeDate);
        if (report == null)
        {
            report = _reportService.GetReport(reportFormVm.CompanyId, reportFormVm.StartRangeDate, reportFormVm.EndRangeDate);
            _unitOfWork.Report.Add(report);
            _unitOfWork.Save();
        }
        var reportVm = new ReportVm(report);
        return View(reportVm);
    }
    [HttpGet]
    public IActionResult DownloadReport(int reportId)
    {
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
        var report = _unitOfWork.Report.Get(p => p.Id == reportId && (RoleService.IsAdmin(User) || p.Company!.OwnerId == userId),
                                            includeProperties: nameof(Company),
                                            tracked: true);
        if(report == null)
        {
            return NotFound();
        }

        string fileName = $"{report.StartRangeDate}_{report.EndRangeDate}" + Path.GetExtension(ReportExtension);
        
        if (!System.IO.File.Exists(report.ReportUrl))
        {
            var reportVm = new ReportVm(report);
            string path = _reportService.GetReportPath(report.CompanyId) + @"\" + fileName;
            _reportService.WriteReportToCSV(reportVm, path);
            report.ReportUrl = path;
            _unitOfWork.Save();
        }
        return File(System.IO.File.ReadAllBytes(report.ReportUrl!), "text/csv",fileName);
    }


}
