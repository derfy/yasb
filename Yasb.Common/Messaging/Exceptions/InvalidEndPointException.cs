using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasb.Common.Messaging.Exceptions
{
    public class InvalidEndPointException : Exception
    {
        public InvalidEndPointException(string message):base(message)
        {

        }
    }
}
