﻿using System;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Mwp.ServiceBus
{
    public class ServiceBusSerializer : ISingletonDependency
    {
        private readonly IJsonSerializer _jsonSerializer;

        public ServiceBusSerializer(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(obj));
        }

        public object Deserialize(byte[] value, Type type)
        {
            return _jsonSerializer.Deserialize(type, Encoding.UTF8.GetString(value));
        }
    }
}