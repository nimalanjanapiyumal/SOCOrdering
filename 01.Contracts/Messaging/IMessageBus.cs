using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _01.Contracts.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync(string topic, object payload);
    }
}