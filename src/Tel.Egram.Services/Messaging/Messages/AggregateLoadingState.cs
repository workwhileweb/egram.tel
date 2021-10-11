using System.Collections.Generic;
using TdLib;

namespace Tel.Egram.Services.Messaging.Messages
{
    public class AggregateLoadingState
    {
        private readonly Dictionary<long, long> _lastMessages;
        private readonly Dictionary<long, Stack<TdApi.Message>> _stackedMessages;

        public AggregateLoadingState()
        {
            _lastMessages = new Dictionary<long, long>();
            _stackedMessages = new Dictionary<long, Stack<TdApi.Message>>();
        }

        public int CountStackedMessages(long chatId)
        {
            return _stackedMessages.TryGetValue(chatId, out var stack) ? stack.Count : 0;
        }

        public TdApi.Message PopMessageFromStack(long chatId)
        {
            return _stackedMessages.TryGetValue(chatId, out var stack) ? stack.Pop() : null;
        }

        public void PushMessageToStack(long chatId, TdApi.Message message)
        {
            if (!_stackedMessages.TryGetValue(chatId, out var stack))
            {
                stack = new Stack<TdApi.Message>();
                _stackedMessages.Add(chatId, stack);
            }

            stack.Push(message);
        }

        public long GetLastMessageId(long chatId)
        {
            _lastMessages.TryGetValue(chatId, out var messageId);
            return messageId;
        }

        public void SetLastMessageId(long chatId, long messageId)
        {
            _lastMessages[chatId] = messageId;
        }
    }
}