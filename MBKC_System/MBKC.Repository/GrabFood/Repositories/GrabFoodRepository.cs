using MBKC.Repository.GrabFood.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<GrabFoodAuthenticationResponse> LoginGrabFoodAsync(GrabFoodAccount account)
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

        public async Task<GrabFoodMenu> GetGrabFoodMenuAsync(GrabFoodAuthenticationResponse grabFoodAuthentication)
        {
            try
            {
                GrabFoodAPI grabFoodAPI = GetGrabFoodAPI();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(grabFoodAPI.MenusURI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Merchantid", grabFoodAuthentication.Data.User_Profile.Grab_Food_Entity_Id);
                client.DefaultRequestHeaders.Add("Authorization", grabFoodAuthentication.Data.Data.JWT);
                client.DefaultRequestHeaders.Add("Requestsource", grabFoodAPI.RequestSource);
                HttpResponseMessage response = await client.GetAsync("");
                string responseText = await response.Content.ReadAsStringAsync();
                GrabFoodMenu grabFoodMenu = JsonConvert.DeserializeObject<GrabFoodMenu>(responseText);
                return grabFoodMenu;
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
