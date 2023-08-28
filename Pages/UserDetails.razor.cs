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
    public partial class UserDetails
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

        protected IEnumerable<SimplyMTD.Models.MTD.UserDetail> userDetails;

        protected RadzenDataGrid<SimplyMTD.Models.MTD.UserDetail> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            userDetails = await MTDService.GetUserDetails(new Query { Filter = $@"i => i.UserId.Contains(@0) || i.Vrn.Contains(@0) || i.BusinessName.Contains(@0) || i.OwnerName.Contains(@0) || i.Address.Contains(@0) || i.Address2.Contains(@0) || i.PostCode.Contains(@0) || i.Nino.Contains(@0) || i.BusinessType.Contains(@0) || i.Photo.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            userDetails = await MTDService.GetUserDetails(new Query { Filter = $@"i => i.UserId.Contains(@0) || i.Vrn.Contains(@0) || i.BusinessName.Contains(@0) || i.OwnerName.Contains(@0) || i.Address.Contains(@0) || i.Address2.Contains(@0) || i.PostCode.Contains(@0) || i.Nino.Contains(@0) || i.BusinessType.Contains(@0) || i.Photo.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddUserDetail>("Add UserDetail", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<SimplyMTD.Models.MTD.UserDetail> args)
        {
            await DialogService.OpenAsync<EditUserDetail>("Edit UserDetail", new Dictionary<string, object> { {"Id", args.Data.Id} }, new DialogOptions() { Width = "1000px", Resizable = true, Draggable = true });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, SimplyMTD.Models.MTD.UserDetail userDetail)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await MTDService.DeleteUserDetail(userDetail.Id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                { 
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error", 
                    Detail = $"Unable to delete UserDetail" 
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await MTDService.ExportUserDetailsToCSV(new Query
{ 
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", 
    OrderBy = $"{grid0.Query.OrderBy}", 
    Expand = "", 
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible()).Select(c => c.Property))
}, "UserDetails");
            }

            if (args == null || args.Value == "xlsx")
            {
                await MTDService.ExportUserDetailsToExcel(new Query
{ 
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}", 
    OrderBy = $"{grid0.Query.OrderBy}", 
    Expand = "", 
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible()).Select(c => c.Property))
}, "UserDetails");
            }
        }
    }
}