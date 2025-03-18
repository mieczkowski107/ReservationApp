using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReservationApp.Utility.Enums
{
    public enum Role
    {
        None,           // Represents the user who is not logged in
        Customer,       // Represents the user who is not an employee of the company
        Employee,       // Represents the user who is an employee of the company, access will be restricted
        [Display(Name = "Company Manager")]
        [Description("Company Manager")]
        CompanyManager,  // Represents owner of the company
        Admin    // Represents the user who has full access to the system,
    }
}
