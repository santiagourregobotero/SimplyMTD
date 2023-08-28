using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using Radzen;

using SimplyMTD.Models;
using Microsoft.AspNetCore.Mvc;

namespace SimplyMTD
{
    public partial class SecurityService
    {
        private readonly HttpClient httpClient;

        private readonly Uri baseUri;

        private readonly NavigationManager navigationManager;

        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUser User { get; private set; } = new ApplicationUser { Name = "Anonymous" };

        public ClaimsPrincipal Principal { get; private set; }

        public SecurityService(UserManager<ApplicationUser> userManager, NavigationManager navigationManager, IHttpClientFactory factory)
        {
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/Identity/");
            this.httpClient = factory.CreateClient("SimplyMTD");
            this.navigationManager = navigationManager;
            this.userManager = userManager;
        }

        public bool IsInRole(params string[] roles)
        {
#if DEBUG
            if (User.Name == "admin")
            {
                return true;
            }
#endif

            if (roles.Contains("Everybody"))
            {
                return true;
            }

            if (!IsAuthenticated())
            {
                return false;
            }

            if (roles.Contains("Authenticated"))
            {
                return true;
            }

            return roles.Any(role => Principal.IsInRole(role));
        }

        public bool IsAuthenticated()
        {
            return Principal?.Identity.IsAuthenticated == true;
        }

        public async Task<bool> InitializeAsync(AuthenticationState result)
        {
            Principal = result.User;
#if DEBUG
            if (Principal.Identity.Name == "admin")
            {
                User = new ApplicationUser { Name = "Admin" };

                return true;
            }
#endif
            var userId = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && User?.Id != userId)
            {
                User = await GetUserById(userId);
            }

            return IsAuthenticated();
        }

        public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
        {
            var uri =  new Uri($"{navigationManager.BaseUri}Account/CurrentUser");

            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri));

            return await response.ReadAsync<ApplicationAuthenticationState>();
        }

        public void Logout()
        {
            navigationManager.NavigateTo("Account/Logout", true);
        }

        public void Login()
        {
            navigationManager.NavigateTo("Login", true);
        }

        public async Task<IEnumerable<ApplicationRole>> GetRoles()
        {
            var uri = new Uri(baseUri, $"ApplicationRoles");

            uri = uri.GetODataUri();

            var response = await httpClient.GetAsync(uri);

            var result = await response.ReadAsync<ODataServiceResult<ApplicationRole>>();

            return result.Value;
        }

        public async Task<ApplicationRole> CreateRole(ApplicationRole role)
        {
            var uri = new Uri(baseUri, $"ApplicationRoles");

            var content = new StringContent(ODataJsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);

            return await response.ReadAsync<ApplicationRole>();
        }

        public async Task<HttpResponseMessage> DeleteRole(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationRoles('{id}')");

            return await httpClient.DeleteAsync(uri);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            var uri = new Uri(baseUri, $"ApplicationUsers");

            uri = uri.GetODataUri();

            var response = await httpClient.GetAsync(uri);

            var result = await response.ReadAsync<ODataServiceResult<ApplicationUser>>();

            return result.Value;
        }

        public async Task<ApplicationUser> CreateUser(ApplicationUser user)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers");

            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);

            return await response.ReadAsync<ApplicationUser>();
        }

        public async Task<HttpResponseMessage> DeleteUser(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')");

            return await httpClient.DeleteAsync(uri);
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')?$expand=Roles");

            var response = await httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.ReadAsync<ApplicationUser>();
        }
        private void EnsureSucceeded(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(error => error.Description));

                throw new ApplicationException(message);
            }
        }

        public async Task<bool> UpdateUser1(ApplicationUser user)
        {
            ApplicationUser user1 = await userManager.FindByIdAsync(user.Id);

           /* user1.BusinessName = user.BusinessName;
			user1.Nino = user.Nino;
			user1.Vrn = user.Vrn;
			user1.Address = user.Address;
            user1.PhoneNumber = user.PhoneNumber;*/

			var result = await userManager.UpdateAsync(user1);
			if (!result.Succeeded)
            {
                return false;
            } else
            {
                return true;
            }
				
        }

		public async Task<bool> UpdateUser2(ApplicationUser user)
		{
			ApplicationUser user1 = await userManager.FindByIdAsync(user.Id);

			/*user1.Start = user.Start;
			user1.End = user.End;
			user1.Deadline = user.Deadline;*/

			var result = await userManager.UpdateAsync(user1);
			if (!result.Succeeded)
			{
				return false;
			}
			else
			{
				return true;
			}

		}

		public async Task<ApplicationUser> UpdateUser(string id, ApplicationUser user)
        {
            /*var roles = user.Roles.ToArray();

            var result = await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));

            EnsureSucceeded(result);

            if (roles.Any())
            {
                result = await userManager.AddToRolesAsync(user, roles);

                EnsureSucceeded(result);
            }*/

            /*var result = await userManager.UpdateAsync(user);

            EnsureSucceeded(result);

            if (!String.IsNullOrEmpty(user.Password) && user.Password == user.ConfirmPassword)
            {
                result = await userManager.RemovePasswordAsync(user);

                EnsureSucceeded(result);

                result = await userManager.AddPasswordAsync(user, user.Password);

                EnsureSucceeded(result);
            }

            return user;*/

            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')");
            var test = JsonSerializer.Serialize(user);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response.ReadAsync<ApplicationUser>();
        }

        public async Task ChangePassword(string oldPassword, string newPassword)
        {
            var uri =  new Uri($"{navigationManager.BaseUri}Account/ChangePassword");

            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "oldPassword", oldPassword },
                { "newPassword", newPassword }
            });

            var response = await httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();

                throw new ApplicationException(message);
            }
        }

        public async Task Register(string userName, string password)
        {
            var uri =  new Uri($"{navigationManager.BaseUri}Account/Register");

            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "userName", userName },
                { "password", password }
            });

            var response = await httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();

                throw new ApplicationException(message);
            }
        }

        public async Task ResetPassword(string userName)
        {
            var uri =  new Uri($"{navigationManager.BaseUri}Account/ResetPassword");

            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "userName", userName }
            });

            var response = await httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();

                throw new ApplicationException(message);
            }
        }
    }
}