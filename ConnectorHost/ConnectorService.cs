using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectorHost.Providers;

namespace ConnectorHost
{
    public interface IConnectorService
    {
        /// <summary>
        /// Bot Framework Provider
        /// </summary>
        BotFramework BotFrameworkProvider { get; set; }

        /// <summary>
        /// Проверка наличия пользователя в коллекции.
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        bool ExistUser(string id);

        /// <summary>
        /// Получление объект пользователь
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User GetUser(string id);

        /// <summary>
        /// Добавление пользовалтеля
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="provider">provider</param>
        /// <param name="information">Техническая информация</param>
        void AddUser(string id, Provider provider, object information);
    }

    /// <summary>
    /// Connector Service
    /// </summary>
    public class ConnectorService : IConnectorService
    {
        /// <summary>
        ///Делегат проброски проактивной отправки сообщений
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public delegate Task SenderMessage(Message message);

        #region Провайдеры

        /// <summary>
        /// Bot Framework Provider
        /// </summary>
        public BotFramework BotFrameworkProvider { get; set; } = new BotFramework();

        #endregion

        /// <summary>
        /// Коллекция для хранения позьзователей
        /// </summary>
        private readonly Dictionary<string, User> _usersDictionary = new Dictionary<string, User>();

        /// <summary>
        /// Проверка наличия пользователя в коллекции.
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public bool ExistUser(string id)
        {
            return _usersDictionary.ContainsKey(id);
        }
        /// <summary>
        /// Получление объект пользователь
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUser(string id)
        {
            return _usersDictionary[id];
        }
        /// <summary>
        /// Добавление пользовалтеля
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="provider">provider</param>
        /// <param name="information">Техническая информация</param>
        public void AddUser(string id, Provider provider, object information)
        {
            //Создание пользователя
            var user = new User();
            //Добавление в коллекцию
            _usersDictionary.Add(id,user);
        }
    }
}
