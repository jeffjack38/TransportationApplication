
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TransportationApplication.SharedModels
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }


    }
}
