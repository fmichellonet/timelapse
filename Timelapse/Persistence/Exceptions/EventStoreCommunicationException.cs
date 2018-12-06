namespace Timelapse.Persistence.Exceptions
{
	using System;

	[Serializable]
    public class EventStoreCommunicationException : EventStoreException
    {
        public EventStoreCommunicationException() { }
        public EventStoreCommunicationException(string message) : base(message) { }
        public EventStoreCommunicationException(string message, Exception inner) : base(message, inner) { }
        protected EventStoreCommunicationException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}