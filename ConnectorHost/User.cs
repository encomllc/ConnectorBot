using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectorHost.Providers;

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
        /// Провайдер 
        /// </summary>
        public Providers.Providers Provider { get; set; }

        /// <summary>
        /// Обратный отправщик сообщений 
        /// </summary>
        public UsersService.SenderMessage Sender { get; set; }
        /// <summary>
        /// Язык пользователя
        /// </summary>
        public Language Language { get; set; }

        /// <summary>
        /// Входная точка для приёма сообщений
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        public async Task InPoint(Message message)
        {
            
        }
    }
    /// <summary>
    /// Перечисление языков
    /// </summary>
    public enum Language
    {
        Russian, English
    }

    /// <summary>
    /// Внутренние состояния объекта User
    /// </summary>
    public enum UserState
    {
        /// <summary>
        /// Старт взаимодействия с пользователем
        /// </summary>
        Started,
        /// <summary>
        /// Выбор языка
        /// </summary>
        Selectlanguage,
        /// <summary>
        /// Маршрутизация сообщений на Sender API
        /// </summary>
        RouteMessage
    }


}
