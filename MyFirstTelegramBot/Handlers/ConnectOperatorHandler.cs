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

                await _botClient.AnswerCallbackQuery(callback.Id, "Вы выбрали связь с оператором. Пожалуйста, подождите...", cancellationToken: cancellationToken);

                // Клиентга хабар
                await _botClient.SendMessage(
                    chatId,
                    "Менеджер скоро свяжется с вами. Пожалуйста, напишите ваш вопрос 📝.",
                    cancellationToken: cancellationToken
                );

                // Клиент ID'ни актив сессияга қўшамиз
                _activeSupportUsers[chatId] = true;
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
        if (_activeSupportUsers.ContainsKey(chatId) && message.Text != "/start")
        {
           
            if (message.Text != null)
            {
                await _botClient.SendMessage(
                    _operatorGroupId,
                    $"📩 Вопрос от клиента:\n🆔 {chatId}\n👤 @{message.From?.Username ?? message.From?.FirstName}\n\n{message.Text}",
                    cancellationToken: cancellationToken
                );
            }
            else if (message.Photo != null)
            {
                await _botClient.SendPhoto(
                    _operatorGroupId,
                    message.Photo!.Last().FileId,
                    caption: $"📩 Вопрос от клиента:\n🆔 {chatId}\n👤 @{message.From?.Username ?? message.From?.FirstName}",
                    cancellationToken: cancellationToken
                );
            }
            else if (message.Video != null)
            {
                await _botClient.SendVideo(
                    _operatorGroupId,
                    message.Video!.FileId,
                    caption: $"📩 Вопрос от клиента:\n🆔 {chatId}\n👤 @{message.From?.Username ?? message.From?.FirstName}",
                    cancellationToken: cancellationToken
                );
            }
        }
        // Агар хабар операторлар гуруҳидан келса ва reply бўлса ёки сурат бўлса → хабар клиентга кетади
        else if (chatId == _operatorGroupId && (message.ReplyToMessage != null || message.Photo != null || message.Video != null))
        {
            var sourceText = message.ReplyToMessage?.Text ?? message.ReplyToMessage?.Caption;

            if (!string.IsNullOrWhiteSpace(sourceText))
            {
                var match = Regex.Match(sourceText, @"🆔 (\d+)");
                if (match.Success && long.TryParse(match.Groups[1].Value, out var clientId))
                {
                    try
                    {
                        
                        if (!string.IsNullOrEmpty(message.Text))
                        {
                            await _botClient.SendMessage(
                                clientId,
                                message.Text,
                                cancellationToken: cancellationToken
                            );

                            await _botClient.SendMessage(
                                _operatorGroupId,
                                $"Сообщение успешно доставлено клиенту",
                                cancellationToken: cancellationToken
                            );
                        }
                        else if (message.Photo != null)
                        {
                            await _botClient.SendPhoto(
                                clientId,
                                message.Photo!.Last().FileId,
                                caption: message.Caption,
                                cancellationToken: cancellationToken
                            );

                            await _botClient.SendMessage(
                                _operatorGroupId,
                                $"Фото успешно отправлено клиенту",
                                cancellationToken: cancellationToken
                            );
                        }
                        else if (message.Video != null)
                        {
                            await _botClient.SendVideo(
                                clientId,
                                message.Video!.FileId,
                                caption: message.Caption,
                                cancellationToken: cancellationToken
                            );

                            await _botClient.SendMessage(
                                _operatorGroupId,
                                $"Видео успешно отправлено клиенту",
                                cancellationToken: cancellationToken
                            );
                        }
                    }
                    catch (Telegram.Bot.Exceptions.ApiRequestException error) when (error.Message.Contains("bot was blocked by the user"))
                    {
                        await _botClient.SendMessage(_operatorGroupId, $"Клиент заблокировал бота. Невозможно отправить сообщение. 🆔 {clientId}", cancellationToken: cancellationToken);
                    }
                    catch (Telegram.Bot.Exceptions.ApiRequestException error) when (error.Message.Contains("chat not found"))
                    {
                        await _botClient.SendMessage(_operatorGroupId, $"Чат с клиентом не найден. Возможно, клиент удалил чат с ботом. 🆔 {clientId}", cancellationToken: cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        await _botClient.SendMessage(_operatorGroupId, $"Произошла непредвиденная ошибка при отправке сообщения клиенту 🆔 {clientId}:\n{ex.Message}", cancellationToken: cancellationToken);
                    }
                }
            }
        }

    }

}
