using Telegram.Bot;

namespace MyFirstTelegramBot.Handlers;

public class ErrorHandler
{
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Произошла ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}