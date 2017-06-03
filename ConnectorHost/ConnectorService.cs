using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectorHost
{
    /// <summary>
    /// Connector Service
    /// </summary>
    public class ConnectorService
    {
        public delegate void SenderMessage(object message);
        /// <summary>
        /// Коллекция для хранения позьзователей
        /// </summary>
        public Dictionary<string,User> UsersDictionary { get; set; } = new Dictionary<string, User>();
    }
}
