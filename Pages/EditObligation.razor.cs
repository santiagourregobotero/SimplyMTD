using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
using Microsoft.JSInterop;
using Microsoft.OData;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using SimplyMTD.Models;
using static System.Net.WebRequestMethods;

namespace SimplyMTD.Pages
{
	public partial class EditObligation
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

		protected IEnumerable<Obligation> obligations;

		protected IEnumerable<Liability> liabilities;

		protected IEnumerable<Payment> payments;

		protected RadzenDataGrid<Obligation> grid1;

		protected RadzenDataGrid<Liability> grid2;

		protected RadzenDataGrid<Payment> grid3;

		[Parameter]
		public string PeriodKey { get; set; }

		protected VATReturn obligation;

		protected override async Task OnInitializedAsync()
		{
			await Load();
		}

		protected async System.Threading.Tasks.Task Load()
		{
			obligation = await VATService.GetObligation(PeriodKey);

		}
	}
}