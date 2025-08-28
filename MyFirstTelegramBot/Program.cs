using MyFirstTelegramBot.Data;
using MyFirstTelegramBot.Handlers;
using Telegram.Bot;

var botClient = new TelegramBotClient(BotConfiguration.Token);

var buttonHandler = new ButtonHandler(botClient);
var updateHandler = new UpdateHandler(buttonHandler);
var errorHandler = new ErrorHandler();

botClient.StartReceiving(updateHandler.HandleUpdateAsync, errorHandler.HandleErrorAsync);

Console.WriteLine("Бот работает.");

Console.ReadLine();
