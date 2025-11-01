using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MyServicesTelegramBotApiConnect.Setting;
using MyServicesTelegramBotDTO.ObjectsDTO.ServicePartDTO;
using MyServicesTelegramBotDTO.ObjectsDTO.ServicesDTO;

namespace MyServicesTelegramBotApiConnect.Connect.ServicePart
{
    public static class clsServicePartConnect
    {

        public static async Task<Tuple<List<clsServicePartDTO>?, string?>> GetAllserviceParts(int ServiceID)
        {

            try
            {

                var Result = await clsClient.Client.GetAsync($"ServicePart/GetAllServiceParts/{ServiceID}");

                if (Result.IsSuccessStatusCode)
                {
                    var Services = await Result.Content.ReadFromJsonAsync<List<clsServicePartDTO>>();

                    return new Tuple<List<clsServicePartDTO>?, string?>(Services, null);
                }

                return new Tuple<List<clsServicePartDTO>?, string?>(null, await Result.Content.ReadAsStringAsync());

            }

            catch (Exception ex)
            {
                return new Tuple<List<clsServicePartDTO>?, string?>(null, ex.Message);
            }

        }

    }
}
