using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Extensions
{
    class NotificationExtenstions
    {
        public static void ShowToast(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        public static void ShowTaskDueNotification(string taskTitle, DateTime dueDate)
        {
            new ToastContentBuilder()
                .AddText("Дедлайн по задаче!")
                .AddText($"{taskTitle} должна быть выполнена {dueDate:g}")
                .Show();
        }
    }
}
