using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agregator.Services
{
    public class SignTaskEvent : ManualResetEventSlim
    {

        public SignTaskEvent() : base()
        {
            ConversationID = Guid.NewGuid();
        }

        public Guid  ConversationID { get; private set; }
        public string EventData { get; private set; }
        public void Set(string data = null)
        {
            EventData = data;
            base.Set();
        }
    }


    public class ComplatedEventArgs
    {
        public Guid CoversationID { private set; get; }
        public string Result { private set; get; }
        public ComplatedEventArgs(string data)
        {
            
            var json = (JToken)JsonConvert.DeserializeObject(data);
            string id = json["id"].ToString();
            CoversationID = Guid.Parse(id);
            Result = json["result"].ToString();
        }
    }

    
    public delegate void ComplatedEvent(object sender, ComplatedEventArgs e);

    public class ComplateTimeoutException : TimeoutException
    {
    }

    public interface ISignService
    {
        void Add(ComplatedEvent action);
        void Remove(ComplatedEvent action);
        Task Complete(string wtaskResult);
    }

    public class SignService : ISignService
    {

        private event ComplatedEvent complete;
        public void Add(ComplatedEvent action) => complete += action;
        public void Remove(ComplatedEvent action) => complete -= action;
        public async Task Complete(string wtaskResult) =>
                
            await Task.Factory.StartNew(() => complete?.Invoke(this, new ComplatedEventArgs(wtaskResult)));
                /*
                await Task.Factory.FromAsync(
                    complete.BeginInvoke(this, new ComplatedEventArgs(wtaskResult), null, this)
                    , complete.EndInvoke
                    );
                */
    }



    }
