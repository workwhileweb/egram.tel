using System;
using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Splat;
using Tel.Egram.Model.Application.Startup;
using Tel.Egram.Model.Authentication;
using Tel.Egram.Model.Popups;
using Tel.Egram.Model.Workspace;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.Model.Application
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowModel : ISupportsActivation
    {
        public MainWindowModel()
        {
            this.WhenActivated(disposables =>
            {
                this.BindAuthentication()
                    .DisposeWith(disposables);

                this.BindConnectionInfo()
                    .DisposeWith(disposables);

                this.BindPopup()
                    .DisposeWith(disposables);
            });
        }

        public StartupModel StartupModel { get; set; }

        public AuthenticationModel AuthenticationModel { get; set; }

        public WorkspaceModel WorkspaceModel { get; set; }

        public PopupModel PopupModel { get; set; }

        public int PageIndex { get; set; }
        
        public string ConnectionState { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}