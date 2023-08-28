using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplyMTD.Models.MTD
{
	[Table("Billing", Schema = "dbo")]
	public partial class Billing
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; }

		public string AccountName { get; set; }

		public string CardNumber { get; set; }

		public DateTime ExpireDate { get; set; }

		public string BillingAddress { get; set; }

		[ForeignKey("ApplicationUser")]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

	}
}
