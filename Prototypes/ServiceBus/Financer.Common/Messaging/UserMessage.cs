namespace Financer.Common.Messaging
{
    public class UserMessage
    {
        public string MessageText { get; set; }

        /// <summary>
        /// Contains data which determines where a message came from 
        /// and how to send it back toa recipient. 
        /// It's a telegram Chat most likely.
        /// </summary>
        public object SourceContext { get; set; }
    }
}
