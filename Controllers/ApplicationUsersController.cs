using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using SimplyMTD.Data;
using SimplyMTD.Models;

namespace SimplyMTD.Controllers
{
    [Authorize]
    [Route("odata/Identity/ApplicationUsers")]
    public partial class ApplicationUsersController : ODataController
    {
        private readonly ApplicationIdentityDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUsersController(ApplicationIdentityDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        partial void OnUsersRead(ref IQueryable<ApplicationUser> users);

        [EnableQuery]
        [HttpGet]
        public IEnumerable<ApplicationUser> Get()
        {
            var users = userManager.Users;

            OnUsersRead(ref users);

            return users;
        }

        [EnableQuery]
        [HttpGet("{Id}")]
        public SingleResult<ApplicationUser> GetApplicationUser(string key)
        {
            var user = context.Users.Where(i => i.Id == key).Include(i => i.UserDetail);

            return SingleResult.Create(user);
        }

        partial void OnUserDeleted(ApplicationUser user);

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string key)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserDeleted(user);

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        partial void OnUserUpdated(ApplicationUser user);

        [HttpPatch("{Id}")]
        public async Task<IActionResult> Patch(string key, [FromBody] ApplicationUser data)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserUpdated(user);

            IdentityResult result = null;

            if (data.Roles != null)
            {
                var roles = await userManager.GetRolesAsync(user);

                result = await userManager.RemoveFromRolesAsync(user, roles);

                if (result.Succeeded)
                {
                    result = await userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
                }
            }

            if (!string.IsNullOrEmpty(data.Password))
            {
                result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, data.Password);
                }
            }

            if (result != null && !result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        partial void OnUserCreated(ApplicationUser user);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApplicationUser data)
        {
            var email = data.Email;
            var password = data.Password;

            var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };

            IdentityResult result = await userManager.CreateAsync(user, password);

            if (result.Succeeded && data.Roles != null)
            {
                result = await userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
            }

            if (result.Succeeded)
            {
                OnUserCreated(user);

                return Created($"odata/Identity/Users('{user.Id}')", user);
            }
            else
            {
                return IdentityError(result);
            }
        }

        private IActionResult IdentityError(IdentityResult result)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(new { error = new { message } });
        }
    }
}