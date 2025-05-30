﻿using ReservationApp.Utility.Enums;
using System.Security.Claims;

namespace ReservationApp.Services;

public static class UserService
{
    public static bool IsAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole(Role.Admin.ToString());
    }
    public static bool IsCompanyManager(ClaimsPrincipal user)
    {
        return user.IsInRole(Role.CompanyManager.ToString());
    }
    public static bool IsCustomer(ClaimsPrincipal user)
    {
        return user.IsInRole(Role.Customer.ToString());
    }
    public static Guid GetUserId(ClaimsPrincipal user)
    {
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId);
        return userId;
    }
}
