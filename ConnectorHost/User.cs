﻿using System;
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
        public Language Language { get; set; }
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

        #region Счётчики спама

        //Счётчик ошибок во время ввода языка
        public int CountErrorSelectLenguage { get; set; } = 0;

        //Счётчик ошибок во время ввода TeamId
        public int CountErrorSelectTeamId { get; set; } = 0;

        //Счётчик подозрений на спам сообщениями
        public int CountSpeedMessage { get; set; } = 0;
        //Последнее время активности
        public DateTime LastTimeActive { get; set; } 

        #endregion



        /// <summary>
        /// Входная точка для приёма сообщений
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns></returns>
        public async Task InPoint(Message message)
        {
            var testExit = message.Text.ToLower();
            //Реализация хранения состояния пользователя
            switch (State)
            {
                case UserState.Started: //Старт
                    {
                        //Отправляем приветствие 
                        await Sender(ContentToMessage(_text.GetText(NemeText.Hello, Language.Russian)));

                        //Отправляем просьбу выбрать язык
                        await Sender(ContentToMessage(_text.GetText(NemeText.Language, Language.Russian)));

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
                                    await Sender(ContentToMessage(_text.GetText(NemeText.SucessSelectLanguage, Language)));
                                    //Изменяем состояние в ожидание идентификатора Team
                                    State = UserState.SelectGetIdTeam;
                                    //Просьба прислать id Team
                                    await Sender(ContentToMessage(_text.GetText(NemeText.GetIdTeam, Language)));
                                }
                                break;
                            case "2":
                                {
                                    //Присваеваем значение языка
                                    Language = Language.English;
                                    await Sender(ContentToMessage(_text.GetText(NemeText.SucessSelectLanguage, Language)));
                                    //Изменяем состояние в ожидание идентификатора Team
                                    State = UserState.SelectGetIdTeam;
                                    //Просьба прислать id Team
                                    await Sender(ContentToMessage(_text.GetText(NemeText.GetIdTeam, Language)));
                                }
                                break;
                            default:
                                {
                                    //На случай ошибки в выборе языка
                                    await Sender(ContentToMessage(_text.GetText(NemeText.ErrorIdTeam, Language.Russian)));

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
                            await Sender(ContentToMessage(_text.GetText(NemeText.SpamSelectLanguage, Language.Russian)));
                        }
                    }
                    break;
                case UserState.SpamSelectlanguage: //Спам по выбору языка
                    {
                        await Sender(ContentToMessage(_text.GetText(NemeText.SpamSelectLanguage, Language.Russian)));
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
                            await Sender(ContentToMessage(_text.GetText(NemeText.SucessIdTeam, Language)));
                            //Переводим в режим роутинга сообщений на API
                            State = UserState.RouteMessage;
                        }
                        else
                        {
                            //Отправляем ошибку
                            await Sender(ContentToMessage(_text.GetText(NemeText.ErrorIdTeam, Language)));
                            //Увеличиваем счётчик ошибок ввода id
                            CountErrorSelectTeamId++;
                        }

                        //Проверка на Spam
                        if (CountErrorSelectTeamId > 5)
                        {
                            //Отправляем сообщение с баном по вводу id
                            await Sender(ContentToMessage(_text.GetText(NemeText.SpamIdTeam, Language)));
                            State = UserState.SpamGetIdTeam;
                        }

                    }
                    break;
                case UserState.SpamGetIdTeam: //Спам по вводу id
                    {
                        await Sender(ContentToMessage(_text.GetText(NemeText.SpamIdTeam, Language)));
                    }
                    break;
                case UserState.RouteMessage://Роутинг сообщений в API
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Конвертер Content to Message
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private Message ContentToMessage(Content content)
        {
            var message = new Message();
            message.Id = Guid.NewGuid().ToString();
            message.IdUser = Id;
            message.Date = DateTime.Now;
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
        RouteMessage

    }


}
