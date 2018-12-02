using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace Tel.Egram.Gui.Views.Notifications
{
    public static class AutohideLogic
    {
        public static IDisposable BindAutohide(
            this NotificationWindow window)
        {
            var timer = Observable.Timer(TimeSpan.FromSeconds(5))
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    window.Close();
                });
            
            return new CompositeDisposable(
                timer,
                Disposable.Create(window.Close));
        }
    }
}