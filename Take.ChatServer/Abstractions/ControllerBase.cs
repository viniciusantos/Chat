using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Take.ChatServer.Abstractions
{
    public abstract class ControllerBaseChatServer : ControllerBase
    {
        public ObjectResult BadBusinessLogicRequest(object error)
        {
            return this.StatusCode(409, error);
        }
    }
}
