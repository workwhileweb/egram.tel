using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using ReactiveUI;
using Splat;
using Tel.Egram.Model.Notifications;
using Tel.Egram.Services.Utils.Platforms;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Views.Notifications;

namespace Tel.Egram.Views.Application
{
    public static class NotificationLogic
    {
        public static IDisposable BindNotifications(
            this MainWindow mainWindow)
        {
            return mainWindow.BindNotifications(
                Locator.Current.GetService<IPlatform>(),
                Locator.Current.GetService<INotificationController>());
        }

        public static IDisposable BindNotifications(
            this MainWindow mainWindow,
            IPlatform platform,
            INotificationController controller)
        {
            var trigger = (controller as NotificationController)?.Trigger;

            return trigger != null
                ? trigger
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Accept(model =>
                    {
                        var screen = mainWindow.Screens.Primary;
                        var window = new NotificationWindow();
                        window.Show();

                        window.DataContext = model;
                        window.Position = new PixelPoint(
                            GetXForNotification(platform, screen.Bounds, window.Bounds),
                            GetYForNotification(platform, screen.Bounds, window.Bounds));
                    })
                : Disposable.Empty;
        }

        private static int GetXForNotification(IPlatform platform, PixelRect outer, Rect inner)
        {
            return platform switch
            {
                _ => outer.Width - (int)inner.Width
            };
        }

        private static int GetYForNotification(IPlatform platform, PixelRect outer, Rect inner)
        {
            return platform switch
            {
                _ => 0
            };
        }
    }
}