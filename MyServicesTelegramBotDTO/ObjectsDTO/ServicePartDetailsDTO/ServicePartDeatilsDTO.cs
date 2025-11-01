using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServicesTelegramBotDTO.ObjectsDTO.ServicePartDetailsDTO
{
    public class clsServicePartDetailsDTO
    {
        public int? ServicePartDetailsID { get; set; }
        public int? ServicePartID { get; set; }
        public string? Title { get; set; }
        public short? WorkTimePerDays { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string? Text_Description { get; set; }
    }
}