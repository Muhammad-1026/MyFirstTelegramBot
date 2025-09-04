using System;
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

        // 🟢 Агар клиент актив бўлса → хабарни операторларга узатиш
        if (_activeSupportUsers.ContainsKey(chatId) && message.Text != "/start")
        {
            await ForwardToOperators(message, cancellationToken);
        }
        // 🟢 Агар хабар оператор гуруҳидан келса ва reply бўлса → клиентга узатиш
        else if (chatId == _operatorGroupId && message.ReplyToMessage != null)
        {
            await ForwardToClient(message, cancellationToken);
        }
    }

    // Клиентдан келган хабарни операторларга узатиш
    private async Task ForwardToOperators(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var senderName = $"🆔 {chatId}\n👤 @{message.From?.Username ?? message.From?.FirstName}";

        if (!string.IsNullOrWhiteSpace(message.Text))
        {
            await _botClient.SendMessage(
                _operatorGroupId,
                $"📩 Вопрос от клиента:\n{senderName}\n\n{message.Text}",
                cancellationToken: cancellationToken
            );

            await _botClient.SendMessage(chatId, "Сообщение успешно доставлено менеджеру ✅", cancellationToken: cancellationToken);
        }
        else if (message.Photo != null)
        {
            await _botClient.SendPhoto(_operatorGroupId, message.Photo.Last().FileId, caption: $"📩 Фото от клиента:\n{senderName}", cancellationToken: cancellationToken);
            await _botClient.SendMessage(chatId, "Фото успешно доставлено менеджеру ✅", cancellationToken: cancellationToken);
        }
        else if (message.Video != null)
        {
            await _botClient.SendVideo(_operatorGroupId, message.Video.FileId, caption: $"📩 Видео от клиента:\n{senderName}", cancellationToken: cancellationToken);
            await _botClient.SendMessage(chatId, "Видео успешно доставлено менеджеру ✅", cancellationToken: cancellationToken);
        }
        else if (message.Document != null)
        {
            await _botClient.SendDocument(_operatorGroupId, message.Document.FileId, caption: $"📩 Документ от клиента:\n{senderName}", cancellationToken: cancellationToken);
            await _botClient.SendMessage(chatId, "Документ успешно доставлен менеджеру ✅", cancellationToken: cancellationToken);
        }
        else if (message.Audio != null)
        {
            await _botClient.SendAudio(_operatorGroupId, message.Audio.FileId, caption: $"📩 Аудио от клиента:\n{senderName}", cancellationToken: cancellationToken);
            await _botClient.SendMessage(chatId, "Аудио успешно доставлено менеджеру ✅", cancellationToken: cancellationToken);
        }
        else if (message.Voice != null)
        {
            await _botClient.SendVoice(_operatorGroupId, message.Voice.FileId, caption: $"📩 Голосовое сообщение:\n{senderName}", cancellationToken: cancellationToken);
            await _botClient.SendMessage(chatId, "Голосовое сообщение успешно доставлено менеджеру ✅", cancellationToken: cancellationToken);
        }
        else if (message.Sticker != null)
        {
            await _botClient.SendSticker(_operatorGroupId, message.Sticker.FileId, cancellationToken: cancellationToken);
            await _botClient.SendMessage(_operatorGroupId, $"📩 Стикер от клиента:\n{senderName}", cancellationToken: cancellationToken);
            await _botClient.SendMessage(chatId, "Стикер успешно доставлено менеджеру ✅", cancellationToken: cancellationToken);
        }
    }

    // Оператордан келган хабарни клиентга узатиш
    private async Task ForwardToClient(Message message, CancellationToken cancellationToken)
    {
        var sourceText = message.ReplyToMessage?.Text ?? message.ReplyToMessage?.Caption;
        var match = Regex.Match(sourceText ?? "", @"🆔 (\d+)");

        if (!match.Success || !long.TryParse(match.Groups[1].Value, out var clientId))
            return;

        try
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                await _botClient.SendMessage(clientId, message.Text, cancellationToken: cancellationToken);
                await _botClient.SendMessage(_operatorGroupId, "Сообщение успешно доставлено клиенту ✅", cancellationToken: cancellationToken);
            }
            else if (message.Photo != null)
            {
                await _botClient.SendPhoto(clientId, message.Photo.Last().FileId, caption: message.Caption, cancellationToken: cancellationToken);
                await _botClient.SendMessage(_operatorGroupId, "Фото успешно отправлено клиенту ✅", cancellationToken: cancellationToken);
            }
            else if (message.Video != null)
            {
                await _botClient.SendVideo(clientId, message.Video.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                await _botClient.SendMessage(_operatorGroupId, "Видео успешно отправлено клиенту ✅", cancellationToken: cancellationToken);
            }
            else if (message.Document != null)
            {
                await _botClient.SendDocument(clientId, message.Document.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                await _botClient.SendMessage(_operatorGroupId, "Документ успешно отправлен клиенту ✅", cancellationToken: cancellationToken);
            }
            else if (message.Audio != null)
            {
                await _botClient.SendAudio(clientId, message.Audio.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                await _botClient.SendMessage(_operatorGroupId, "Аудио успешно отправлено клиенту ✅", cancellationToken: cancellationToken);
            }
            else if (message.Voice != null)
            {
                await _botClient.SendVoice(clientId, message.Voice.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                await _botClient.SendMessage(_operatorGroupId, "Голосовое сообщение отправлено клиенту ✅", cancellationToken: cancellationToken);
            }
            else if (message.Sticker != null)
            {
                await _botClient.SendSticker(clientId, message.Sticker.FileId, cancellationToken: cancellationToken);
                await _botClient.SendMessage(_operatorGroupId, "Стикер отправлен клиенту ✅", cancellationToken: cancellationToken);
            }
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException error) when (error.Message.Contains("bot was blocked by the user"))
        {
            await _botClient.SendMessage(_operatorGroupId, $"Клиент заблокировал бота ❌ 🆔 {clientId}", cancellationToken: cancellationToken);
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException error) when (error.Message.Contains("chat not found"))
        {
            await _botClient.SendMessage(_operatorGroupId, $"Чат с клиентом не найден ❌ 🆔 {clientId}", cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            await _botClient.SendMessage(_operatorGroupId, $"Ошибка при отправке клиенту 🆔 {clientId}:\n{ex.Message}", cancellationToken: cancellationToken);
        }
    }
}
