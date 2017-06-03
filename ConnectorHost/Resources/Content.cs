using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectorHost.Resources
{
    /// <summary>
    /// Объект контент
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Вложения
        /// </summary>
        public List<Message.Attachment> Attachments { get; set; }
    }
}
