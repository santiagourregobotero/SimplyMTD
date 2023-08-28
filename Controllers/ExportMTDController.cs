using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using SimplyMTD.Data;

namespace SimplyMTD.Controllers
{
    public partial class ExportMTDController : ExportController
    {
        private readonly MTDContext context;
        private readonly MTDService service;

        public ExportMTDController(MTDContext context, MTDService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/MTD/userdetails/csv")]
        [HttpGet("/export/MTD/userdetails/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserDetailsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserDetails(), Request.Query), fileName);
        }

        [HttpGet("/export/MTD/userdetails/excel")]
        [HttpGet("/export/MTD/userdetails/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserDetailsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserDetails(), Request.Query), fileName);
        }
    }
}
