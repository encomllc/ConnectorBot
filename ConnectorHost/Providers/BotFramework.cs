using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace ConnectorHost.Providers
{
    /// <summary>
    /// Microsoft Bot Framework
    /// </summary>
    public class BotFramework
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
            ProviderClients.Add(CreateIdentificator(conversationId, channelId,fromId), botClient);
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

            //if (message.Attachments != null)
            //{
            //    message.ChannelData = message.Attachments;
            //}
            //else
            //{
            activity.Text = message.Text;
            //}

            // message.Locale = "en-Us";
            await ProviderClients[message.IdUser].ConnectorClient.Conversations.SendToConversationAsync((Activity)activity);
            //await SendMessageActivity(message.IdUser, (Activity)activity);
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
