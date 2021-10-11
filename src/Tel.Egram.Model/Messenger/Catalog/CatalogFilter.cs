using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;
using TdLib;
using Tel.Egram.Model.Messenger.Catalog.Entries;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.Model.Messenger.Catalog
{
    public class CatalogFilter
    {
        public IDisposable Bind(
            CatalogModel model,
            Section section)
        {
            return model.WhenAnyValue(m => m.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Accept(text =>
                {
                    var sorting = GetSorting(e => e.Order);
                    var filter = GetFilter(section);

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        sorting = GetSorting(e => e.Title);

                        filter = entry =>
                            entry.Title.Contains(text)
                            && GetFilter(section)(entry);
                    }

                    model.SortingController.OnNext(sorting);
                    model.FilterController.OnNext(filter);
                });
        }

        private static Func<EntryModel, bool> GetFilter(Section section)
        {
            return section switch
            {
                Section.Bots => BotFilter,
                Section.Channels => ChannelFilter,
                Section.Groups => GroupFilter,
                Section.Directs => DirectFilter,
                _ => All,
            };
        }

        private static IComparer<EntryModel> GetSorting(Func<EntryModel, IComparable> f)
        {
            return SortExpressionComparer<EntryModel>.Ascending(f);
        }

        private static bool All(EntryModel model)
        {
            return true;
        }

        private static bool BotFilter(EntryModel model)
        {
            if (model is ChatEntryModel chatEntryModel)
            {
                var chat = chatEntryModel.Chat;
                if (chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate)
                    return chat.User is {Type: TdApi.UserType.UserTypeBot _};
            }

            return false;
        }

        private static bool DirectFilter(EntryModel model)
        {
            if (model is ChatEntryModel chatEntryModel)
            {
                var chat = chatEntryModel.Chat;
                if (chat.ChatData.Type is TdApi.ChatType.ChatTypePrivate)
                    return chat.User is {Type: TdApi.UserType.UserTypeRegular _};
            }

            return false;
        }

        private static bool GroupFilter(EntryModel model)
        {
            if (model is ChatEntryModel chatEntryModel)
            {
                var chat = chatEntryModel.Chat;
                return chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType
                    ? !supergroupType.IsChannel
                    : chat.ChatData.Type is TdApi.ChatType.ChatTypeBasicGroup;
            }

            return false;
        }

        private static bool ChannelFilter(EntryModel model)
        {
            if (model is ChatEntryModel chatEntryModel)
            {
                var chat = chatEntryModel.Chat;
                if (chat.ChatData.Type is TdApi.ChatType.ChatTypeSupergroup supergroupType)
                {
                    return supergroupType.IsChannel;
                }
            }
            return false;
        }
    }
}