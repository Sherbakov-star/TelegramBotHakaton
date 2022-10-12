using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotLunch2
{

    internal class TelegramBotHelper
    {
        //Buttons
        private const string StartOrder = "Начать ланч";
        private const string Burger = "Бургер";
        private const string Pizza = "Пицца";
        private const string Another = "Другое";
        private const string Sushi = "Суши/Роллы";
        public static bool IsStartOrder = false;
        public const string MakeButton = "Сформировать заказ";
        public const string JoinOrderButton = "Присоединится к заказу";
        //
        public static bool TapBurger = false;
        public static bool TapPizza = false;
        public static bool TapSushi = false;
        public static bool TapAnother = false;

        //
        public static bool HelpForSushi = false;

        public static bool IsOrderSelectVibor = false;
        //Создание заказа
        public static string RestourantName = "";
        public static string Cost = "";
        public static string Number = "";
        //Собранный заказ
        public static string Order = "";
        public static bool IsGetResourantName = false;
        public static bool IsGetCost = false;
        public static bool IsGetNumber = false;

        //
        public static bool IsTap = false;
        //
        public static bool IsOrderDone = false;

        public const string MakeNewOrderButton = "Удалить старый заказ и создать новый";

       
        public async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
           
            var message = update.Message;
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                var text = message.Text;
                if (!IsOrderSelectVibor)
                {
                    Start(botClient, update, token);
                }
                else
                {

                    if (TapSushi)
                    {
                        if (update.Message != null && !IsGetResourantName)
                        {
                            MakeOrder(botClient, update.Message.Chat.Id);
                            TapSushi = false;//delete   
                        }
                    }

                    if (TapBurger)
                    {
                        if (update.Message != null && !IsGetResourantName)
                        {
                            MakeOrder(botClient, update.Message.Chat.Id);
                            TapBurger = false;//delete   
                        }

                    }

                    if (TapPizza)
                    {
                        if (update.Message != null && !IsGetResourantName)
                        {
                            MakeOrder(botClient, update.Message.Chat.Id);
                            TapPizza = false;//delete   
                        }

                    }

                    if (TapAnother)
                    {
                        if (update.Message != null && !IsGetResourantName)
                        {
                            MakeOrder(botClient, update.Message.Chat.Id);
                            TapAnother = false;//delete   
                        }

                    }

                }
                

                switch (text)
                {
                    case StartOrder:
                        IsStartOrder = true;
                        if (!IsOrderSelectVibor)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, " Выберите что хотите кушать ", replyMarkup: Menu());
                        }
                        break;
                    case MakeButton:
                        GetRestourantName(botClient, update);
                        break;
                    case JoinOrderButton:
                        if (Order == "")
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Нет активных заказов");
                        }
                        break;
                    case MakeNewOrderButton:
                        IsStartOrder = false;
                        IsTap = false;
                        IsOrderDone = false;
                        IsGetResourantName = false;
                        IsGetCost = false;
                        IsGetNumber = false;
                        TapBurger = false;
                        TapPizza = false;
                        TapSushi = false;
                        TapAnother = false;
                        HelpForSushi = false;
                        IsOrderSelectVibor = false;
                        break;
                }
                Restourants restourants = new Restourants();

                if (!IsGetResourantName)
                {
                    foreach (var i in restourants.Sushi)
                    {
                        if (i.Contains(text))
                        {
                            RestourantName = text;
                            IsGetResourantName = true;
                            GetCost(botClient, update);
                        }
                        
                    }

                    foreach (var i in restourants.Burger)
                    {
                        if (i.Contains(text))
                        {
                            RestourantName = text;
                            IsGetResourantName = true;
                            GetCost(botClient, update);
                        }
                    }

                    foreach (var i in restourants.Pizza)
                    {
                        if (i.Contains(text))
                        {
                            RestourantName = text;
                            IsGetResourantName = true;
                            GetCost(botClient, update);
                        }
                    }

                    foreach (var i in restourants.Another)
                    {
                        if (i.Contains(text))
                        {
                            RestourantName = text;
                            IsGetResourantName = true;
                            GetCost(botClient, update);
                        }
                    }

                    return;
                }

                if (!IsGetCost)
                {
                    if (IsGetResourantName)
                    {
                        Cost = text;
                        IsGetCost = true;
                        GetNumber(botClient,update);
                    }
                    return;
                }

                if (!IsGetNumber)
                {
                    if (IsGetCost)
                    {
                        Number = text;
                        IsGetNumber = true;
                    }
                    
                }

                if (IsGetResourantName && IsGetNumber && IsGetCost)
                {
                    Order = $"Ресторан: {RestourantName}\n" +
                   $"Стоимость: {Cost}\n" +
                   $"Номер куда скидываться: {Number}";
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Заказ:\n{Order}",replyMarkup:Continue());
                    IsOrderDone = true;
                    return;
                }



            }

            if (update.Type == UpdateType.CallbackQuery)
            {
               await HandleCallbackQuery(botClient, update.CallbackQuery,update); 
            }

        }

       
        public static async void GetRestourantName(ITelegramBotClient botClient,Update update)
        {
           await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Введите название заведения", replyMarkup: new ForceReplyMarkup());
            if (update.Message.ReplyToMessage != null)
            {
                var text = update.Message.ReplyToMessage.Text;
                RestourantName = text;
                IsGetResourantName = true;
            }

            //return;
           
        }

        public static async void GetCost(ITelegramBotClient botClient, Update update)
        {
           await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Введите стоимость в рублях", replyMarkup: new ForceReplyMarkup());
            if (update.Message.ReplyToMessage != null)
            {
                var text = update.Message.ReplyToMessage.Text;
                Cost = text;
            }
            return;
        }

        public static async void GetNumber(ITelegramBotClient botClient, Update update)
        {
           await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Введите номер телефона или карты", replyMarkup: new ForceReplyMarkup());
           
            return;
        }


        public static async void MakeOrder(ITelegramBotClient botClient, ChatId chatId)
        {
            if (IsStartOrder)
            {
                KeyboardButton buttonStart = MakeButton;
                var rmu = new ReplyKeyboardMarkup(buttonStart);

                rmu.Keyboard = new List<List<KeyboardButton>>
                    {
                    new List<KeyboardButton>{{MakeButton},{JoinOrderButton} }
                    };
                await botClient.SendTextMessageAsync(chatId, "Выберите действие :", replyMarkup: rmu);
                return;
            }
        }

      
        public static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery,Update update)
        {
            Restourants restourantsSushi = new Restourants();
            switch (callbackQuery.Data)
            {
                case Sushi:
                    if (!TapSushi)
                    {
                        await botClient.SendTextMessageAsync(
                        callbackQuery.Message.Chat.Id,
                        $"Вот список ресторанов суши :\n");
                        foreach (var i in restourantsSushi.Sushi)
                        {
                            await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id, i, ParseMode.Markdown);
                        }
                        TapSushi = true;
                        IsOrderSelectVibor = true;
                        IsTap = true;
                    }
                    break;
                case Pizza:
                    if (!TapPizza)
                    {
                        await botClient.SendTextMessageAsync(
                       callbackQuery.Message.Chat.Id,
                       $"Вот список ресторанов пиццы :\n");
                        foreach (var i in restourantsSushi.Pizza)
                        {
                            await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id, i, ParseMode.Markdown);
                        }
                        TapPizza = true;
                        IsOrderSelectVibor = true;
                        IsTap = true;
                    }
                    break;
                case Burger:
                    if (!TapBurger)
                    {
                        await botClient.SendTextMessageAsync(
                       callbackQuery.Message.Chat.Id,
                       $"Вот список ресторанов бургер :\n");
                        foreach (var i in restourantsSushi.Burger)
                        {
                            await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id, i, ParseMode.Markdown);
                        }
                        TapBurger = true;
                        IsOrderSelectVibor = true;
                        IsTap = true;
                    }
                    break;
                case Another:
                    if (!TapAnother)
                    {
                        await botClient.SendTextMessageAsync(
                       callbackQuery.Message.Chat.Id,
                       $"Вот список ресторанов :\n");
                        foreach (var i in restourantsSushi.Another)
                        {
                            await botClient.SendTextMessageAsync(
                            callbackQuery.Message.Chat.Id, i, ParseMode.Markdown);
                        }
                        TapAnother = true;
                        IsOrderSelectVibor = true;
                        IsTap = true;
                    }
                    break;
            }

           // return;
        }

        public static async void Start(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (IsStartOrder) return;
            var message = update.Message;
            var text = message.Text;
            await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите что вас интересует:", replyMarkup: GetButtons());
            IsStartOrder = true;
        }

        public static IReplyMarkup GetButtons()
        {
            KeyboardButton buttonStart = StartOrder;
            return new ReplyKeyboardMarkup(buttonStart)
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{{StartOrder} }
                }
            };
        }


        public static IReplyMarkup Continue()
        {
            KeyboardButton buttonStart = JoinOrderButton;
            var rmu = new ReplyKeyboardMarkup(buttonStart);

            rmu.Keyboard = new List<List<KeyboardButton>>
                    {
                    new List<KeyboardButton>{{JoinOrderButton},{ MakeNewOrderButton}}
                    };
            return rmu;
        }

        public static IReplyMarkup MakeOrderButton()
        {
            KeyboardButton buttonStart = MakeButton;
            return new ReplyKeyboardMarkup(buttonStart)
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{{MakeButton},{JoinOrderButton} }
                }
            };
        }

        public static IReplyMarkup Menu()
        {
            var menu = new InlineKeyboardMarkup(new[]
                    {
                                new[]
                                {
                                    InlineKeyboardButton.WithCallbackData(Burger),
                                    InlineKeyboardButton.WithCallbackData(Sushi)
                                },
                                new[]
                                {
                                    InlineKeyboardButton.WithCallbackData(Pizza),
                                    InlineKeyboardButton.WithCallbackData(Another),
                                },
                });
            return menu;

        }


        public static Task Error(ITelegramBotClient arg1, Exception exception, CancellationToken arg3)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
