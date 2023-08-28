using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplyMTD.Models.MTD
{
	[Table("Accounting", Schema = "dbo")]
	public partial class Accounting
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; }

		public DateTime TradeStartDate { get; set; }

		public DateTime TradeEndDate { get; set; }

		public DateTime AccountingStartDate { get; set; }

		public DateTime AccountingEndDate { get; set; }

		public string Basis { get; set; }

		public int Frequency { get; set; }

		[ForeignKey("ApplicationUser")]
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

	}
}
