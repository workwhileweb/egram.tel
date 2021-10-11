using System.Linq;
using TdLib;
using Tel.Egram.Model.Messenger.Explorer.Messages;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Utils.Formatting;

namespace Tel.Egram.Model.Messenger.Explorer.Factories
{
    public class MessageModelFactory : IMessageModelFactory
    {
        private readonly IBasicMessageModelFactory _basicMessageModelFactory;
        private readonly INoteMessageModelFactory _noteMessageModelFactory;
        private readonly ISpecialMessageModelFactory _specialMessageModelFactory;

        private readonly IStringFormatter _stringFormatter;
        private readonly IVisualMessageModelFactory _visualMessageModelFactory;

        public MessageModelFactory(
            IBasicMessageModelFactory basicMessageModelFactory,
            INoteMessageModelFactory noteMessageModelFactory,
            ISpecialMessageModelFactory specialMessageModelFactory,
            IVisualMessageModelFactory visualMessageModelFactory,
            IStringFormatter stringFormatter)
        {
            _basicMessageModelFactory = basicMessageModelFactory;
            _noteMessageModelFactory = noteMessageModelFactory;
            _specialMessageModelFactory = specialMessageModelFactory;
            _visualMessageModelFactory = visualMessageModelFactory;

            _stringFormatter = stringFormatter;
        }

        public MessageModel CreateMessage(Message message)
        {
            var model = GetMessage(message);
            ApplyMessageAttributes(model, message);
            return model;
        }

        private MessageModel GetMessage(Message message)
        {
            var messageData = message.MessageData;
            var content = messageData.Content;

            return content switch
            {
                // basic
                TdApi.MessageContent.MessageText messageText => _basicMessageModelFactory.CreateTextMessage(message,
                    messageText),
                // visual
                TdApi.MessageContent.MessagePhoto messagePhoto => _visualMessageModelFactory.CreatePhotoMessage(message,
                    messagePhoto),
                TdApi.MessageContent.MessageExpiredPhoto expiredPhoto => _visualMessageModelFactory
                    .CreateExpiredPhotoMessage(message, expiredPhoto),
                TdApi.MessageContent.MessageSticker messageSticker => _visualMessageModelFactory.CreateStickerMessage(
                    message, messageSticker),
                TdApi.MessageContent.MessageAnimation messageAnimation => _visualMessageModelFactory
                    .CreateAnimationMessage(message, messageAnimation),
                TdApi.MessageContent.MessageVideo messageVideo => _visualMessageModelFactory.CreateVideoMessage(message,
                    messageVideo),
                TdApi.MessageContent.MessageExpiredVideo expiredVideo => _visualMessageModelFactory
                    .CreateExpiredVideoMessage(message, expiredVideo),
                TdApi.MessageContent.MessageVideoNote videoNote => _visualMessageModelFactory.CreateVideoNoteMessage(
                    message, videoNote),
                // special
                TdApi.MessageContent.MessageDocument messageDocument => _specialMessageModelFactory
                    .CreateDocumentMessage(message, messageDocument),
                TdApi.MessageContent.MessageAudio messageAudio => _specialMessageModelFactory.CreateAudioMessage(
                    message, messageAudio),
                TdApi.MessageContent.MessageVoiceNote voiceNote => _specialMessageModelFactory.CreateVoiceNoteMessage(
                    message, voiceNote),
                TdApi.MessageContent.MessagePaymentSuccessful paymentSuccessful => _specialMessageModelFactory
                    .CreatePaymentSuccessfulMessage(message, paymentSuccessful),
                TdApi.MessageContent.MessagePaymentSuccessfulBot paymentSuccessfulBot => _specialMessageModelFactory
                    .CreatePaymentSuccessfulBotMessage(message, paymentSuccessfulBot),
                TdApi.MessageContent.MessageLocation location => _specialMessageModelFactory.CreateLocationMessage(
                    message, location),
                TdApi.MessageContent.MessageVenue venue => _specialMessageModelFactory.CreateVenueMessage(message,
                    venue),
                TdApi.MessageContent.MessageContact contact => _specialMessageModelFactory.CreateContactMessage(message,
                    contact),
                TdApi.MessageContent.MessageGame game => _specialMessageModelFactory.CreateGameMessage(message, game),
                TdApi.MessageContent.MessageGameScore gameScore => _specialMessageModelFactory.CreateGameScoreMessage(
                    message, gameScore),
                TdApi.MessageContent.MessageInvoice invoice => _specialMessageModelFactory.CreateInvoiceMessage(message,
                    invoice),
                TdApi.MessageContent.MessagePassportDataSent passportDataSent => _specialMessageModelFactory
                    .CreatePassportDataSentMessage(message, passportDataSent),
                TdApi.MessageContent.MessagePassportDataReceived passportDataReceived => _specialMessageModelFactory
                    .CreatePassportDataReceivedMessage(message, passportDataReceived),
                TdApi.MessageContent.MessageContactRegistered contactRegistered => _specialMessageModelFactory
                    .CreateContactRegisteredMessage(message, contactRegistered),
                TdApi.MessageContent.MessageWebsiteConnected websiteConnected => _specialMessageModelFactory
                    .CreateWebsiteConnectedMessage(message, websiteConnected),
                // notes
                TdApi.MessageContent.MessageCall messageCall => _noteMessageModelFactory.CreateCallMessage(message,
                    messageCall),
                TdApi.MessageContent.MessageBasicGroupChatCreate basicGroupChatCreate => _noteMessageModelFactory
                    .CreateBasicGroupChatCreateMessage(message, basicGroupChatCreate),
                TdApi.MessageContent.MessageChatChangeTitle chatChangeTitle => _noteMessageModelFactory
                    .CreateChatChangeTitleMessage(message, chatChangeTitle),
                TdApi.MessageContent.MessageChatChangePhoto chatChangePhoto => _noteMessageModelFactory
                    .CreateChatChangePhotoMessage(message, chatChangePhoto),
                TdApi.MessageContent.MessageChatDeletePhoto chatDeletePhoto => _noteMessageModelFactory
                    .CreateChatDeletePhotoMessage(message, chatDeletePhoto),
                TdApi.MessageContent.MessageChatAddMembers chatAddMembers => _noteMessageModelFactory
                    .CreateChatAddMembersMessage(message, chatAddMembers),
                TdApi.MessageContent.MessageChatJoinByLink chatJoinByLink => _noteMessageModelFactory
                    .CreateChatJoinByLinkMessage(message, chatJoinByLink),
                TdApi.MessageContent.MessageChatDeleteMember chatDeleteMember => _noteMessageModelFactory
                    .CreateChatDeleteMemberMessage(message, chatDeleteMember),
                TdApi.MessageContent.MessageChatUpgradeTo chatUpgradeTo => _noteMessageModelFactory
                    .CreateChatUpgradeToMessage(message, chatUpgradeTo),
                TdApi.MessageContent.MessageChatUpgradeFrom chatUpgradeFrom => _noteMessageModelFactory
                    .CreateChatUpgradeFromMessage(message, chatUpgradeFrom),
                TdApi.MessageContent.MessagePinMessage pinMessage => _noteMessageModelFactory.CreatePinMessageMessage(
                    message, pinMessage),
                TdApi.MessageContent.MessageScreenshotTaken screenshotTaken => _noteMessageModelFactory
                    .CreateScreenshotTakenMessage(message, screenshotTaken),
                TdApi.MessageContent.MessageChatSetTtl chatSetTtl => _noteMessageModelFactory.CreateChatSetTtlMessage(
                    message, chatSetTtl),
                TdApi.MessageContent.MessageCustomServiceAction customServiceAction => _noteMessageModelFactory
                    .CreateCustomServiceActionMessage(message, customServiceAction),
                _ => _basicMessageModelFactory.CreateUnsupportedMessage(message)
            };
        }

        private void ApplyMessageAttributes(MessageModel model, Message message)
        {
            var user = message.UserData;
            var chat = message.ChatData;

            var authorName = user == null
                ? chat.Title
                : $"{user.FirstName} {user.LastName}";

            model.Message = message;
            model.AuthorName = authorName;
            model.Time = _stringFormatter.FormatShortTime(message.MessageData.Date);

            if (message.ReplyMessage == null) return;
            model.HasReply = true;
            model.Reply = new ReplyModel
            {
                Message = message.ReplyMessage,
                AuthorName = GetReplyAuthorName(message.ReplyMessage),
                Text = GetReplyText(message.ReplyMessage),
                PhotoData = GetReplyPhoto(message.ReplyMessage),
                VideoData = GetReplyVideo(message.ReplyMessage),
                StickerData = GetReplySticker(message.ReplyMessage)
            };
        }

        private string GetReplyAuthorName(Message message)
        {
            var replyUser = message.UserData;
            var replyChat = message.ChatData;

            var replyAuthorName = replyUser == null
                ? replyChat.Title
                : $"{replyUser.FirstName} {replyUser.LastName}";

            return replyAuthorName;
        }

        private string GetReplyText(Message message)
        {
            var messageData = message.MessageData;
            var content = messageData.Content;

            var text = content switch
            {
                TdApi.MessageContent.MessageText messageText => messageText.Text?.Text,
                TdApi.MessageContent.MessagePhoto messagePhoto => messagePhoto.Caption?.Text,
                _ => null
            };

            return text == null
                ? text
                : new string(
                text.Take(64)
                    .TakeWhile(c => c != '\n' && c != '\r')
                    .ToArray());
        }

        private TdApi.Photo GetReplyPhoto(Message message)
        {
            return message.MessageData.Content is TdApi.MessageContent.MessagePhoto messagePhoto ? messagePhoto.Photo : null;
        }

        private TdApi.Video GetReplyVideo(Message message)
        {
            return message.MessageData.Content is TdApi.MessageContent.MessageVideo messageVideo ? messageVideo.Video : null;
        }

        private TdApi.Sticker GetReplySticker(Message message)
        {
            return message.MessageData.Content is TdApi.MessageContent.MessageSticker messageSticker ? messageSticker.Sticker : null;
        }
    }
}