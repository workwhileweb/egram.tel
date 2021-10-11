using System;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Messenger.Explorer.Items;
using Tel.Egram.Model.Messenger.Explorer.Loaders;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;
using Range = Tel.Egram.Services.Utils.Range;

namespace Tel.Egram.Model.Messenger.Explorer
{
    [AddINotifyPropertyChangedInterface]
    public class ExplorerModel : ISupportsActivation
    {
        public ExplorerModel(Chat chat)
        {
            this.WhenActivated(disposables =>
            {
                BindSource()
                    .DisposeWith(disposables);

                var conductor = new MessageLoaderConductor();

                new InitMessageLoader(conductor)
                    .Bind(this, chat)
                    .DisposeWith(disposables);

                new NextMessageLoader(conductor)
                    .Bind(this, chat)
                    .DisposeWith(disposables);

                new PrevMessageLoader(conductor)
                    .Bind(this, chat)
                    .DisposeWith(disposables);
            });
        }

        private ExplorerModel()
        {
        }

        public bool IsVisible { get; set; } = true;

        public Range VisibleRange { get; set; }

        public ItemModel TargetItem { get; set; }

        public ObservableCollectionExtended<ItemModel> Items { get; set; }
            = new ObservableCollectionExtended<ItemModel>();

        public SourceList<ItemModel> SourceItems { get; set; }
            = new SourceList<ItemModel>();

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        private IDisposable BindSource()
        {
            return SourceItems.Connect()
                .Bind(Items)
                .Accept();
        }

        public static ExplorerModel Hidden()
        {
            return new ExplorerModel
            {
                IsVisible = false
            };
        }
    }
}