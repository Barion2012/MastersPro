using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agregator.Models
{
    public enum Sender 
    {
        Me,
        User,
        Manager,
        Robot
    }


    public class ChatMessage
    {
        public Sender who { set; get; }
        public string user { set; get; }
        public string text { set; get; }

        public DateTime time { set; get; }
        public byte[] awatar { set; get; }
    }
}
