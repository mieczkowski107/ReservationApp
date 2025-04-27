using ReservationApp.Utility.Enums;
using System.Security.Claims;

namespace ReservationApp.Services;

public class RoleService
{
    public static bool IsAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole(Role.Admin.ToString());
    }
}
