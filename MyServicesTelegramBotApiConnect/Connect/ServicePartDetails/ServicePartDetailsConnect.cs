using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MyServicesTelegramBotApiConnect.Setting;
using MyServicesTelegramBotDTO.ObjectsDTO.ServicePartDetailsDTO;
using MyServicesTelegramBotDTO.ObjectsDTO.ServicePartDTO;

namespace MyServicesTelegramBotApiConnect.Connect.ServicePartDetails
{
    public static class clsServicePartDetailsConnect
    {
        public static async Task<Tuple<List<clsServicePartDetailsTitleDTO>?, string?>> GetServicePartDetailsTitles(int ServicePartID)
        {
            try
            {
                var Result = await clsClient.Client.GetAsync($"ServicePartDetails/GetServicePartDetailsTitles/{ServicePartID}");

                if (Result.IsSuccessStatusCode)
                {
                    var Details = await Result.Content.ReadFromJsonAsync<List<clsServicePartDetailsTitleDTO>>();

                    return new Tuple<List<clsServicePartDetailsTitleDTO>?, string?>(Details, null);
                }

                return new Tuple<List<clsServicePartDetailsTitleDTO>?, string?>(null, await Result.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return new Tuple<List<clsServicePartDetailsTitleDTO>?, string?>(null, ex.Message);
            }
        }


        public static async Task<Tuple<clsServicePartDetailsDTO?, string?>> GetServicePartDetails(int ServicePartDetailsID)
        {
            try
            {
                var Result = await clsClient.Client.GetAsync($"ServicePartDetails/GetServicePartDetails/{ServicePartDetailsID}");

                if (Result.IsSuccessStatusCode)
                {
                    var Details = await Result.Content.ReadFromJsonAsync<clsServicePartDetailsDTO>();

                    return new Tuple<clsServicePartDetailsDTO?, string?>(Details, null);
                }

                return new Tuple<clsServicePartDetailsDTO?, string?>(null, await Result.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return new Tuple<clsServicePartDetailsDTO?, string?>(null, ex.Message);
            }
        }
    }
}


