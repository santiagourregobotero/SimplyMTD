using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using SimplyMTD.Models;

namespace SimplyMTD.Pages
{
    public partial class Hmrc
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected SimplyMTD.Models.ApplicationUser user;

        
    }
}