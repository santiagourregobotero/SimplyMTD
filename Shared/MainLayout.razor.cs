using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using SimplyMTD.Models.MTD;

namespace SimplyMTD.Shared
{
    public partial class MainLayout
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

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

        private bool sidebarExpanded = true;

        [Inject]
        protected SecurityService Security { get; set; }

		protected SimplyMTD.Models.ApplicationUser user;
		[Inject]
		public MTDService MTDService { get; set; }

		void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        protected void Home() 
        {
			NavigationManager.NavigateTo("/", true);
		}

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }

		protected override async Task OnInitializedAsync()
        {
            //user = await Security.GetUserById($"{Security.User.Id}");
			user = await MTDService.GetUserById($"{Security.User.Id}");

            
		}
	}
}
