using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectorHost.Providers
{
    /// <summary>
    /// User Service Interface 
    /// </summary>
    public interface IUsersService
    {

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
        /// <param name="sender">Делегат обратный отправщик сообщений</param>
        void AddUser(string id, Providers provider, UsersService.SenderMessage sender);
    }

    /// <summary>
    /// User Service
    /// </summary>
    public class UsersService : IUsersService
    {
        /// <summary>
        ///Делегат проброски проактивной отправки сообщений
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public delegate Task SenderMessage(Message message);

        

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
        /// <param name="sender">Делегат обратный отправщик сообщений</param>
        public void AddUser(string id, Providers provider, SenderMessage sender)
        {
            //Создание пользователя
            var user = new User
            {
                Id = id,
                Provider = provider,
                Sender = sender
            };
            //Добавление в коллекцию
            _usersDictionary.Add(id,user);
        }
    }
}
