using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using SimplyMTD.Models.MTD;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace SimplyMTD.Models
{
    public partial class ApplicationUser : IdentityUser
    {

        [JsonIgnore, IgnoreDataMember]
        public override string PasswordHash { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        [JsonIgnore, IgnoreDataMember, NotMapped]
        public string Name
        {
            get
            {
                return UserName;
            }
            set
            {
                UserName = value;
            }
        }
        

		public ICollection<ApplicationRole> Roles { get; set; }

        public virtual UserDetail UserDetail { get; set; }

		public virtual Billing Billing { get; set; }

		public virtual Accounting Accounting { get; set; }

		public virtual Accountant Accountant { get; set; }
	}
}