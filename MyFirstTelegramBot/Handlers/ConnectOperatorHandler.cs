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
        try
        {
            if (callback.Data == "contact_operator")
            {
                var chatId = callback.Message!.Chat.Id;
                var from = callback.From;
                var userName = !string.IsNullOrEmpty(from.Username) ? "@" + from.Username : from.FirstName;

                await _botClient.AnswerCallbackQuery(callback.Id, "Вы выбрали связь с оператором. Пожалуйста, подождите...", cancellationToken: cancellationToken);

                // Клиентга хабар
                await _botClient.SendMessage(
                    chatId,
                    "Менеджер скоро свяжется с вами. Пожалуйста, напишите ваш вопрос 📝.",
                    cancellationToken: cancellationToken
                );

                // Клиент ID'ни актив сессияга қўшамиз
                _activeSupportUsers[chatId] = true;

                // Операторларга хабар
                await _botClient.SendMessage(
                    _operatorGroupId,
                    $"Forwarded from {userName}\n🆔 {chatId}",
                    cancellationToken: cancellationToken
                );
            }
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException error)
        {
            await _botClient.SendMessage(_operatorGroupId, $"Ошибка отправки сообщения менеджеру: {error.Message}", cancellationToken: cancellationToken);

            Console.WriteLine($"Ошибка отправки сообщения менеджеру: {error.Message}");
        }
    }

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
                try
                {
                    await _botClient.SendMessage(
                        clientId,
                        $"\n{message.Text}",
                        cancellationToken: cancellationToken
                    );

                    // ✅ Муваффақият хабарини операторга юбориш
                    await _botClient.SendMessage(
                        _operatorGroupId,
                        $"Сообщение успешно доставлено клиенту 🆔 {clientId}",
                        cancellationToken: cancellationToken
                    );
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException error) when (error.Message.Contains("bot was blocked by the user"))
                {
                    await _botClient.SendMessage(
                        _operatorGroupId,
                        $"Клиент заблокировал бота. Невозможно отправить сообщение. 🆔 {clientId}",
                        cancellationToken: cancellationToken
                    );
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException error) when (error.Message.Contains("chat not found"))
                {
                    await _botClient.SendMessage(
                        _operatorGroupId,
                        $"Чат с клиентом не найден. Возможно, клиент удалил чат с ботом. 🆔 {clientId}",
                        cancellationToken: cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    await _botClient.SendMessage(
                        _operatorGroupId,
                        $"Произошла непредвиденная ошибка при отправке сообщения клиенту 🆔 {clientId}:\n{ex.Message}",
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
    }

}
