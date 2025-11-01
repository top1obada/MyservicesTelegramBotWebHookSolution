using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using MyServicesTelegramBot.FormParts;
using MyServicesTelegramBot.HelpLibrary;

namespace MyServicesTelegramBot.TelegramBot
{
    public class clsTelegramBot
    {

        private TelegramBotClient _Bot;

        private Dictionary<long, string> _userLanguages = new Dictionary<long, string>();

        private Dictionary<long, int> _userPreviousMessage = new Dictionary<long, int>();

        private Dictionary<string, int> _usersPartsIDs = new Dictionary<string, int>();


        private delegate Task PreviousAction(Update u);

        private Dictionary<long, PreviousAction> _usersPreviousAction = new Dictionary<long, PreviousAction>();
        public clsTelegramBot(string Token)
        {
            _Bot = new TelegramBotClient(Token);
        }

        public async Task Play(Update update)
        {


            await _HandleUpdateAsync(update);

        }


        private async Task _ShowLanguages(Update update)
        {
            long ChatID;

            if(update.Message == null)
            {
                ChatID = update.CallbackQuery.Message.Chat.Id;
            }
            else
            {
                ChatID = update.Message.Chat.Id;
            }

            

            if (_userPreviousMessage.ContainsKey(ChatID))
            {
                await _Bot.DeleteMessageAsync(ChatID, _userPreviousMessage[ChatID]);
            }

            var Message = await LanguagesPart.ShowLanguages(_Bot, ChatID);

            _userPreviousMessage[ChatID] = Message.MessageId;

        }

        private async Task _ShowServices(Update update)
        {

            var callback = update.CallbackQuery;

            var ChatID = callback.Message.Chat.Id;

            

            if(callback.Data == "Back" || callback.Data == "MainMenu")
            {
                _usersPartsIDs.Remove($"{ChatID}:Service");
            }
            else
            {
                _userLanguages[ChatID] = callback.Data.Substring(0, callback.Data.IndexOf(':'));
            }

            await _Bot.DeleteMessageAsync(ChatID, _userPreviousMessage[ChatID]);

            var Message = await clsServicesPart.ShowServices(_Bot, ChatID, _userLanguages[ChatID]);

            if (Message.Item2 != null)
            {
                return;
            }

            _userPreviousMessage[ChatID] = Message.Item1.MessageId;

            _usersPreviousAction[ChatID] = _ShowLanguages;
        }

        private async Task _ShowServicesParts(Update update)
        {

            var callback = update.CallbackQuery;

            var ChatID = callback.Message.Chat.Id;

            await _Bot.DeleteMessageAsync(ChatID, _userPreviousMessage[ChatID]);

            int? ServiceID = null;

            if (callback.Data == "Back")
            {
                ServiceID = _usersPartsIDs[$"{ChatID}:Service"];
                _usersPartsIDs.Remove($"{ChatID}:ServicePart");
            }
            else
            {
                ServiceID = clsCallBackDataHelp.GetCallBackDataID(callback.Data);
                if (ServiceID == null) return;
                _usersPartsIDs[$"{ChatID}:Service"] = (int)ServiceID;
            }
            

            var Message = await clsServicePartsPart.ShowServiceParts((int)ServiceID, _Bot, ChatID, _userLanguages[ChatID]);

            if (Message == null) return;

            _userPreviousMessage[ChatID] = Message.MessageId;

            _usersPreviousAction[ChatID] = _ShowServices;
        }


        private async Task _ShowServicePartDetailsTitles(Update update)
        {

            var callback = update.CallbackQuery;

            var ChatID = callback.Message.Chat.Id;

            await _Bot.DeleteMessageAsync(ChatID, _userPreviousMessage[ChatID]);

            int? ServicePartID = null;

            if (callback.Data == "Back")
            {
                ServicePartID = _usersPartsIDs[$"{ChatID}:ServicePart"];
                _usersPartsIDs.Remove($"{ChatID}:ServicePartDetail");
            }
            else
            {
                ServicePartID = clsCallBackDataHelp.GetCallBackDataID(callback.Data);
                if (ServicePartID == null) return;
                _usersPartsIDs[$"{ChatID}:ServicePart"] = (int)ServicePartID;
            }

            var Message = await clsServicePartDetailsTitlePart.ShowServicePartDetailsTitles((int)ServicePartID, _Bot, ChatID, _userLanguages[ChatID]);

            if(Message == null) return;

            _userPreviousMessage[ChatID] = Message.MessageId;

            _usersPreviousAction[ChatID] = _ShowServicesParts;

        }


        private async Task _ShowServicePartDetails(Update update)
        {

            var callback = update.CallbackQuery;

            var ChatID = callback.Message.Chat.Id;

            await _Bot.DeleteMessageAsync(ChatID, _userPreviousMessage[ChatID]);

            int? ServicePartDetailID = clsCallBackDataHelp.GetCallBackDataID(callback.Data);


            if (ServicePartDetailID == null) return;


            _usersPartsIDs[$"{ChatID}:ServicePartDetail"] = (int)ServicePartDetailID;

            var Message = await clsServicePartDetailsPart.ShowServicePartDetails((int)ServicePartDetailID, _Bot, ChatID
                , _userLanguages[ChatID]);

            _userPreviousMessage[ChatID] = Message.MessageId;

            _usersPreviousAction[ChatID] = _ShowServicePartDetailsTitles;

        }

        private async Task _HandleUpdateAsync(Update update)
        {

            if (update.Message != null)
            {

                if (!_userLanguages.ContainsKey(update.Message.Chat.Id))
                {
                    await _ShowLanguages(update);
                    return;
                }
            }



            if (update.Type == UpdateType.CallbackQuery)
            {

                var callback = update.CallbackQuery;


                if (callback == null || callback.Message == null || callback.Message.Text == null) return;


                if (clsCallBackDataHelp.GetCallBackDataName(callback.Data) == "Language")
                {
                    await _ShowServices(update);

                    return;
                }

                if (clsCallBackDataHelp.GetCallBackDataName(callback.Data) == "Service")
                {
                    await _ShowServicesParts(update);
                    return;
                }

                if(clsCallBackDataHelp.GetCallBackDataName(callback.Data) == "ServicePart")
                {
                    await _ShowServicePartDetailsTitles(update);
                    return;
                }

                if(clsCallBackDataHelp.GetCallBackDataName(callback.Data) == "ServicePartDetailTitle")
                {
                    await _ShowServicePartDetails(update);
                    return;
                }


                if (callback.Data == "Back")
                {

                    await _usersPreviousAction[callback.Message.Chat.Id]?.Invoke(update);


                    return;
                }

                if(callback.Data == "MainMenu")
                {
                    if (_usersPartsIDs.ContainsKey($"{callback.Message.Chat.Id}:ServicePart"))
                    {
                        _usersPartsIDs.Remove($"{callback.Message.Chat.Id}:ServicePart");
                    }

                    if (_usersPartsIDs.ContainsKey($"{callback.Message.Chat.Id}:ServicePartDetail"))
                    {
                        _usersPartsIDs.Remove($"{callback.Message.Chat.Id}:ServicePartDetail");
                    }


                    await _ShowServices(update);

                    return;
                }

                return;
            }

        }
    }
}
