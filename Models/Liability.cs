namespace SimplyMTD.Models
{
	public partial class Liability
	{
		public TaxPeroid taxPeriod { get; set; }

		public string type { get; set; }

		public float originalAmount { get; set; }

		public float outstandingAmount { get; set; }

		public string due { get; set; }

		public class TaxPeroid
		{
			public string from { get; set;}

			public string to { get; set; }
		}
	}
}
