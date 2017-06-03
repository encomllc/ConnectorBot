using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectorHost
{
   /// <summary>
   /// Объект сообщение
   /// </summary>
    public class Message
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string IdUser { get; set; }
        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Вложения
        /// </summary>
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// Объект вложения
        /// </summary>
        public class Attachment
        {
            /// <summary>
            /// Типа
            /// </summary>
            public string ContentType { get; set; }
            /// <summary>
            /// Адрес
            /// </summary>
            public string ContentUrl { get; set; }
            /// <summary>
            /// Название
            /// </summary>
            public string Name { get; set; }
        }
    }
}
