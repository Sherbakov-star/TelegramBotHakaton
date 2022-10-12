using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotLunch2
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            try
            {
                TelegramBotHelper botHelper = new TelegramBotHelper();
                var client = new TelegramBotClient("5729814705:AAHB0zJcy1wrWNYGkOgtUrrmtxritCq8nQU");
                client.StartReceiving(TelegramBotHelper.Update, TelegramBotHelper.Error);

                Console.ReadKey();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

        }

    }
}
