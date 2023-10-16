using MBKC.Repository.GrabFood.Models;
using MBKC.Repository.SMTPs.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MBKC.Repository.GrabFood.Repositories
{
    public class GrabFoodRepository
    {
        public GrabFoodRepository()
        {

        }

        private GrabFoodAPI GetGrabFoodAPI()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            return new GrabFoodAPI()
            {
                AuthenticationURI = configuration.GetSection("GrabFood:API:AuthenticationURI").Value,
                MenusURI = configuration.GetSection("GrabFood:API:MenusURI").Value,
                StoresURI = configuration.GetSection("GrabFood:API:StoresURI").Value,
                RequestSource = configuration.GetSection("GrabFood:API:RequestSource").Value,
            };
        }

        public async Task<GrabFoodAuthenticationResponse> LoginGrabFood(GrabFoodAccount account)
        {
            try
            {
                GrabFoodAPI grabFoodAPI = GetGrabFoodAPI();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(grabFoodAPI.AuthenticationURI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync("", account);
                string responseText = await response.Content.ReadAsStringAsync();
                GrabFoodAuthenticationResponse grabFoodAuthenticationResponse = JsonConvert.DeserializeObject<GrabFoodAuthenticationResponse>(responseText);
                if (response.IsSuccessStatusCode)
                {
                    return grabFoodAuthenticationResponse;
                }
                throw new Exception($"{grabFoodAuthenticationResponse.Error.Msg} for GrabFood Partner.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GrabFoodStoreResponse> GetGrabFoodStores(string jwt)
        {
            try
            {
                GrabFoodAPI grabFoodAPI = GetGrabFoodAPI();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(grabFoodAPI.StoresURI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", jwt);
                client.DefaultRequestHeaders.Add("Requestsource", grabFoodAPI.RequestSource);
                HttpResponseMessage response = await client.GetAsync("");
                string responseText = await response.Content.ReadAsStringAsync();
                GrabFoodStoreResponse grabFoodStoreResponse = JsonConvert.DeserializeObject<GrabFoodStoreResponse>(responseText);
                return grabFoodStoreResponse;
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GetGrabFoodProducts(string jwt, string merchantGroupId, string merchantId)
        {
            try
            {
                GrabFoodAPI grabFoodAPI = GetGrabFoodAPI();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(grabFoodAPI.MenusURI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", jwt);
                client.DefaultRequestHeaders.Add("Requestsource", grabFoodAPI.RequestSource);
                client.DefaultRequestHeaders.Add("Merchantgroupid", merchantGroupId);
                client.DefaultRequestHeaders.Add("Merchantid", merchantId);
                HttpResponseMessage response = await client.GetAsync("");
                string responseText = await response.Content.ReadAsStringAsync();
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
