using MyFirstTelegramBot.Data;
using MyFirstTelegramBot.Handlers;
using Telegram.Bot;

var botClient = new TelegramBotClient(BotConfiguration.Token);
var connectOperatorHandler = new ConnectOperatorHandler(botClient);
var buttonHandler = new ButtonHandler(botClient);
var updateHandler = new UpdateHandler(buttonHandler, connectOperatorHandler);
var errorHandler = new ErrorHandler();

botClient.StartReceiving(updateHandler.HandleUpdateAsync, errorHandler.HandleErrorAsync);

Console.WriteLine("Бот работает.");

Console.ReadLine();
