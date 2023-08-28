using Newtonsoft.Json;
using Radzen.Blazor;
using Radzen;
using SimplyMTD.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SimplyMTD.Models.MTD;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.Json;

namespace SimplyMTD.Pages
{
	public partial class SubmitVAT
	{
		[Inject]
		protected IJSRuntime JSRuntime { get; set; }

		[Inject]
		public NotificationService NotificationService { get; set; }

		[Inject]
		public VATService VATService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected SimplyMTD.Models.ApplicationUser user;

        [Parameter]
		public dynamic periodKey { get; set; }

		[Parameter]
		public dynamic start { get; set; }

		[Parameter]
		public dynamic end { get; set; }

		protected override async Task OnInitializedAsync()
		{
			vatReturn = new SimplyMTD.Models.VATReturn();
            user = await Security.GetUserById($"{Security.User.Id}");
        }

		protected SimplyMTD.Models.VATReturn vatReturn;

		RadzenUpload upload;

		void OnProgress(UploadProgressArgs args, string name)
		{
			Console.WriteLine(name);
			if (args.Progress == 100)
			{
				foreach (var file in args.Files)
				{
				}
			}
		}

		void OnComplete(UploadCompleteEventArgs args)
		{
			List<string> amounts = JsonConvert.DeserializeObject<List<string>>(args.RawResponse);
			this.vatReturn.periodKey = periodKey;
			this.vatReturn.vatDueSales = float.Parse(amounts.ElementAt(1));
			this.vatReturn.vatDueAcquisitions = float.Parse(amounts.ElementAt(2));
			this.vatReturn.totalVatDue = float.Parse(amounts.ElementAt(3));
			this.vatReturn.vatReclaimedCurrPeriod = float.Parse(amounts.ElementAt(4));
			this.vatReturn.netVatDue = float.Parse(amounts.ElementAt(5));
			this.vatReturn.totalValueSalesExVAT = int.Parse(amounts.ElementAt(6));
			this.vatReturn.totalValuePurchasesExVAT = int.Parse(amounts.ElementAt(7));
			this.vatReturn.totalValueGoodsSuppliedExVAT = int.Parse(amounts.ElementAt(8));
			this.vatReturn.totalAcquisitionsExVAT = int.Parse(amounts.ElementAt(9));
			this.vatReturn.finalised = true;
		}

		void OnChange(UploadChangeEventArgs args, string name)
		{
			foreach (var file in args.Files)
			{
			}

		}

		protected async Task FormSubmit()
		{
			bool res = await VATService.submitVAT(vatReturn);
			if(res)
			{
				ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Success Summary", Detail = "", Duration = 4000 });
			} else
			{
				ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error Summary", Detail = "", Duration = 4000 });
			}
		}

		void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
		{
			//console.Log($"InvalidSubmit: {JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
		}

		void Cancel()
		{
			//
		}

		void ShowNotification(NotificationMessage message)
		{
			NotificationService.Notify(message);

		}
	}
}