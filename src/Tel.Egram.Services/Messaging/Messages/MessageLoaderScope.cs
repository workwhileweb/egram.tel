using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Messages
{
    /// <summary>
    /// Caches execution results for the scope of request
    /// </summary>
    public class MessageLoaderScope : IDisposable
    {
        private readonly IAgent _agent;

        private readonly ConcurrentDictionary<int, TdApi.User> _users
            = new ConcurrentDictionary<int, TdApi.User>();

        private readonly ConcurrentDictionary<long, TdApi.Chat> _chats
            = new ConcurrentDictionary<long, TdApi.Chat>();

        private readonly ConcurrentDictionary<(long, long), TdApi.Message> _messages
            = new ConcurrentDictionary<(long, long), TdApi.Message>();

        public MessageLoaderScope(IAgent agent)
        {
            _agent = agent;
        }

        public IObservable<TdApi.User> GetUser(int userId)
        {
            return _users.TryGetValue(userId, out var user)
                ? Observable.Return(user)
                : _agent.Execute(new TdApi.GetUser
            {
                UserId = userId
            })
                .Do(u =>
                {
                    _users.GetOrAdd(userId, u);
                });
        }

        public IObservable<TdApi.Chat> GetChat(long chatId)
        {
            return _chats.TryGetValue(chatId, out var chat)
                ? Observable.Return(chat)
                : _agent.Execute(new TdApi.GetChat
            {
                ChatId = chatId
            })
                .Do(c =>
                {
                    _chats.GetOrAdd(chatId, c);
                });
        }

        public IObservable<TdApi.Message> GetMessage(long chatId, long messageId)
        {
            return _messages.TryGetValue((chatId, messageId), out var message)
                ? Observable.Return(message)
                : _agent.Execute(new TdApi.GetMessage
            {
                ChatId = chatId,
                MessageId = messageId
            })
                .Do(m =>
                {
                    _messages.GetOrAdd((chatId, messageId), m);
                });
        }

        public void Dispose()
        {
            _users.Clear();
            _chats.Clear();
            _messages.Clear();
        }
    }
}