using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectorHost.Providers;
using ConnectorHost.Resources;

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
        /// Мессенджер 
        /// </summary>
        public string Messenger { get; set; }

        /// <summary>
        /// Объект по работе с классом Text (not good)
        /// </summary>
        private Text _text = new Text();
        /// <summary>
        /// Язык пользователя
        /// </summary>
        public Language Language { get; set; } = Language.Russian;
        /// <summary>
        /// Состояние пользователя
        /// </summary>
        public UserState State { get; set; }

        /// <summary>
        /// Идентификатор комнаты
        /// </summary>
        public string TeamId { get; set; }
       

        /// <summary>
        /// Обратный отправщик сообщений 
        /// </summary>
        public UsersService.SenderMessageDelagate Sender { get; set; }

        /// <summary>
        /// Проверка корректности id
        /// </summary>
        public UsersService.ExistIdTeamDelegate ExistIdTeam { get; set; }
        /// <summary>
        /// Получение цикла платформы
        /// </summary>
        public UsersService.GetCycleDelegate GetCycle { get; set; }

        #region Счётчики спама

        /// <summary>
        /// Счётчик ошибок во время ввода языка
        /// </summary>
        public int CountErrorSelectLenguage { get; set; } = 0;

        /// <summary>
        /// Счётчик ошибок во время ввода TeamId
        /// </summary>
        public int CountErrorSelectTeamId { get; set; } = 0;

        /// <summary>
        /// Счётчик подозрений на спам сообщениями
        /// </summary>
        public int CountSpeedMessage { get; set; } = 0;

        /// <summary>
        /// Цикл на котором произашло событие пользователя
        /// </summary>
        public int СycleEvent { get; set; }
        #endregion

        /// <summary>
        /// Последнее время активности
        /// </summary>
        public DateTime LastTimeActive { get; set; }
        /// <summary>
        /// Последняя активность в цикле
        /// </summary>
        public int LastCycleActive { get; set; }
        

        /// <summary>
        /// Входная точка для приёма сообщений
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        public async Task InPoint(Message message)
        {
            //Присвеваем время последней активности
            LastTimeActive = message.Date;
            //Присваеваем цикл временной активности
            LastCycleActive = GetCycle();
            #region Command Exit

            //Проверка на команды выходы из сессии
            var testExit = message.Text.ToLower();
            if (testExit == "exit")
            {
                await Sender(ContentToMessage(_text.GetText(NameText.ExitSession, Language)));
                //Перевод состояния в ожидание idTeam
                State = UserState.SpamGetIdTeam;
                return;
            }

            #endregion

            #region Время и Speed Messaging
            if (State == UserState.RouteMessage)
            {
                //Вычисление времени последней активности 
                var time = message.Date.Subtract(LastTimeActive);
                //Проверка высокой скорости отправки сообщений 
                //Чаще 1 сообщения в 15 секунд
                if (time.TotalSeconds < 15)
                {
                    //Увеличение счётчика нарушений
                    CountSpeedMessage++;
                    //Нарушение 3 раза
                    if (CountSpeedMessage > 3)
                    {
                        await Sender(ContentToMessage(_text.GetText(NameText.SpeedMessaging, Language)));
                    }
                    //Нарушение 5 раз
                    if (CountSpeedMessage > 5)
                    {
                        await Sender(ContentToMessage(_text.GetText(NameText.SpamSpeedMessaging, Language)));
                        State = UserState.SpamSpeedMessaging;
                        //Выход  из метода
                        return;
                    }
                }
                else
                {
                    //Обнуляем счётчик
                    CountSpeedMessage = 0;
                }
            }



            #endregion

           

            //Реализация хранения состояния пользователя
            switch (State)
            {
                case UserState.Started: //Старт
                    {
                        //Отправляем приветствие 
                        await Sender(ContentToMessage(_text.GetText(NameText.Hello, Language.Russian)));

                        //Отправляем просьбу выбрать язык
                        await Sender(ContentToMessage(_text.GetText(NameText.Language, Language.Russian)));

                        //Изменяем состояние в позицию ожидания цыфры языка
                        State = UserState.Selectlanguage;
                    }
                    break;
                case UserState.Selectlanguage: //Выбор языка
                    {
                        var text = message.Text.ToLower();
                        switch (text)
                        {
                            case "1":
                                {
                                    //Присваеваем значение языка
                                    Language = Language.Russian;
                                    await Sender(ContentToMessage(_text.GetText(NameText.SucessSelectLanguage, Language)));
                                    //Изменяем состояние в ожидание идентификатора Team
                                    State = UserState.SelectGetIdTeam;
                                    //Просьба прислать id Team
                                    await Sender(ContentToMessage(_text.GetText(NameText.GetIdTeam, Language)));
                                }
                                break;
                            case "2":
                                {
                                    //Присваеваем значение языка
                                    Language = Language.English;
                                    await Sender(ContentToMessage(_text.GetText(NameText.SucessSelectLanguage, Language)));
                                    //Изменяем состояние в ожидание идентификатора Team
                                    State = UserState.SelectGetIdTeam;
                                    //Просьба прислать id Team
                                    await Sender(ContentToMessage(_text.GetText(NameText.GetIdTeam, Language)));
                                }
                                break;
                            default:
                                {
                                    //На случай ошибки в выборе языка
                                    await Sender(ContentToMessage(_text.GetText(NameText.ErrorIdTeam, Language.Russian)));

                                    //Увеличиваем счётчик ошибок ввода языка
                                    CountErrorSelectLenguage++;
                                }
                                break;
                        }

                        //Проверка на спам во время выбора языка
                        if (CountErrorSelectLenguage > 5)
                        {
                            //Перевод в состояние бана по SpamSelectlanguage
                            State = UserState.SpamSelectlanguage;
                            //На случай ошибки в выборе языка
                            await Sender(ContentToMessage(_text.GetText(NameText.SpamSelectLanguage, Language.Russian)));
                            //Присваеваем занчние внцтреплатформенного цикла 
                            СycleEvent = GetCycle();
                        }
                    }
                    break;
                case UserState.SpamSelectlanguage: //Спам по выбору языка
                    {
                        await Sender(ContentToMessage(_text.GetText(NameText.SpamSelectLanguage, Language.Russian)));
                    }
                    break;
                case UserState.SelectGetIdTeam: //Ввод id
                    {
                        var text = message.Text.ToLower();
                        //Проверка присланного идентификтаора Team
                        if (await ExistIdTeam(text))
                        {
                            //Присваеваем idTeam
                            TeamId = text;
                            //Отправляем Сообщение что всё ок
                            await Sender(ContentToMessage(_text.GetText(NameText.SucessIdTeam, Language)));
                            //Переводим в режим роутинга сообщений на API
                            State = UserState.RouteMessage;
                        }
                        else
                        {
                            //Отправляем ошибку
                            await Sender(ContentToMessage(_text.GetText(NameText.ErrorIdTeam, Language)));
                            //Увеличиваем счётчик ошибок ввода id
                            CountErrorSelectTeamId++;
                        }

                        //Проверка на Spam
                        if (CountErrorSelectTeamId > 5)
                        {
                            //Отправляем сообщение с баном по вводу id
                            await Sender(ContentToMessage(_text.GetText(NameText.SpamIdTeam, Language)));
                            State = UserState.SpamGetIdTeam;
                            //Присваеваем занчние внцтреплатформенного цикла 
                            СycleEvent = GetCycle();
                        }

                    }
                    break;
                case UserState.SpamGetIdTeam: //Спам по вводу id
                    {
                        await Sender(ContentToMessage(_text.GetText(NameText.SpamIdTeam, Language)));
                    }
                    break;
                case UserState.RouteMessage://Роутинг сообщений в API
                    {
                        await Sender(ContentToMessage(new Content(){Text = message.Text}));
                        // TODO Добавить код
                    }
                    break;
                case UserState.SpamSpeedMessaging://Спам с высокой скоросью
                    {
                        await Sender(ContentToMessage(_text.GetText(NameText.SpamSpeedMessaging, Language)));
                    }
                    break;
            }
        }
        /// <summary>
        /// Метод закрытия сесси по времени 
        /// </summary>
        public async Task CloseTimeSession()
        {
            //Присваеваем состояние ожидания id Team
            State = UserState.SelectGetIdTeam;
            await Sender(ContentToMessage(_text.GetText(NameText.ExitTimeSession, Language)));

        }

        /// <summary>
        /// Конвертер Content to Message
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private Message ContentToMessage(Content content)
        {
            var message = new Message()
            {
                Id = Guid.NewGuid().ToString(),
                IdUser = Id,
                Date = DateTime.Now
            };
            if (content.Text != null)
                message.Text = content.Text;
            if (content.Attachments != null)
                message.Attachments = content.Attachments;
            return message;
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
        /// Spam во ремя выбора языка
        /// </summary>
        SpamSelectlanguage,
        /// <summary>
        /// Уточнение идентификатора комнаты
        /// </summary>
        SelectGetIdTeam,
        /// <summary>
        /// Spam идентификатор комнаты
        /// </summary>
        SpamGetIdTeam,
        /// <summary>
        /// Маршрутизация сообщений на Sender API
        /// </summary>
        RouteMessage,
        /// <summary>
        /// Спам по скорости отправки сообщений
        /// </summary>
        SpamSpeedMessaging
            

    }


}
