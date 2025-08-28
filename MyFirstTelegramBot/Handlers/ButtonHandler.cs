using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MyFirstTelegramBot.Handlers;

public class ButtonHandler
{
    private readonly ITelegramBotClient _botClient;

    InlineKeyboardMarkup navButtons;
    string caption = "";
    string imageFile = "";

    public ButtonHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendPublishTaskStepAsync(long chatId, string callbackData, CancellationToken cancellationToken, int messageId)
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

        // Файлни очиш
        await using var stream = File.OpenRead(imageFile);

        // Агар биринчи қадам бўлса — янги хабар
        if (callbackData == "publish_task")
        {
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




    public async Task SendAddCarAsync(long chatId, CancellationToken cancellationToken)
    {
    }

    public async Task SendCancelBookingAsync(long chatId, CancellationToken cancellationToken)
    {
    }

    public async Task SendAppIssueAsync(long chatId, CancellationToken cancellationToken)
    {
    }

    public async Task SendContactOperatorAsync(long chatId, CancellationToken cancellationToken)
    {

    }


}
