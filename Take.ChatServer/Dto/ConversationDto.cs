using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Take.ChatServer.Dto
{
    public class ConversationDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public bool isPublicMessage { get; set; }
    }
}
