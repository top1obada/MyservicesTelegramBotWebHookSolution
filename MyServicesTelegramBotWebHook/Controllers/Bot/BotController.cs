using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyServicesTelegramBot.TelegramBot;
using Telegram.Bot.Types;

namespace MyServicesTelegramBotWebHook.Controllers.Bot
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {

        private readonly clsTelegramBot _TelegramBot;

        public BotController()
        {
            _TelegramBot = new clsTelegramBot("8435433205:AAEEtB_s7M2--vDlOFhpphyz3JSL9npg3OM");
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Update update)
        {

            try
            {
                await _TelegramBot.Play(update);

                return Ok();
            }
            catch (Exception ex)
            {  
                return StatusCode(500, ex.Message);
            }

        }


    }
}
