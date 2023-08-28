using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace SimplyMTD.Models
{
    public partial class ApplicationRole : IdentityRole
    {
        [JsonIgnore]
        public ICollection<ApplicationUser> Users { get; set; }
    }
}