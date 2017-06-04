using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectorHost.Resources
{
    /// <summary>
    /// Обработчик текстов
    /// </summary>
    public class Text
    {
        public Text()
        {
            foreach (NameText name in Enum.GetValues(typeof(NameText)))
            {
                RussianTextDictionary.Add(name, new Content() { Text = "**Text Ru**: " + name.ToString() });
                EnglishTextDictionary.Add(name, new Content() { Text = "**Text En**: " + name.ToString() });
            }
        }
        /// <summary>
        /// Список для хранения Русский текстов
        /// </summary>
        public Dictionary<NameText, Content> RussianTextDictionary { get; set; } = new Dictionary<NameText, Content>();
        /// <summary>
        /// Список для хранения английских текстов
        /// </summary>
        public Dictionary<NameText, Content> EnglishTextDictionary { get; set; } = new Dictionary<NameText, Content>();

        /// <summary>
        /// Получить текст
        /// </summary>
        /// <param name="nameText">Название текста</param>
        /// <param name="language">Язык</param>
        /// <returns></returns>
        public Content GetText(NameText nameText, Language language)
        {
            switch (language)
            {
                case Language.Russian:
                {
                    if (RussianTextDictionary.ContainsKey(nameText))
                        return RussianTextDictionary[nameText];
                }
                    break;
                case Language.English:
                {
                    if (EnglishTextDictionary.ContainsKey(nameText))
                        return EnglishTextDictionary[nameText];
                }
                    break;
            }
            //На случай отсутвия текста
            var defaultContent = new Content(){Text = "Not Text Dictionary"};
            return defaultContent;
        }
    }

    /// <summary>
    /// Названия текстов
    /// </summary>
    public enum NameText
    {
        /// <summary>
        /// Приветствие
        /// </summary>
        Hello,
        /// <summary>
        /// Просьба выбрать язык
        /// </summary>
        Language,
        /// <summary>
        /// Успешный выбор языка
        /// </summary>
        SucessSelectLanguage,
        /// <summary>
        /// Ошибочный выбор языка
        /// </summary>
        ErrorSelectLanguage,
        /// <summary>
        /// Spam выбор языка
        /// </summary>
        SpamSelectLanguage,
        /// <summary>
        /// Просьба прислать идентификатор комнаты
        /// </summary>
        GetIdTeam,
        /// <summary>
        /// Ошибка во время проверки идентификатора комнаты
        /// </summary>
        ErrorIdTeam,
        /// <summary>
        /// Спам во время проверки идентификатора комнаты
        /// </summary>
        SpamIdTeam,
        /// <summary>
        /// Успешная проверка id комнаты
        /// </summary>
        SucessIdTeam,
        /// <summary>
        /// Старт маршрутизации сообщений на API
        /// </summary>
        StartRoutingMessage,
        /// <summary>
        /// Высокая скорость отправки сообщений
        /// </summary>
        SpeedMessaging,
        /// <summary>
        /// Спам по скорости отправки сообщений
        /// </summary>
        SpamSpeedMessaging,
        /// <summary>
        /// Закрытие сессии
        /// </summary>
        ExitSession,
        /// <summary>
        /// Закрытие сессии по таймеру
        /// </summary>
        ExitTimeSession
    }
}
