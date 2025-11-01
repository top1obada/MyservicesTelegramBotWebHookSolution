using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MyServicesTelegramBotApiConnect.Setting;
using MyServicesTelegramBotDTO.ObjectsDTO.ServicesDTO;
namespace MyServicesTelegramBotApiConnect.Connect.Service
{
    public static class clsServiceConnect
    {

        public static async Task<Tuple<List<clsServiceDTO>?, string?>> GetAllservices()
        {

            try
            {

                var Result = await clsClient.Client.GetAsync("Service/GetAll");

                if (Result.IsSuccessStatusCode)
                {
                    var Services = await Result.Content.ReadFromJsonAsync<List<clsServiceDTO>>();

                    return new Tuple<List<clsServiceDTO>?, string?>(Services, null);
                }

                return new Tuple<List<clsServiceDTO>?, string?>(null, await Result.Content.ReadAsStringAsync());

            }

            catch (Exception ex)
            {
                return new Tuple<List<clsServiceDTO>?, string?>(null, ex.Message);
            }

        }

    }
}
