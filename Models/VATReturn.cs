namespace SimplyMTD.Models
{
    public partial class VATReturn
    {
        public string periodKey { get; set; }

        public float vatDueSales { get; set; }

        public float vatDueAcquisitions { get; set; }

        public float totalVatDue { get; set; }

        public float vatReclaimedCurrPeriod { get; set; }

        public float netVatDue { get; set; }

        public int totalValueSalesExVAT { get; set; }

        public int totalValuePurchasesExVAT { get; set; }

        public int totalValueGoodsSuppliedExVAT { get; set; }

        public int totalAcquisitionsExVAT { get; set; }

        public bool finalised { get; set; }
    }
}
