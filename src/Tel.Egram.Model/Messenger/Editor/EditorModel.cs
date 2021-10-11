using System.Reactive;
using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Services.Messaging.Chats;

namespace Tel.Egram.Model.Messenger.Editor
{
    [AddINotifyPropertyChangedInterface]
    public class EditorModel : ISupportsActivation
    {
        public EditorModel(Chat chat)
        {
            this.WhenActivated(disposables =>
            {
                this.BindSender(chat)
                    .DisposeWith(disposables);
            });
        }

        private EditorModel()
        {
        }

        public bool IsVisible { get; set; } = true;

        public string Text { get; set; }

        public ReactiveCommand<Unit, Unit> SendCommand { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public static EditorModel Hidden()
        {
            return new EditorModel
            {
                IsVisible = false
            };
        }
    }
}