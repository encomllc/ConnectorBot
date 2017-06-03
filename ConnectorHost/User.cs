using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectorHost
{
    /// <summary>
    /// Объект пользователь 
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string Id { get; set; }  

        /// <summary>
        /// Обратный отправщик сообщений 
        /// </summary>
        public ConnectorService.SenderMessage Sender { get; set; }
        /// <summary>
        /// Входная точка для приёма сообщений
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        public async Task InPoint(object message)
        {
            
        }
        
    }
}
