// (c) 2020 Oleg Fischer

using System;
using System.Collections.Generic;

namespace MessageSystem
{
	/// <summary>
	/// Class which generates and recycles message objects.
	/// </summary>
	public static class MessageProvider
	{
		private static Dictionary<Type, Queue<Message>> _messagePool;

		/// <summary>
		/// Generate a new message object or get one from pool.
		/// </summary>
		/// <typeparam name="T">Message type.</typeparam>
		/// <returns>Message type.</returns>
		public static T GetMessage<T>() where T : Message, new()
		{
			var messageType = typeof(T);
			if (_messagePool == null) _messagePool = new Dictionary<Type, Queue<Message>>();

			if (!_messagePool.ContainsKey(messageType) || _messagePool[messageType].Count == 0) return new T();

			return _messagePool[messageType].Dequeue() as T;
		}

		/// <summary>
		/// Recycle message by putting it to a pool.
		/// </summary>
		/// <param name="message">Message object.</param>
		public static void RecycleMessage(Message message)
		{
			var messageType = message.GetType();

			if (!_messagePool.ContainsKey(messageType)) _messagePool.Add(messageType, new Queue<Message>());

			_messagePool[messageType].Enqueue(message);
		}
	
}

}