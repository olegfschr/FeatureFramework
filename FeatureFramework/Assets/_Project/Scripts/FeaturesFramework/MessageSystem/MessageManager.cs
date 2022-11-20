// (c) 2020 Oleg Fischer

using System;
using System.Collections.Generic;

namespace MessageSystem
{
	public static class MessageManager
	{
		private static Dictionary<Type, HashSet<IMessageReceiver>> _messages;
		private static HashSet<IMessageReceiver> _disabledReceivers;
		private static Dictionary<Type, Queue<Message>> _messagePool;

		/// <summary>
		/// Subscribe object to the given message.
		/// </summary>
		/// <param name="receiver">Object which should receive the message.</param>
		/// <typeparam name="T">Message type which the object should receive.</typeparam>
		public static void StartReceivingMessage<T>(IMessageReceiver receiver) where T : Message
		{
			_messages ??= new Dictionary<Type, HashSet<IMessageReceiver>>();

			var messageType = typeof(T);

			if (!_messages.ContainsKey(messageType)) _messages.Add(messageType, new HashSet<IMessageReceiver>());

			if (!_messages[messageType].Contains(receiver))
			{
				_messages[messageType].Add(receiver);

			}
			else if (_disabledReceivers != null && _disabledReceivers.Contains(receiver)) _disabledReceivers.Remove(receiver);
		}

		/// <summary>
		/// Unsubscribe the given message.
		/// </summary>
		/// <param name="receiver">Object which should stop receiving message.</param>
		/// <typeparam name="T">Message type, which object should stop receiving.</typeparam>
		public static void StopReceivingMessage<T>(IMessageReceiver receiver) where T : Message
		{
			_disabledReceivers ??= new HashSet<IMessageReceiver>();
			_disabledReceivers.Add(receiver);
		}

		/// <summary>
		/// Unsubscribe to all messages.
		/// </summary>
		/// <param name="receiver">Object which should stop receiving all messages.</param>
		public static void StopReceivingAllMessages(IMessageReceiver receiver)
		{
			foreach (var messageList in _messages.Values)
				if (messageList.Contains(receiver))
					messageList.Remove(receiver);
		}

		/// <summary>
		/// Send message to all receivers.
		/// </summary>
		/// <param name="message">Message object.</param>
		public static void SendMessage(Message message)
		{
			if (_messages == null || !_messages.ContainsKey(message.GetType())) return;

			message.Init(_messages[message.GetType()].Count);
			foreach (var receiver in _messages[message.GetType()])
			{
				if (_disabledReceivers != null && _disabledReceivers.Contains(receiver)) continue;
				receiver.MessageReceived(message);
			}
		}

		#region Message Provider

		/// <summary>
		/// Gen number of listeners for given message.
		/// </summary>
		/// <typeparam name="T">Message Type.</typeparam>
		/// <returns>Number of listeners.</returns>
		public static int NumberOfListeners<T>() where T : Message 
			=> _messages.ContainsKey(typeof(T)) ? _messages[typeof(T)].Count : -1;

		/// <summary>
		/// Generate a new message object or get one from pool.
		/// </summary>
		/// <typeparam name="T">Message type.</typeparam>
		/// <returns>Message type.</returns>
		public static T GetMessage<T>() where T : Message, new()
		{
			var messageType = typeof(T);
			_messagePool ??= new Dictionary<Type, Queue<Message>>();

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

		/// <summary>
		/// Clear cached messages for given type. Use if message shouldn't be used anymore.
		/// </summary>
		/// <typeparam name="T">Type of the message cache to clear.</typeparam>
		public static void ClearMessageCache<T>() where T : Message
		{
			Type messageType = typeof(T);
			if (_messagePool.ContainsKey(messageType))
			{
				_messagePool[messageType].Clear();
			}
		}

		#endregion


	}

}