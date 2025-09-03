using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyFirstTelegramBot.Handlers;

public class ButtonHandler(ITelegramBotClient botClient)
{
    private readonly ITelegramBotClient _botClient = botClient;

    InlineKeyboardMarkup? navButtons;
    string caption = "";
    string imageFile = "";

    public async Task SendPublishTaskStepAsync(long chatId, string callbackData, int messageId, string callBackDataId, CancellationToken cancellationToken)
    {
        switch (callbackData)
        {
            case "publish_task":
                caption = "Шаг 1: Откройте приложение Hamroh 🚖";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img1.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "publish_task_next2")
                });

                break;

            case "publish_task_next1":
                caption = "Шаг 1: Откройте приложение Hamroh 🚖";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img1.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "publish_task_next2")
                });
                break;

            case "publish_task_next2":
                caption = "Шаг 2: Нажмите кнопку 'Создать задачу' ✍️";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img2.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "publish_task_next1"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "publish_task_next3"),
                });
                break;

            case "publish_task_next3":
                caption = "Шаг 3: Заполните все данные задачи 📋";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img3.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "publish_task_next2"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "publish_task_next4")
                });
                break;

            case "publish_task_next4":
                caption = "Шаг 4: Проверьте данные и подтвердите ✅";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img4.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "publish_task_next3"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "publish_task_next5")
                });
                break;

            case "publish_task_next5":
                caption = "Шаг 5: Ваша задача успешно опубликована 🎉";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img5.jpg");
                navButtons = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "publish_task_next4"));
                break;

            default:
                return;
        }

        await ResponseMessage(callbackData, imageFile, chatId, messageId, callBackDataId, cancellationToken);
    }

    public async Task SendAddCarAsync(long chatId, string callbackData, int messageId, string callBackDataId, CancellationToken cancellationToken)
    {
        switch (callbackData)
        {
            case "add_car":
                caption = "Шаг 1: Откройте Hamroh и перейдите в профиль 👤";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img6.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "add_car_next2")
                });
                break;

            case "add_car_next1":
                caption = "Шаг 1: Откройте Hamroh и перейдите в профиль 👤";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img6.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                        InlineKeyboardButton.WithCallbackData("➡️ Следующий", "add_car_next2")
                    });
                break;

            case "add_car_next2":
                caption = "Шаг 2: Выберите раздел «Автомобиль» 🚗";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img7.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "add_car_next1"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "add_car_next3")
                });
                break;

            case "add_car_next3":
                caption = "Шаг 3: Нажмите «Добавить» ➕";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img8.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "add_car_next2"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "add_car_next4")
                });
                break;

            case "add_car_next4":
                caption = "Шаг 4: Введите данные: марку, модель и номер 📝";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img9.jpg");
                navButtons = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "add_car_next3"));
                break;
        }

        await ResponseMessage(callbackData, imageFile, chatId, messageId, callBackDataId,cancellationToken);
    }

    public async Task SendCancelBookingAsync(long chatId, string callbackData, int messageId, string callBackDataId, CancellationToken cancellationToken)
    {
        switch (callbackData)
        {
            case "cancel_booking":
                caption = "Шаг 1: Откройте Hamroh и перейдите в раздел «Мои поездки» 🧳";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img10.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "cancel_booking_next2")
                });
                break;
            case "cancel_booking_next1":
                caption = "Шаг 1: Откройте Hamroh и перейдите в раздел «Мои поездки» 🧳";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img10.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "cancel_booking_next2")
                });
                break;
            case "cancel_booking_next2":
                caption = "Шаг 2: Выберите поездку, которую хотите отменить ❌";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img11.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "cancel_booking_next1"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "cancel_booking_next3")
                });
                break;
            case "cancel_booking_next3":
                caption = "Шаг 3: Подтвердите отмену поездки ✅";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img12.jpg");
                navButtons = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "cancel_booking_next2"));
                break;
        }

        await ResponseMessage(callbackData, imageFile, chatId, messageId, callBackDataId, cancellationToken);
    }

    public async Task SendAppIssueAsync(long chatId, string callbackData, int messageId, string callBackDataId, CancellationToken cancellationToken)
    {
        switch (callbackData)
        {
            case "app_issue":
                caption = "Шаг 1: Как посмотреть важные новости о дорогах? 🛣️";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img13.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "app_issue_next2")
                });
                break;

            case "app_issue_next1":
                caption = "Шаг 1: Как посмотреть важные новости о дорогах? 🛣️";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img13.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "app_issue_next2")
                });
                break;

            case "app_issue_next2":
                caption = "Шаг 2: Откройте главное меню 📲,";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img14.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "app_issue_next1"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "app_issue_next3")
                });
                break;

            case "app_issue_next3":
                caption = "Шаг 3: Нажмите на иконку в левом верхнем углу экрана 🔍";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img15.jpg");
                navButtons = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "app_issue_next2"),
                    InlineKeyboardButton.WithCallbackData("➡️ Следующий", "app_issue_next4")
                });
                break;

            case "app_issue_next4":
                caption = "Шаг 4: И перейдите в раздел «Новости» 📢";
                imageFile = Path.Combine(Environment.CurrentDirectory, "Images", "img16.jpg");
                navButtons = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "app_issue_next3"));
                break;
        }

        await ResponseMessage(callbackData, imageFile, chatId, messageId, callBackDataId, cancellationToken);
    }

    private async Task ResponseMessage(string callbackData, string imageFile, long chatId, int messageId, string callBackDataId, CancellationToken cancellationToken)
    {
        var stream = File.OpenRead(imageFile);

        if (callbackData == "app_issue" || callbackData == "cancel_booking" || callbackData == "add_car" || callbackData == "publish_task")
        {
            await _botClient.AnswerCallbackQuery(callBackDataId, "✅ Выбрано!");

            await _botClient.SendPhoto(
                chatId: chatId,
                photo: InputFile.FromStream(stream, Path.GetFileName(imageFile)),
                caption: caption,
                replyMarkup: navButtons,
                cancellationToken: cancellationToken
            );
        }
        else // кейинги қадамлар — эски хабарни edit қиламиз
        {
            await _botClient.EditMessageMedia(
                chatId: chatId,
                messageId: messageId,
                media: new InputMediaPhoto(InputFile.FromStream(stream, Path.GetFileName(imageFile)))
                {
                    Caption = caption
                },
                replyMarkup: navButtons,
                cancellationToken: cancellationToken
            );
        }
    }
}