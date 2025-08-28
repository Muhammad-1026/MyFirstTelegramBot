using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyFirstTelegramBot.Handlers;

public class UpdateHandler
{
    private readonly ButtonHandler _buttonHandler;

    public UpdateHandler(ButtonHandler buttonHandler)
    {
        _buttonHandler = buttonHandler;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery?.Message?.Chat.Id;
        var messageId = update.CallbackQuery?.Message?.MessageId;
        var callBackData = update.CallbackQuery?.Data;

        if (update.Type == UpdateType.Message && update.Message?.Text?.Trim() == "/start")
        {
            var firstName = update.Message.From?.FirstName ?? "Пользователь";
            await SendStartMessage(botClient, update.Message.Chat.Id, firstName, cancellationToken);
        }

        if (update.Type == UpdateType.CallbackQuery)
        {
            if (callBackData == "publish_task" 
                || callBackData == "publish_task_next1"
                || callBackData == "publish_task_next2" 
                || callBackData == "publish_task_next3" 
                || callBackData == "publish_task_next4" 
                || callBackData == "publish_task_next5")
            {
                await _buttonHandler.SendPublishTaskStepAsync(chatId!.Value, callBackData, cancellationToken, update.CallbackQuery.Message.MessageId);
            }

            //switch (callBackData)
            //{
            //    case "publish_task":
            //        await _buttonHandler.SendPublishTaskStepAsync(chatId!.Value, callBackData, cancellationToken, update.CallbackQuery.Message.MessageId);
            //        break;

            //    case "add_car":
            //        await _buttonHandler.SendAddCarAsync(chatId!.Value, cancellationToken);
            //        break;

            //    case "cancel_booking":
            //        await _buttonHandler.SendCancelBookingAsync(chatId!.Value, cancellationToken);
            //        break;

            //    case "app_issue":
            //        await _buttonHandler.SendAppIssueAsync(chatId!.Value, cancellationToken);
            //        break;

            //    case "contact_operator":
            //        await _buttonHandler.SendContactOperatorAsync(chatId!.Value, cancellationToken);
            //        break;
            //}
        }
    }

    private async Task SendStartMessage(ITelegramBotClient botClient, long chatId, string firstName, CancellationToken cancellationToken)
    {
        string message =
            $"Здравствуйте, {firstName}! 👋\n\n" +
            "Добро пожаловать в бот поддержки Hamroh. \n" +
            "Здесь вы можете задать вопросы, оставить отзывы или сообщить о проблемах.\n\n" +
            "Выберите интересующий вас вопрос ниже: \n";

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("📝 Как опубликовать задачу?", "publish_task")},
            new[] { InlineKeyboardButton.WithCallbackData("🚗 Как добавить автомобиль?", "add_car")},
            new[] { InlineKeyboardButton.WithCallbackData("❌ Как отменить посадку?", "cancel_booking")},
            new[] { InlineKeyboardButton.WithCallbackData("📰 Не работает приложение", "app_issue")},
            new[] { InlineKeyboardButton.WithCallbackData("💬 Связаться с оператором", "contact_operator")}
        });

        await botClient.SendMessage(
            chatId: chatId,
            text: message,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken
        );
    }
}