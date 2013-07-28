﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasb.Common.Messaging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Yasb.Common.Serialization.MessageDeserializers
{
    public delegate IMessageDeserializer MessageDeserializerFactory(Type type);
    public interface IMessageDeserializer
    {
        IMessage DeserializeFrom(JToken jToken, JsonSerializer serializer);
    }

    public interface IMessageDeserializer<TMessage> : IMessageDeserializer where TMessage : IMessage
    {

    }
}
