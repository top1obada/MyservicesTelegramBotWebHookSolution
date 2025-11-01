using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServicesTelegramBot.HelpLibrary
{
    public static class clsCallBackDataHelp
    {

        public static string ?GetCallBackDataName(string CallBack)
        {
            var Index = CallBack.IndexOf(':');

            if(Index != -1)
            {
                return CallBack.Substring(Index + 1, CallBack.Length - Index - 1);
            }

            return null;

        }

        public static int? GetCallBackDataID(string CallBack)
        {

            var Index = CallBack.IndexOf(':');

            if (Index != -1) 
            {
                return Convert.ToInt32(CallBack.Substring(0, Index));
            }

            return null;

        }

    }
}
