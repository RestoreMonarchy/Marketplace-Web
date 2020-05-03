﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Marketplace.WebSockets.Models
{
    public class WebSocketMessage
    {
        public uint Id { get; set; }
        public string Method { get; set; }
        public bool IsQuestion { get; set; }
        public object[] Arguments { get; set; }
        public uint? QuestionId { get; set; }

        [JsonIgnore]
        public virtual SemaphoreSlim Signal { get; set; }
        [JsonIgnore]
        public virtual WebSocketMessage Response { get; set; }
        [JsonIgnore]
        public virtual WebSocket WebSocket { get; set; }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Respond(WebSocketMessage msg)
        {
            Response = msg;
            Signal.Release();            
        }

        public static WebSocketMessage FromJson(string json)
        {
            return JsonConvert.DeserializeObject<WebSocketMessage>(json);
        }

        public static WebSocketMessage FromJson(byte[] buffer)
        {
            return FromJson(Encoding.ASCII.GetString(buffer, 0, buffer.Length));
        }
    }
}
