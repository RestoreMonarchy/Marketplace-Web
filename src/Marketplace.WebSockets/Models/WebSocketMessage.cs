using Newtonsoft.Json;
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
        public string[] Arguments { get; set; }
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

        public static WebSocketMessage FromJson(byte[] buffer, int count)
        {
            var text = Encoding.ASCII.GetString(buffer, 0, count);
            return FromJson(text);
        }
    }
}
