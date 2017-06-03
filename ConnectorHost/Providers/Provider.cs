using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectorHost.Providers
{
    /// <summary>
    /// Перечисление провайдеров, мессенджеров
    /// </summary>
    public enum Providers
    {
        /// <summary>
        /// Microsoft Bot Framework
        /// </summary>
        BotFramework,
        /// <summary>
        /// Viber
        /// </summary>
        Viber,
        /// <summary>
        /// Вконтакте 
        /// </summary>
        Vkontakte,
        /// <summary>
        /// Смс
        /// </summary>
        Sms
    }
}
