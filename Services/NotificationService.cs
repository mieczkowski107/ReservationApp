using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using ReservationApp.Data.Repository.IRepository;
using ReservationApp.Models;
using ReservationApp.Services.Interfaces;
using ReservationApp.Utility.Enums;

namespace ReservationApp.Services;
public class NotificationService(IUnitOfWork unitOfWork, IEmailSender emailSender) : INotificationService
{
    public void CreateNotification(NotificationType type, int appointmentId)
    {
        var appointment = GetAppointmentDetails(appointmentId);
        if(appointment == null)
        {
            return;
        }
        if (appointment.Status != AppointmentStatus.Cancelled)
        {
            var (title, message) = PrepareNotification(type, appointment);
            var notification = new Notification
            {
                Title = title,
                Message = message,
                AppointmentId = appointmentId,
                Type = type,
                Status = NotificationStatus.Created,
                userEmail = appointment.User!.Email!
            };
            SaveNotificationToDb(notification);
        }
    }
    public async Task SendNotification(int appointmentId)
    {
        var notification = unitOfWork.Notification.Get(n => n.AppointmentId == appointmentId && n.Status != NotificationStatus.Sent,
                                                       tracked: true);
        if (notification != null && notification.Status != NotificationStatus.Sent)
        {
            await emailSender.SendEmailAsync(notification.userEmail, notification.Title!, notification.Message!);
            notification.Status = NotificationStatus.Sent;
            unitOfWork.Save();
        }
    }
    private Appointment? GetAppointmentDetails(int appointmentId)
    {
        var appointment = unitOfWork.Appointments.Get(a => a.Id == appointmentId, includeProperties: "Service.Company,User");
        return appointment;
    }
    private static (string title, string message) PrepareNotification(NotificationType type, Appointment appointment)
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
        message += $"Appointment details: {appointment.Service!.Name} on {appointment.Date} at {appointment.Time}.\n" +
                   $"Address: {appointment.Service!.Company!.Address}, {appointment.Service.Company.City}, " +
                   $"{appointment.Service.Company.State}";

        return (title, message);
    }

    private void SaveNotificationToDb(Notification notification)
    {
        unitOfWork.Notification.Add(notification);
        unitOfWork.Save();
    }


}