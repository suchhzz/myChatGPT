namespace mychatgpt.Models
{
    public class PostData
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
        public float temperature { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
