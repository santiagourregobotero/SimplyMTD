using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace SimplyMTD.Pages
{
    public partial class EditUserDetail
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
        [Inject]
        public MTDService MTDService { get; set; }

        [Parameter]
        public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            userDetail = await MTDService.GetUserDetailById(Id);
        }
        protected bool errorVisible;
        protected SimplyMTD.Models.MTD.UserDetail userDetail;

        protected async Task FormSubmit()
        {
            try
            {
                await MTDService.UpdateUserDetail(Id, userDetail);
                DialogService.Close(userDetail);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
           MTDService.Reset();
            hasChanges = false;
            canEdit = true;

            userDetail = await MTDService.GetUserDetailById(Id);
        }

		protected async Task InformClient(MouseEventArgs args)
		{
			await DialogService.OpenAsync<InformClient>("Inform Client", new Dictionary<string, object> { { "Id", Id } }, new DialogOptions() { Width = "700px", Resizable = true, Draggable = true });
		}
	}
}