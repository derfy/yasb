using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.Msmq.Messaging
{
    public class MsmqEndPoint :IEndPoint
    {
        private string _value;
        public MsmqEndPoint()
        {

        }
        public MsmqEndPoint(string queueName)
        {
            _value = ParseEndPoint(queueName);
        }
        public string Value
        {
            get { return _value; }
        }

        public string Name { get; set; }

        private static string ParseEndPoint(string value)
        {
            var machine = ".";
            var queue = value;
            if (value.Contains("@"))
            {
                machine = value.Split('@')[1];
                queue = value.Split('@')[0];
            }
            if (machine == "localhost") machine = ".";
            return machine + "\\private$\\" + queue;
        }
    }
}
