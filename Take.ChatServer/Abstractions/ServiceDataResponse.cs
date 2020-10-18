using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take.ChatServer.Abstractions
{
    public class ServiceDataResponse
    {
        public ServiceDataResponse(string errorMessage)
        {
            this.ErrorMessage.Add(errorMessage);
        }

        public ServiceDataResponse()
        {

        }

        public bool Success { get { return this.ErrorMessage.Count == 0; } }
        public IList<string> ErrorMessage { get; set; } = new List<string>();
    }
}
