using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ConnectorHost.Controllers
{
    /// <summary>
    /// Контроллер приёма сообщений
    /// </summary>
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        [HttpPost]
        [Route("BotFramework")]
        public virtual async Task<OkResult> Post([FromBody]object activity)
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
