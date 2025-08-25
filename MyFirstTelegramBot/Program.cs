using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("8060528136:AAEMm3IvEid1EqnZ8TB2sVukqTrYwd8ZqvA");

var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    HandleUpdate,
    HandleError,
    receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMe();
Console.WriteLine($"Bot @{me.Username} начал работать!");
Console.ReadLine();
cts.Cancel();

// --- UPDATE HANDLER ---
static async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken token)
{
    if (update.Message is not { } message) return;
    if (message.Text is not { } text) return;

    var chatId = message.Chat.Id;

    if (text == "/start")
    {
        // бот ҳақида маълумот
        var info = "Ассалому алайкум!\n" +
                   "Меня зовут *MyTest-Бот*.\n" +
                   "Я могу оказать Вам различные услуги.";

        // тугмалар
        var buttons = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "О боте", "Меню" },
            new KeyboardButton[] { "Выход" }
        })
        {
            ResizeKeyboard = true // кнопкаларни телефонга мослаштиради
        };

        await bot.SendMessage(
            chatId: chatId,
            text: info,
            parseMode: ParseMode.Markdown,
            replyMarkup: buttons,
            cancellationToken: token
        );
    }
    else if (text == "О боте")
    {
        string path = File.ReadAllText(Environment.CurrentDirectory + "/AboutHimself.txt");

        await bot.SendMessage(chatId, path, cancellationToken: token);
    }
    else if (text == "Меню")
    {
        await bot.SendMessage(chatId, "Раздел меню в настоящее время пуст. 🙂", cancellationToken: token);
    }
    else if (text == "Выход")
    {
        await bot.SendMessage(chatId, "Вы вышли из меню.", cancellationToken: token);
    }
    else
    {
        await bot.SendMessage(chatId, "Я тебя не понял.", cancellationToken: token);
    }
}

// --- ERROR HANDLER ---
static Task HandleError(ITelegramBotClient bot, Exception exception, CancellationToken token)
{
    var msg = exception switch
    {
        ApiRequestException api => $"Ошибка API Telegram: [{api.ErrorCode}] {api.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(msg);

    return Task.CompletedTask;
}
