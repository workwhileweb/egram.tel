using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Model.Messenger.Catalog;
using Tel.Egram.Model.Messenger.Catalog.Entries;
using Tel.Egram.Model.Messenger.Editor;
using Tel.Egram.Model.Messenger.Explorer;
using Tel.Egram.Model.Messenger.Homepage;
using Tel.Egram.Model.Messenger.Informer;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.Model.Messenger
{
    public static class MessengerLogic
    {
        public static IDisposable BindCatalog(this MessengerModel model, Section section)
        {
            model.CatalogModel = new CatalogModel(section);

            return Disposable.Empty;
        }

        public static IDisposable BindInformer(this MessengerModel model)
        {
            model.InformerModel = InformerModel.Hidden();

            return model.SubscribeToSelection(entry =>
            {
                model.InformerModel = entry switch
                {
                    ChatEntryModel chatEntryModel => new InformerModel(chatEntryModel.Chat),
                    AggregateEntryModel aggregateEntryModel => new InformerModel(aggregateEntryModel.Aggregate),
                    HomeEntryModel _ => InformerModel.Hidden(),
                    _ => model.InformerModel
                };
            });
        }

        public static IDisposable BindExplorer(this MessengerModel model)
        {
            model.ExplorerModel = ExplorerModel.Hidden();

            return model.SubscribeToSelection(entry =>
            {
                switch (entry)
                {
                    case ChatEntryModel chatEntryModel:
                        model.ExplorerModel = new ExplorerModel(chatEntryModel.Chat);
                        break;

                    case AggregateEntryModel aggregateEntryModel:
                        //model.ExplorerModel = new ExplorerModel(aggregateEntryModel.Aggregate);
                        break;

                    case HomeEntryModel _:
                        model.ExplorerModel = ExplorerModel.Hidden();
                        break;
                    default:
                        break;
                }
            });
        }

        public static IDisposable BindHome(this MessengerModel model)
        {
            model.HomepageModel = HomepageModel.Hidden();

            return model.SubscribeToSelection(entry =>
            {
                model.HomepageModel = entry switch
                {
                    HomeEntryModel _ => new HomepageModel(),
                    _ => HomepageModel.Hidden()
                };
            });
        }

        public static IDisposable BindEditor(this MessengerModel model)
        {
            model.EditorModel = EditorModel.Hidden();

            return model.SubscribeToSelection(entry =>
            {
                model.EditorModel = entry switch
                {
                    ChatEntryModel chatEntryModel => new EditorModel(chatEntryModel.Chat),
                    _ => EditorModel.Hidden()
                };
            });
        }

        private static IDisposable SubscribeToSelection(this MessengerModel model, Action<EntryModel> action)
        {
            return model.WhenAnyValue(ctx => ctx.CatalogModel.SelectedEntry)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Accept(entry =>
                {
                    if (entry != null)
                    {
                        action(entry);
                    }
                });
        }
    }
}