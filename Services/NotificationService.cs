using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;
public class NotificationService 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    public NotificationService(IUnitOfWork unitOfWork, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
    }

    private Appointment GetAppointmentDetails(int appointmentId)
    {
        var appointment = _unitOfWork.Appointments.Get(a => a.Id == appointmentId, includeProperties: "Service.Company,User");
        if(appointment == null)
        {
            throw new Exception("Appointment not found");
        }
        return appointment;
    }

    
    public void CreateNotification(NotificationType type, int appointmentId)
    {
        var appointment = GetAppointmentDetails(appointmentId);
        if(appointment.Status == AppointmentStatus.Cancelled)
        {
            throw new Exception("Appointment is already cancelled or completed");
        }
        var details = PrepareNotification(type, appointment);
        var notification = new Notification
        {
            Title = details.title,
            Message = details.message,
            AppointmentId = appointmentId,
            Type = type,
            Status = NotificationStatus.Created,
            userEmail = appointment.User!.Email!
        };
        SaveNotificationToDb(notification);
    }

    private (string title, string message) PrepareNotification(NotificationType type, Appointment appointment)
    {
        string title = string.Empty;
        string message = string.Empty;

        switch (type)
        {
            case NotificationType.Confirmation:
                title = "Appointment Confirmed!";
                message = "Your appointment has been confirmed";
                break;
            case NotificationType.Cancellation:
                title = "Appointment Cancelled!";
                message = "Your appointment has been cancelled";
                break;
            case NotificationType.Reminder:
                title = "Appointment Reminder!";
                message = "You have an appointment in 24 hours";
                break;
            case NotificationType.Completed:
                title = "Appointment Completed!";
                message = "Your appointment has been completed. Please leave a review! URL: xyz.com ";
                break;
        }
        message += $"Appointment details: {appointment.Service.Name} on {appointment.Date} at {appointment.Time}.\n" +
                   $"Address: {appointment.Service.Company.Address}, {appointment.Service.Company.City}, {appointment.Service.Company.State}";

        return (title, message);
    }


    public async Task SendNotification(int appointmentId)
    {
        var notification = _unitOfWork.Notification.Get(n => n.AppointmentId == appointmentId && n.Status != NotificationStatus.Sent, tracked: true);
        if(notification == null)
        {
            throw new Exception("Notification not found");
        }
        await _emailSender.SendEmailAsync(notification.userEmail, notification.Title!, notification.Message!);
       
        notification.Status = NotificationStatus.Sent;
        _unitOfWork.Save();
    }

    private void SaveNotificationToDb(Notification notification)
    {
        _unitOfWork.Notification.Add(notification);
        _unitOfWork.Save();
    }


}