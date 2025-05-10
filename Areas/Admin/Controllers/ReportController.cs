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
    private const string ReportExtension = ".csv";
    public IActionResult Index()
    {
        var userId = UserService.GetUserId(User);
        var reportFormVm = new ReportFormVM
        {
            CompanyList = unitOfWork.Companies.GetAll( u => UserService.IsAdmin(User)
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
        var userId = UserService.GetUserId(User);
        var report = unitOfWork.Report.Get(p => (UserService.IsAdmin(User) || p.Company!.OwnerId == userId)
                                                 && p.CompanyId == reportFormVm.CompanyId
                                                 && p.StartRangeDate == reportFormVm.StartRangeDate && p.EndRangeDate == reportFormVm.EndRangeDate);
        if (report == null)
        {
            report = reportService.GetReport(reportFormVm.CompanyId, reportFormVm.StartRangeDate, reportFormVm.EndRangeDate);
            unitOfWork.Report.Add(report);
            unitOfWork.Save();
        }
        var reportVm = new ReportVm(report);
        return View(reportVm);
    }
    [HttpGet]
    public IActionResult DownloadReport(int reportId)
    {
        var userId = UserService.GetUserId(User);
        var report = unitOfWork.Report.Get(p => p.Id == reportId && (UserService.IsAdmin(User) || p.Company!.OwnerId == userId),
                                            includeProperties: nameof(Company),
                                            tracked: true);
        if(report == null)
        {
            return NotFound();
        }

        var fileName = $"{report.StartRangeDate}_{report.EndRangeDate}" + Path.GetExtension(ReportExtension);
        
        if (!System.IO.File.Exists(report.ReportUrl))
        {
            var reportVm = new ReportVm(report);
            var path = reportService.GetReportPath(report.CompanyId) + @"\" + fileName;
            reportService.WriteReportToCSV(reportVm, path);
            report.ReportUrl = path;
            unitOfWork.Save();
        }
        return File(System.IO.File.ReadAllBytes(report.ReportUrl!), "text/csv",fileName);
    }


}
