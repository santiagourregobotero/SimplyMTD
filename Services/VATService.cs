using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using SimplyMTD.Data;
using SimplyMTD.Models;
using System.Net.Http.Headers;

namespace SimplyMTD
{
	public partial class VATService
	{
		private readonly IConfiguration configuration;
		private readonly TokenProvider _store;
		private readonly string token;

        public async Task<bool> HmrcAsync()
        {
            if (this.token != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public VATService(TokenProvider tokenProvider, IConfiguration configuration)
		{
			this._store = tokenProvider;
			this.configuration = configuration;
			//this.token = _store.AccessToken;
			this.token = "c532c9864b154d8703e2b548ceff8eab";
		}

		public async Task<List<Obligation>> GetObligations()
		{
			var token = this.token; // Todo

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(configuration.GetValue<string>("Auth0:uri"));
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				HttpResponseMessage response = await client.GetAsync("organisations/vat/423439757/obligations?from=2023-01-01&to=2023-01-04&status=O");

				String resp = await response.Content.ReadAsStringAsync();
				ObligationBody obligationBody = JsonConvert.DeserializeObject<ObligationBody>(resp);
				return obligationBody.obligations;
			}
		}

		public async Task<VATReturn> GetObligation(string periodKey)
		{
			var token = this.token; // Todo

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(configuration.GetValue<string>("Auth0:uri"));
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				HttpResponseMessage response = await client.GetAsync("organisations/vat/423439757/returns/" + periodKey);

				String resp = await response.Content.ReadAsStringAsync();
				VATReturn obligation = JsonConvert.DeserializeObject<VATReturn>(resp);
				return obligation;
			}
		}

		public async Task<List<Liability>> GetLiabilities()
		{
			var token = this.token; // Todo

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(configuration.GetValue<string>("Auth0:uri"));
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
				client.DefaultRequestHeaders.Add("Gov-Test-Scenario", "MULTIPLE_LIABILITIES");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				HttpResponseMessage response = await client.GetAsync("organisations/vat/423439757/liabilities?from=2023-01-01&to=2023-01-04");

				String resp = await response.Content.ReadAsStringAsync();
				LiabilityContainer liabilityContainer = JsonConvert.DeserializeObject<LiabilityContainer>(resp);
				return liabilityContainer.liabilities;
			}
		}

		public async Task<List<Payment>> GetPayments()
		{
			var token = this.token; // Todo

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(configuration.GetValue<string>("Auth0:uri"));
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
				client.DefaultRequestHeaders.Add("Gov-Test-Scenario", "MULTIPLE_PAYMENTS");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				HttpResponseMessage response = await client.GetAsync("organisations/vat/423439757/payments?from=2017-01-25&to=2017-06-25");

				String resp = await response.Content.ReadAsStringAsync();
				PaymentContainer paymentContainer = JsonConvert.DeserializeObject<PaymentContainer>(resp);
				return paymentContainer.payments;
			}
		}

		public async Task<bool> submitVAT(VATReturn vatReturn)
		{
            var token = this.token; // Todo
			 
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(configuration.GetValue<string>("Auth0:uri"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				vatReturn.finalised = true;

                var response = await client.PostAsJsonAsync("organisations/vat/423439757/returns", vatReturn);
				// Todo: customize
				if((int)response.StatusCode == 201)
				{
					return true;
				} else
				{
					return false;
				}
				
				

            }
        }
    }
}
