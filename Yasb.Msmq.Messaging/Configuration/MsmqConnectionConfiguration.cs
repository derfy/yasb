using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging.Configuration;

namespace Yasb.Msmq.Messaging.Configuration
{
    public class MsmqConnectionConfiguration
    {
        private string _host;
       
        public MsmqConnectionConfiguration(string host, bool isPrivate=true)
        {
            this._host = host;
            IsPrivate = isPrivate;
        }
        public string Host { get { return _host == "localhost" ? "." : _host; } }
        public bool IsPrivate { get; private set; }
    }
}
