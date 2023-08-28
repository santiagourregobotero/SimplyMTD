using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Microsoft.OData;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using SimplyMTD.Models;
using SimplyMTD.Models.MTD;
using static System.Net.WebRequestMethods;

namespace SimplyMTD.Pages
{
	public partial class Index
	{
		[Inject]
		protected IJSRuntime JSRuntime { get; set; }

		[Inject]
		protected NavigationManager UriHelper { get; set; }

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		[Inject]
		protected DialogService DialogService { get; set; }

		[Inject]
		protected TooltipService TooltipService { get; set; }

		[Inject]
		protected ContextMenuService ContextMenuService { get; set; }

		[Inject]
		protected NotificationService NotificationService { get; set; }

		[Inject]
		protected SecurityService Security { get; set; }

		[Inject]
		public VATService VATService { get; set; }

		[Inject]
		HttpClient Http { get; set; }

		[Inject]
        public MTDService MTDService { get; set; }

		protected IEnumerable<Obligation> obligations;

		protected IEnumerable<Liability> liabilities;

		protected IEnumerable<Payment> payments;

		protected RadzenDataGrid<Obligation> grid1;

		protected RadzenDataGrid<Liability> grid2;

		protected RadzenDataGrid<Payment> grid3;

		protected SimplyMTD.Models.ApplicationUser user;

		protected SimplyMTD.Models.MTD.UserDetail userDetail;

		protected override async Task OnInitializedAsync()
		{
			await Load();
		}

		protected async System.Threading.Tasks.Task Load()
		{
			obligations = await VATService.GetObligations();

			foreach(var obligation in obligations)
			{
				if(obligation.status == "O")
				{
					// update the client 
					user = await Security.GetUserById($"{Security.User.Id}");
					userDetail = await MTDService.GetUserDetailByUserId($"{Security.User.Id}");
					userDetail.Start = DateTime.Parse(obligation.start);
					userDetail.End = DateTime.Parse(obligation.end);
					userDetail.Deadline = DateTime.Parse(obligation.due);
					await MTDService.UpdateUserDetail(userDetail.Id, userDetail);
					/*user.Start = DateTime.Parse(obligation.start);
					user.End = DateTime.Parse(obligation.end);
					user.Deadline = DateTime.Parse(obligation.due);*/
					break;
				}
			}

			liabilities = await VATService.GetLiabilities();

			payments = await VATService.GetPayments();
		}

		public async Task OpenObligation(string periodKey)
		{
			await DialogService.OpenAsync<EditObligation>("View Obligation", new Dictionary<string, object> { { "PeriodKey", periodKey } },
				new DialogOptions() { Width = "900px", Resizable = true, Draggable = true });
		}

		async Task Submit(MouseEventArgs args, string periodKey, string start, string end)
		{
			var confirmationResult = await this.DialogService.Confirm("How would you like to provide your VAT information?", "", new ConfirmOptions { OkButtonText = "EXCEL BRIDGING", CancelButtonText = "ACCOUNTING RECORDS" });
			if (confirmationResult == true) // excel bridging
			{
				NavigationManager.NavigateTo("/excel/" + periodKey + "/" + start + "/" + end);

			}
			else // accounting records
			{

			}
		}

		protected async System.Threading.Tasks.Task Button0Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
		{
			NavigationManager.NavigateTo("/Account/UserRestrictedCall");
		}

		/// <summary>
		/// 
		/// </summary>

		bool showDataLabels = true;

		class DataItem
		{
			public string Quarter { get; set; }
			public double Revenue { get; set; }
		}

		DataItem[] revenue = new DataItem[] {
			new DataItem
			{
				Quarter = "Q1",
				Revenue = 30000
				},
			new DataItem
			{
				Quarter = "Q2",
				Revenue = 40000
			},
			new DataItem
			{
				Quarter = "Q3",
				Revenue = 50000
			},
			new DataItem
			{
				Quarter = "Q4",
				Revenue = 80000
			},
		};

		public class Test
		{
			public int TaxYear { get; set; }
		}

		Test test = new Test()
		{

		};

		public class TaxYear
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		List<TaxYear> taxYears = new List<TaxYear>()
		{
		new TaxYear() { Id = 1, Name = "2023/24" },
		new TaxYear() { Id = 2, Name = "2022/23" }
		};
	}
}