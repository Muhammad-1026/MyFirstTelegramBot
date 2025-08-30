using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyFirstTelegramBot.Handlers;

public class ConnectOperatorHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly long _operatorGroupId = -4893537315; // операторлар гуруҳи ID
    private readonly ConcurrentDictionary<long, bool> _activeSupportUsers = new(); // актив клиентлар

    public ConnectOperatorHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            await HandleCallback(update.CallbackQuery!, cancellationToken);
        }
        else if (update.Type == UpdateType.Message)
        {
            await HandleMessage(update.Message!, cancellationToken);
        }
    }

    // 📌 "Операторга уланиш" тугмаси босилганда
    private async Task HandleCallback(CallbackQuery callback, CancellationToken cancellationToken)
    {
        if (callback.Data == "contact_operator")
        {
            var chatId = callback.Message!.Chat.Id;
            var from = callback.From;
            var userName = !string.IsNullOrEmpty(from.Username) ? "@" + from.Username : from.FirstName;

            // Клиентга хабар
            await _botClient.SendMessage(
                chatId,
                "Менеджер скоро свяжется с вами. Пожалуйста, напишите ваш вопрос здесь 📝.",
                cancellationToken: cancellationToken
            );

            // Клиент ID'ни актив сессияга қўшамиз
            _activeSupportUsers[chatId] = true;

            // Операторларга хабар
            await _botClient.SendMessage(
                _operatorGroupId,
                $"🆕 Клиент связывается с операторами:\n👤 {userName}\n🆔 {chatId}",
                cancellationToken: cancellationToken
            );
        }
    }

    // 📌 Клиент ёки оператор хабар йўллаганда
    private async Task HandleMessage(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        // Агар клиент актив сессияда бўлса → хабар операторларга кетади
        if (_activeSupportUsers.ContainsKey(chatId))
        {
            await _botClient.SendMessage(
                _operatorGroupId,
                $"📩 Вопрос от клиента:\n🆔 {chatId}\n👤 @{message.From?.Username ?? message.From?.FirstName}\n\n{message.Text}",
                cancellationToken: cancellationToken
            );
        }
        // Агар хабар операторлар гуруҳидан келса ва reply бўлса
        else if (chatId == _operatorGroupId && message.ReplyToMessage != null)
        {
            var match = Regex.Match(message.ReplyToMessage.Text!, @"🆔 (\d+)");
            if (match.Success && long.TryParse(match.Groups[1].Value, out var clientId))
            {
                await _botClient.SendMessage(
                    clientId,
                    $"\n{message.Text}",
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
