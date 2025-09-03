using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyFirstTelegramBot.Handlers;

public class UpdateHandler
{
    private readonly ButtonHandler _buttonHandler;
    private readonly ConnectOperatorHandler _connectOperatorHandler;
    private bool test = false;

    public UpdateHandler(ButtonHandler buttonHandler, ConnectOperatorHandler connectOperatorHandler)
    {
        _buttonHandler = buttonHandler;
        _connectOperatorHandler = connectOperatorHandler;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery?.Message?.Chat.Id;
        var messageId = update.CallbackQuery?.Message?.MessageId;
        var callBackData = update.CallbackQuery?.Data;
        string callBackDataId = update.CallbackQuery?.Id ?? "";

        if (update.Type == UpdateType.Message && update.Message?.Text?.Trim() == "/start")
        {
            var firstName = update.Message.From?.FirstName ?? "Пользователь";
            await SendStartMessage(botClient, update.Message.Chat.Id, firstName, cancellationToken);
        }

        if (update.Type == UpdateType.CallbackQuery)
        {
            if (chatId.HasValue && messageId.HasValue && !string.IsNullOrWhiteSpace(callBackData))
            {
                if (callBackData == "publish_task"
                    || callBackData == "publish_task_next1"
                    || callBackData == "publish_task_next2"
                    || callBackData == "publish_task_next3"
                    || callBackData == "publish_task_next4"
                    || callBackData == "publish_task_next5")
                {
                    await _buttonHandler.SendPublishTaskStepAsync(chatId.Value, callBackData, messageId.Value, callBackDataId, cancellationToken);
                    test = false;
                }

                else if (callBackData == "add_car"
                    || callBackData == "add_car_next1"
                    || callBackData == "add_car_next2"
                    || callBackData == "add_car_next3"
                    || callBackData == "add_car_next4")
                {
                    await _buttonHandler.SendAddCarAsync(chatId.Value, callBackData, messageId.Value, callBackDataId, cancellationToken);
                    test = false;
                }

                else if (callBackData == "cancel_booking"
                    || callBackData == "cancel_booking_next1"
                    || callBackData == "cancel_booking_next2"
                    || callBackData == "cancel_booking_next3")
                {
                    await _buttonHandler.SendCancelBookingAsync(chatId.Value, callBackData, messageId.Value, callBackDataId, cancellationToken);
                    test = false;
                }

                else if (callBackData == "app_issue"
                    || callBackData == "app_issue_next1"
                    || callBackData == "app_issue_next2"
                    || callBackData == "app_issue_next3"
                    || callBackData == "app_issue_next4")
                {
                    await _buttonHandler.SendAppIssueAsync(chatId.Value, callBackData, messageId.Value, callBackDataId, cancellationToken);
                    test = false;
                }
            }
        } 

        if (update.Type == UpdateType.CallbackQuery || update.Type == UpdateType.Message)
        {
            if (callBackData == "contact_operator")
            {
                await _connectOperatorHandler.HandleUpdateAsync(update, cancellationToken);
                test = true;
            }
            else if (test && update.Type == UpdateType.Message)
            {
                await _connectOperatorHandler.HandleUpdateAsync(update, cancellationToken);
            }
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