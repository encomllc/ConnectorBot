using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectorHost.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;

namespace ConnectorHost.Controllers
{
    /// <summary>
    /// Контроллер приёма сообщений
    /// </summary>
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        /// <summary>
        /// Конструктор контроллера
        /// </summary>
        /// <param name="usersesService"></param>
        /// <param name="bfService"></param>
        /// <param name="configuration"></param>
        public MessagesController(IUsersService usersesService, IBotFramework bfService, IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _appCredentials = new MicrosoftAppCredentials(this._configuration);
            _usersService = usersesService;
            _bfService = bfService;
        }
        /// <summary>
        /// Экземпляр сервиса с конфигурацией 
        /// </summary>
        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Сертификат microsoft bot framework
        /// </summary>
        private MicrosoftAppCredentials _appCredentials;

        /// <summary>
        /// Users Service
        /// </summary>
        private readonly IUsersService _usersService;

        #region Services Provider

        /// <summary>
        /// Bot Framework Provider Service
        /// </summary>
        private readonly IBotFramework _bfService;

        #endregion

        /// <summary>
        /// Конечная точка для сообщений из Microsoft Bot Framework
        /// </summary>
        /// <param name="activity">Объект сообщения</param>
        /// <returns></returns>
        [HttpPost]
        [Route("botframework")]
        public virtual async Task<OkResult> PostBotFramework([FromBody]Activity activity) 
        {
            //Проверка наличия Client Provider
            if (!_bfService.ExistProviderClient(activity.Conversation.Id, activity.ChannelId,
                activity.From.Id))
            {
                _bfService.AddProviderClient(new Uri(activity.ServiceUrl), _appCredentials, activity.From, activity.Recipient, activity.Conversation.Id, activity.ChannelId, activity.From.Id);
                //Создание id по шаблону
                var id = _bfService.CreateIdentificator(activity.Conversation.Id, activity.ChannelId,
                activity.From.Id);
                //Проверка наличия пользовалея
                if (!_usersService.ExistUser(id))
                {
                    //Добавление пользователя
                    _usersService.AddUser(id, activity.ChannelId, _bfService.SendMessage);
                }
            }

            //if message
            if (activity.Type == ActivityTypes.Message)
            {
                //Генерация id
                var id = _bfService.CreateIdentificator(activity.Conversation.Id, activity.ChannelId,
                    activity.From.Id);
               
                //Отправка сообщения на обработку
                //Можно поставить выполнение в ассинхронном режиме
               await _usersService.GetUser(id).InPoint(_bfService.ActivityToMessage(activity, id));
            }
            return Ok();
        }

        [HttpPost]
        [Route("viber")]
        public virtual async Task<OkResult> PostViber([FromBody]object activity)
        {
            //if (!_botService.ExistBotClient(activity.Conversation.Id, activity.ChannelId, activity.From.Id))
            //{
            //    _botService.AddBotClient(new Uri(activity.ServiceUrl), _appCredentials, activity.From, activity.Recipient, activity.Conversation.Id, activity.ChannelId, activity.From.Id);
            //}

            //if (activity.Type == ActivityTypes.Message)
            //{
            //    //Activity reply = activity.CreateReply($"You sent {activity.Text} which was  characters");
            //    //var connector = new ConnectorClient(new Uri(activity.ServiceUrl), _appCredentials);
            //    //await connector.Conversations.ReplyToActivityAsync(reply);

            //    //Отправка сообщения на обработку
            //    _botService.InMethod(activity);
            //}
            //else
            //{
            //    // reply.Text = $"activity type: {activity.Type}";
            //}
            return Ok();
        }

        [HttpPost]
        [Route("vkontakte")]
        public virtual async Task<OkResult> PostVKontakte([FromBody]object activity)
        {
            //if (!_botService.ExistBotClient(activity.Conversation.Id, activity.ChannelId, activity.From.Id))
            //{
            //    _botService.AddBotClient(new Uri(activity.ServiceUrl), _appCredentials, activity.From, activity.Recipient, activity.Conversation.Id, activity.ChannelId, activity.From.Id);
            //}

            //if (activity.Type == ActivityTypes.Message)
            //{
            //    //Activity reply = activity.CreateReply($"You sent {activity.Text} which was  characters");
            //    //var connector = new ConnectorClient(new Uri(activity.ServiceUrl), _appCredentials);
            //    //await connector.Conversations.ReplyToActivityAsync(reply);

            //    //Отправка сообщения на обработку
            //    _botService.InMethod(activity);
            //}
            //else
            //{
            //    // reply.Text = $"activity type: {activity.Type}";
            //}
            return Ok();
        }

        [HttpPost]
        [Route("sms")]
        public virtual async Task<OkResult> PostSms([FromBody]object activity)
        {
            //if (!_botService.ExistBotClient(activity.Conversation.Id, activity.ChannelId, activity.From.Id))
            //{
            //    _botService.AddBotClient(new Uri(activity.ServiceUrl), _appCredentials, activity.From, activity.Recipient, activity.Conversation.Id, activity.ChannelId, activity.From.Id);
            //}

            //if (activity.Type == ActivityTypes.Message)
            //{
            //    //Activity reply = activity.CreateReply($"You sent {activity.Text} which was  characters");
            //    //var connector = new ConnectorClient(new Uri(activity.ServiceUrl), _appCredentials);
            //    //await connector.Conversations.ReplyToActivityAsync(reply);

            //    //Отправка сообщения на обработку
            //    _botService.InMethod(activity);
            //}
            //else
            //{
            //    // reply.Text = $"activity type: {activity.Type}";
            //}
            return Ok();
        }



        #region Demo

        //// GET api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        #endregion
    }
}
