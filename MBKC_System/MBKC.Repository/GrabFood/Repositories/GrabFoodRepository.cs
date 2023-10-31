using MBKC.Repository.GrabFood.Models;
using MBKC.Repository.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
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
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, grabFoodAPI.AuthenticationURI);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
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
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, grabFoodAPI.MenusURI);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("Merchantid", grabFoodAuthentication.Data.Data.User_Profile.Grab_Food_Entity_Id);
                request.Headers.Add("Authorization", grabFoodAuthentication.Data.Data.JWT);
                request.Headers.Add("Requestsource", grabFoodAPI.RequestSource);
                ProductHeaderValue header = new ProductHeaderValue("MBKCApplication", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                ProductInfoHeaderValue userAgent = new ProductInfoHeaderValue(header);
                client.DefaultRequestHeaders.UserAgent.Add(userAgent);
                HttpResponseMessage response = await client.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();
                GrabFoodMenu grabFoodMenu = JsonConvert.DeserializeObject<GrabFoodMenu>(responseText);
                return grabFoodMenu;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
