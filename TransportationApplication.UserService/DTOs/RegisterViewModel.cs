using System.ComponentModel.DataAnnotations;

namespace TransportationApplication.UserService.DTOs
{
    public class RegisterViewModel
    {
        //will throw a error if emailaddress doesn't have '@' or end in .com,.org,.net....
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }

        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be more than 100 characters long.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } //Roles: Admin, Driver, Dispatcher, Customer





    }
}
