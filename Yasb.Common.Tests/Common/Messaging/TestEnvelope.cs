using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;

namespace Yasb.Tests.Common.Messaging
{
    public class TestEnvelope : IEnvelope
    {
        public string Id
        {
            get { throw new NotImplementedException(); }
        }

        public Type ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public IMessage Message
        {
            get { throw new NotImplementedException(); }
        }
    }
}
