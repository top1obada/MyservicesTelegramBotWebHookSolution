using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServicesTelegramBotApiConnect.Setting
{
    public static class clsClient
    {
        //https://localhost:7009/api/

        public static HttpClient Client =
            new HttpClient()
            { BaseAddress = new Uri("http://myserviceapi.runasp.net/api/") };



    }
}
