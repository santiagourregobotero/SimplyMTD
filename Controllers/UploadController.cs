using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimplyMTD.Models;
using System;
using System.Security.Claims;
using System.Text;

namespace SimplyMTD.Controllers
{
    public partial class UploadController : Controller
    {
        private readonly IWebHostEnvironment environment;

		private readonly UserManager<ApplicationUser> userManager;

		private readonly MTDService _MTDService;

		protected SimplyMTD.Models.MTD.UserDetail userDetail;

		public UploadController(IWebHostEnvironment environment, UserManager<ApplicationUser> userManager, MTDService mtdService)
        {
            this.environment = environment;
			this.userManager = userManager;
			this._MTDService = mtdService;
		}

        // Single file upload
        [HttpPost("upload/single")]
        public IActionResult Single(IFormFile file)
        {
            try
            {
                // Put your code here
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var fullFileName = Path.Combine(environment.WebRootPath, fileName);
                using (var stream = new FileStream(fullFileName, FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);
                }

                List<string> amounts = new List<string>();

                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fullFileName, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    string text;
                    foreach (Row r in sheetData.Elements<Row>())
                    {
                        string xxx = r.Elements<Cell>().ElementAt(1).CellValue.Text;
                        amounts.Add(xxx);
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            text = c.CellValue.Text;
                            Console.Write(text + " ");
                        }
                    }
                    /*Console.WriteLine();
                    Console.ReadKey();*/
                }
                // Delete file
                System.IO.File.Delete(fullFileName);

                return Ok(amounts);

                //return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload
        [HttpPost("upload/multiple")]
        public IActionResult Multiple(IFormFile[] files)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload with parameter
        [HttpPost("upload/{id}")]
        public IActionResult Post(IFormFile[] files, int id)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Image file upload (used by HtmlEditor components)
        [HttpPost("upload/image")]
        public async Task<IActionResult> Image(IFormFile file)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(environment.WebRootPath, fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = Url.Content($"~/{fileName}");
					string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
					userDetail = await _MTDService.GetUserDetailByUserId(userId);

					userDetail.Photo = fileName;
                    await _MTDService.UpdateUserDetail(userDetail.Id, userDetail);
					return Ok(new { Url = fileName });
				}
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
