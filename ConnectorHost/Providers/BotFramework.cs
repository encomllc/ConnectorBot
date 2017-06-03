using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace ConnectorHost.Providers
{
    public interface IBotFramework
    {
        /// <summary>
        /// Добавить провайдера Bot Framework
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="appCredentials"></param>
        /// <param name="userAccount"></param>
        /// <param name="botAccount"></param>
        /// <param name="conversationId"></param>
        /// <param name="channelId"></param>
        /// <param name="fromId"></param>
        void AddProviderClient(Uri baseUri, MicrosoftAppCredentials appCredentials, ChannelAccount userAccount, ChannelAccount botAccount, string conversationId, string channelId, string fromId);

        /// <summary>
        /// Проверка наличия Client Provider
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="channelId"></param>
        /// <param name="fromId"></param>
        /// <returns></returns>
        bool ExistProviderClient(string conversationId, string channelId, string fromId);

        /// <summary>
        /// Создание идентификатора
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="channelId"></param>
        /// <param name="fromId"></param>
        /// <returns></returns>
        string CreateIdentificator(string conversationId, string channelId, string fromId);

        /// <summary>
        /// Проактивная отправка сообщения 
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        Task SendMessage(Message message);

        Message ActivityToMessage(Activity activity, string idUser);
    }

    /// <summary>
    /// Microsoft Bot Framework
    /// </summary>
    public class BotFramework : IBotFramework
    {
        /// <summary>
        /// Список для хранения клиентов
        /// </summary>
        private Dictionary<string, BotClient> ProviderClients { get; set; } = new Dictionary<string, BotClient>();

        /// <summary>
        /// Добавить провайдера Bot Framework
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="appCredentials"></param>
        /// <param name="userAccount"></param>
        /// <param name="botAccount"></param>
        /// <param name="conversationId"></param>
        /// <param name="channelId"></param>
        /// <param name="fromId"></param>
        public void AddProviderClient(Uri baseUri, MicrosoftAppCredentials appCredentials, ChannelAccount userAccount, ChannelAccount botAccount, string conversationId, string channelId, string fromId)
        {
            var botClient = new BotClient();
            botClient.AppCredentials = appCredentials;
            botClient.ConnectorClient = new ConnectorClient(baseUri, appCredentials);
            botClient.BotAccount = botAccount;
            botClient.UserAccount = userAccount;
            botClient.ChannelId = channelId;
            botClient.ConversationId = conversationId;
            botClient.FromId = fromId;
            ProviderClients.Add(CreateIdentificator(conversationId, channelId, fromId), botClient);
        }

        /// <summary>
        /// Проверка наличия Client Provider
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="channelId"></param>
        /// <param name="fromId"></param>
        /// <returns></returns>
        public bool ExistProviderClient(string conversationId, string channelId, string fromId)
        {
            return ProviderClients.ContainsKey(conversationId + "|" + channelId + "|" + fromId);
        }
        /// <summary>
        /// Создание идентификатора
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="channelId"></param>
        /// <param name="fromId"></param>
        /// <returns></returns>
        public string CreateIdentificator(string conversationId, string channelId, string fromId)
        {
            return conversationId + "|" + channelId + "|" + fromId;
        }
        /// <summary>
        /// Проактивная отправка сообщения 
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        public async Task SendMessage(Message message)
        {
            IMessageActivity activity = Activity.CreateMessageActivity();
            // message.From = new ChannelAccount();
            activity.From = ProviderClients[message.IdUser].BotAccount;
            // message.Recipient = new ChannelAccount();
            activity.Recipient = ProviderClients[message.IdUser].UserAccount;
            activity.Conversation = new ConversationAccount(id: ProviderClients[message.IdUser].ConversationId);

            //Заглушка для создания custome Attachments message
            //switch (ProviderClients[message.IdUser].ChannelId)
            //{
            //    case "emulator":
            //        { }
            //        break;
            //}

            //Загрузка вложений
            if (message.Attachments != null)
            {
                //activity.Attachments = new Attachments();
                foreach (var messageAttachment in message.Attachments)
                {
                    var attachment = new Attachment();
                    attachment.ContentType = messageAttachment.ContentType;
                    attachment.ContentUrl = messageAttachment.ContentUrl;
                    attachment.Name = messageAttachment.Name;

                    activity.Attachments.Add(attachment);
                }
            }

            //Текст
            activity.Text = message.Text;

            // message.Locale = "en-Us";
            await ProviderClients[message.IdUser].ConnectorClient.Conversations.SendToConversationAsync((Activity)activity);
        }

        public Message ActivityToMessage(Activity activity, string idUser)
        {
            var message = new Message();
            message.Id = activity.Id;
            message.IdUser = idUser;
            //Текст
            if (activity.Text != null)
                message.Text = activity.Text;
            //Дата
            if (activity.Timestamp != null)
                message.Date = (DateTime)activity.Timestamp;
            //Мессенджер
            message.Messanger = activity.ChannelId;
            //Вложения
            if (activity.Attachments != null)
            {
                message.Attachments = new List<Message.Attachment>();
                foreach (var activityAttachment in activity.Attachments)
                {
                    var attachment = new Message.Attachment();
                    attachment.ContentType = activityAttachment.ContentType;
                    attachment.ContentUrl = activityAttachment.ContentUrl;
                    attachment.Name = activityAttachment.Name;
                }
            }

            return message;
        }

        /// <summary>
        /// Технической класс для работыс Bot Framework
        /// </summary>
        public class BotClient
        {
            public BotClient()
            {
                //var conversationId = await ConnectorClient.Conversations.CreateDirectConversationAsync(BotAccount, UserAccount);
                //conversationId.Id
            }
            public MicrosoftAppCredentials AppCredentials { get; set; } = new MicrosoftAppCredentials();
            public ChannelAccount UserAccount { get; set; } = new ChannelAccount();
            public ChannelAccount BotAccount { get; set; } = new ChannelAccount();
            public ConnectorClient ConnectorClient { get; set; }
            public string ConversationId { get; set; }
            public string ChannelId { get; set; }
            public string FromId { get; set; }
        }

    }
}
