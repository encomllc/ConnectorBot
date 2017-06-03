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
        /// <param name="messenger">Мессенджер</param>
        /// <param name="sender">Делегат обратный отправщик сообщений</param>
        void AddUser(string id, string messenger, UsersService.SenderMessageDelagate sender);

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
        public delegate Task SenderMessageDelagate(Message message);

        public delegate Task<bool> ExistIdTeamDelegate(string idTeam);



        /// <summary>
        /// Коллекция для хранения позьзователей
        /// </summary>
        private readonly Dictionary<string, User> _usersDictionary = new Dictionary<string, User>();

        /// <summary>
        /// Список идентификаторов Team кеш
        /// </summary>
        private List<string> _idTeam = new List<string>(){"1"};

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
        /// <param name="messenger">Мессенджер</param>
        /// <param name="sender">Делегат обратный отправщик сообщений</param>
        public void AddUser(string id, string messenger, SenderMessageDelagate sender)
        {
            //Создание пользователя
            var user = new User
            {
                Id = id,
                Messenger = messenger,
                Sender = sender,
                State = UserState.Started,
                ExistIdTeam = ExistIdTeam
                
            };
            //Добавление в коллекцию
            _usersDictionary.Add(id,user);
        }
        /// <summary>
        /// Проверка наличия id team 
        /// </summary>
        /// <param name="idTeam"></param>
        /// <returns></returns>
        public async Task<bool> ExistIdTeam(string idTeam)
        {
            //Проверка наличия id в кеше
            if (_idTeam.Contains(idTeam))
            {
                return true;
            }
            else
            {
                //Получение id у API
                var id = await GetIdTeamApi(idTeam);
                //Проверка на корректность если введённый равен полученному то ОК
                if (id == idTeam)
                {
                    _idTeam.Add(id);
                    return true;
                }
            }
           
            return false;
        }

        /// <summary>
        /// Заглушка для полноценной проверки у API
        /// </summary>
        /// <param name="idTeam"></param>
        /// <returns></returns>
        public async Task<string> GetIdTeamApi(string idTeam)
        {
            return "not exist";
        }

    }
}
