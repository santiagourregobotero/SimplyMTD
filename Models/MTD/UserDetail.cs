using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplyMTD.Models.MTD
{
    [Table("UserDetails", Schema = "dbo")]
    public partial class UserDetail
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; }

		[ForeignKey("ApplicationUser")]
		public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

		public int ClientId { get; set; }

        public string Vrn { get; set; }

        public string BusinessName { get; set; }

        public string OwnerName { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string PostCode { get; set; }

        public string Nino { get; set; }

        public string BusinessType { get; set; }

		public string Photo { get; set; }

		public string PhoneNumber { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public DateTime? Deadline { get; set; }

    }
}