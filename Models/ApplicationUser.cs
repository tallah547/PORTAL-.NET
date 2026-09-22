using Microsoft.AspNetCore.Identity;

namespace PORTAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string StudentCode {  get; set; }
        
    }
       
}
