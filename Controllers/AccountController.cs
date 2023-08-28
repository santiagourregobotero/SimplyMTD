using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimplyMTD.Models;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Net.Http.Headers;
using SimplyMTD.Models.MTD;

namespace SimplyMTD.Controllers
{
    [Route("Account/[action]")]
    public partial class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
		private readonly MTDService _MTDService;

		// protected SimplyMTD.Models.MTD.UserDetail userDetail;

		// public MTDService MTDService { get; set; }

		public AccountController(IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IConfiguration configuration, MTDService mtdService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.env = env;
            this.configuration = configuration;
            this._MTDService = mtdService;

		}

        private IActionResult RedirectWithError(string error, string redirectUrl = null)
        {
             if (!string.IsNullOrEmpty(redirectUrl))
             {
                 return Redirect($"~/Login?error={error}&redirectUrl={Uri.EscapeDataString(redirectUrl)}");
             }
             else
             {
                 return Redirect($"~/Login?error={error}");
             }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string redirectUrl)
        {
            if (env.EnvironmentName == "Development" && userName == "admin" && password == "admin")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin")
                };

                roleManager.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
                await signInManager.SignInWithClaimsAsync(new ApplicationUser { UserName = userName, Email = userName }, isPersistent: false, claims);

                return Redirect($"~/{redirectUrl}");
            }

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {

                var user = await userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    return RedirectWithError("Invalid user or password", redirectUrl);
                }

                if (!user.EmailConfirmed)
                {
                    return RedirectWithError("User email not confirmed", redirectUrl);
                }
                var result = await signInManager.PasswordSignInAsync(userName, password, false, false);

                if (result.Succeeded)
                {
                    return Redirect($"~/{redirectUrl}");
                }
            }

            return RedirectWithError("Invalid user or password", redirectUrl);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Invalid password");
            }

            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await userManager.FindByIdAsync(id);

            var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (result.Succeeded)
            {
                return Ok();
            }

            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(message);
        }

        [HttpPost]
        public ApplicationAuthenticationState CurrentUser()
        {
            return new ApplicationAuthenticationState
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name,
                Claims = User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
            };
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return Redirect("~/");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Invalid user name or password.");
            }
			string startTime = "1/06/2010";
			var seconds = (DateTime.Now - DateTime.Parse(startTime)).TotalSeconds;
            var clientId = Convert.ToInt32(seconds % 100000000);
			var user = new ApplicationUser { UserName = userName, Email = userName };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                try
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Scheme);

                    var body = string.Format(@"<a href=""{0}"">{1}</a>", callbackUrl, "Please confirm your registration");

                    await SendEmailAsync(user.Email, "Confirm your registration", body);

                    var userDetail = new UserDetail { UserId = user.Id, ClientId =  clientId};
					await _MTDService.CreateUserDetail(userDetail);

					var accountant = new Accountant { UserId = user.Id };
					await _MTDService.CreateAccountant(accountant);

					var accounting = new Accounting { UserId = user.Id };
					await _MTDService.CreateAccounting(accounting);

					var billing = new Billing { UserId = user.Id };
					await _MTDService.CreateBilling(billing);

					return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(message);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);

            var result = await userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return Redirect("~/Login?info=Your registration has been confirmed");
            }

            return RedirectWithError("Invalid user or confirmation code");
        }

        public async Task<IActionResult> ResetPassword(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest("Invalid user name.");
            }

            try
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmPasswordReset", "Account", new { userId = user.Id, code }, protocol: Request.Scheme);

                var body = string.Format(@"<a href=""{0}"">{1}</a>", callbackUrl, "Please confirm your password reset.");

                await SendEmailAsync(user.Email, "Confirm your password reset", body);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> ConfirmPasswordReset(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Redirect("~/Login?error=Invalid user");
            }

            var password = GenerateRandomPassword();

            var result = await userManager.ResetPasswordAsync(user, code, password);

            if (result.Succeeded)
            {
                await SendEmailAsync(user.Email, "New password", $"<p>Your new password is: {password}</p><p>Please change it after login.</p>");

                return Redirect("~/Login?info=Password reset successful. You will receive an email with your new password.");
            }

            return Redirect("~/Login?error=Invalid user or confirmation code");
        }

        private static string GenerateRandomPassword()
        {
            var options = new PasswordOptions
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            var randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",
                "abcdefghijkmnopqrstuvwxyz",
                "0123456789",
                "!@$?_-"
            };

            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (options.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);
            }

            if (options.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);
            }

            if (options.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);
            }

            if (options.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);
            }

            for (int i = chars.Count; i < options.RequiredLength || chars.Distinct().Count() < options.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            string sendGridApiKey = configuration.GetValue<string>("Sendgrid:API_KEY");
            if (string.IsNullOrEmpty(sendGridApiKey))
            {
                throw new Exception("The 'SendGridApiKey' is not configured");
            }

            var client = new SendGridClient(sendGridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(configuration.GetValue<string>("Sendgrid:FROM_EMAIL"), subject),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = body
            };
            msg.AddTo(new EmailAddress(to));

            var response = await client.SendEmailAsync(msg);

            /*var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(configuration.GetValue<string>("Smtp:User"));
            mailMessage.Body = body;
            mailMessage.Subject = subject;
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(to);

            var client = new System.Net.Mail.SmtpClient(configuration.GetValue<string>("Smtp:Host"))
            {
                UseDefaultCredentials = false,
                EnableSsl = configuration.GetValue<bool>("Smtp:Ssl"),
                Port = configuration.GetValue<int>("Smtp:Port"),
                Credentials = new System.Net.NetworkCredential(configuration.GetValue<string>("Smtp:User"), configuration.GetValue<string>("Smtp:Password"))
            };

            await client.SendMailAsync(mailMessage);*/
        }

        public async Task<IActionResult> UserRestrictedCall(string key, string vrn)
        {           
            if(key != null && vrn != null)
            {
				ApplicationUser user = await userManager.FindByIdAsync(key);

				if (user == null)
				{
					return NotFound();
				}

/*				user.Vrn = vrn;
*/				await userManager.UpdateAsync(user);
			}
            

            string accessToken = await HttpContext.GetTokenAsync(IdentityConstants.ExternalScheme, "access_token");

            if (accessToken != null)
            {
                return Redirect($"~/hmrc-success");
               
            }
            else
            {
                return Challenge(new AuthenticationProperties() { RedirectUri = "/Account/UserRestrictedCall" }, "HMRC");
            }
        }



	}
}