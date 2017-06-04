using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// Конструктор UsersService
        /// </summary>
        public UsersService()
        {
            //Задаёт параметр времени циклов
            var timer = new TimeSpan(0, 1, 0);
            CycleTimer = new Timer(TimerCallback, Cycle, timer, timer);
        }


        #region Делегаты

        /// <summary>
        ///Делегат проброски проактивной отправки сообщений
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public delegate Task SenderMessageDelagate(Message message);
        /// <summary>
        /// Делегат для проверки team id API 
        /// </summary>
        /// <param name="idTeam"></param>
        /// <returns></returns>
        public delegate Task<bool> ExistIdTeamDelegate(string idTeam);
        /// <summary>
        /// Делегат для получения пользлователем цикла платформы
        /// </summary>
        /// <returns></returns>
        public delegate int GetCycleDelegate();

        #endregion

        /// <summary>
        /// Коллекция для хранения позьзователей
        /// </summary>
        private readonly Dictionary<string, User> _usersDictionary = new Dictionary<string, User>();

        /// <summary>
        /// Список идентификаторов Team кеш
        /// </summary>
        private List<string> _idTeam = new List<string>() { "1" };

        /// <summary>
        /// Таймер внутриплатформенных циклов
        /// </summary>
        public Timer CycleTimer;

        public int Cycle { get; set; }


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
                ExistIdTeam = ExistIdTeam,
                GetCycle = GetCycle
            };
            //Добавление в коллекцию
            _usersDictionary.Add(id, user);
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
        /// Получить текущий цикл платформы
        /// </summary>
        /// <returns></returns>
        public int GetCycle()
        {
            return Cycle;
        }
        /// <summary>
        /// Событие срабатываение таймера
        /// </summary>
        /// <param name="sender"></param>
        public void TimerCallback(object sender)
        {
            //Увеличиваем цикл
            Cycle++;

            //Разбан spam Select Language
            foreach (var user in _usersDictionary.Where(x => x.Value.State == UserState.SpamSelectlanguage))
            {
                //+6 = 30 минут
                //+12 = 60 минут
                if (user.Value.СycleEvent + 6 < Cycle)
                    user.Value.State = UserState.SelectGetIdTeam;
            }
            //Разбан spam Get Id Team
            foreach (var user in _usersDictionary.Where(x=>x.Value.State== UserState.SpamGetIdTeam))
            {
                //+6 = 30 минут
                //+12 = 60 минут
                if (user.Value.СycleEvent + 12 < Cycle)
                    user.Value.State = UserState.SelectGetIdTeam;
            }

            //Разбан spam Speed Messaging 
            foreach (var user in _usersDictionary.Where(x=>x.Value.State== UserState.SpamSpeedMessaging))
            {   
                //+3 = 15 минут
                //+6 = 30 минут
                //+12 = 60 минут
                if (user.Value.СycleEvent + 3 < Cycle)
                    user.Value.State = UserState.SelectGetIdTeam;
            }

            //Закрытие Routing сессий в которых нет активности на протяжении 30 минут
            foreach (var user in _usersDictionary.Values.Where(x=>x.LastCycleActive+6<Cycle&& x.State== UserState.RouteMessage))
            {
                user.CloseTimeSession().Wait();
            }
                
           
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
