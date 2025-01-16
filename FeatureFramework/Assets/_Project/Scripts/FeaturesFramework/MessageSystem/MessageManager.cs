// (c) 2020 Oleg Fischer

using System;
using System.Collections.Generic;

namespace MessageSystem
{
	public static class MessageManager
	{
		private static Dictionary<Type, HashSet<IMessageReceiver>> _messages;
		private static Dictionary<Type, Queue<Message>> _messagePool;
		private static Queue<Message> _messagesQueue = new();
		private static Queue<Type> _messagesToRemove = new();
		private static Queue<IMessageReceiver> _receiversToRemove = new();
		private static Queue<IMessageReceiver> _receiversToCleanup = new ();

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
		}

		/// <summary>
		/// Unsubscribe the given message.
		/// </summary>
		/// <param name="receiver">Object which should stop receiving message.</param>
		/// <typeparam name="T">Message type, which object should stop receiving.</typeparam>
		public static void StopReceivingMessage<T>(IMessageReceiver receiver) where T : Message
		{
			var messageType = typeof(T);
			if (_messages.ContainsKey(messageType))
			{
				_messagesToRemove.Enqueue(messageType);
				_receiversToRemove.Enqueue( receiver);

				if (_receiversToRemove.Count == 1 && _messagesQueue.Count == 0 && _receiversToCleanup.Count == 0)
				{
					HandleMessages();
				}
			}
			
		}

		/// <summary>
		/// Unsubscribe to all messages.
		/// </summary>
		/// <param name="receiver">Object which should stop receiving all messages.</param>
		public static void StopReceivingAllMessages(IMessageReceiver receiver)
		{
			_receiversToCleanup.Enqueue(receiver);

			if (_receiversToCleanup.Count == 1 && _messagesQueue.Count == 0 && _receiversToRemove.Count == 0)
			{
				HandleMessages();
			}
		}

		/// <summary>
		/// Send message to all receivers.
		/// </summary>
		/// <param name="message">Message object.</param>
		public static void SendMessage(Message message)
		{
			if (_messages == null || !_messages.ContainsKey(message.GetType())) return;
			
			message.Init(_messages[message.GetType()].Count);
			
			_messagesQueue.Enqueue(message);

			if (_messagesQueue.Count == 1 && _receiversToCleanup.Count == 0 && _receiversToRemove.Count == 0)
			{
				HandleMessages();
			}
		}

		/// <summary>
		/// Handle all message related activities such as sending messages,
		/// and removing listeners in the same function to prevent data being
		/// accessed at the same time
		/// </summary>
		private static void HandleMessages()
		{
			// Handle sending messages
			while (_messagesQueue.Count > 0)
			{
				var message = _messagesQueue.Peek();
				foreach (var receiver in _messages[message.GetType()])
				{
					receiver.MessageReceived(message);
				}

				_messagesQueue.Dequeue();
			}

			// Handle removing single receivers for a message
			while (_receiversToRemove.Count > 0)
			{
				var receiver = _receiversToRemove.Peek();
				var messageType = _messagesToRemove.Dequeue();
				_messages[messageType].Remove(receiver);
				_receiversToRemove.Dequeue();
			}

			// Handling removing a receiver for all messages 
			while (_receiversToCleanup.Count > 0)
			{
				var receiver = _receiversToCleanup.Peek();
				foreach (var messageList in _messages.Values)
				{
					if (messageList.Contains(receiver))
					{
						messageList.Remove(receiver);
					}
				}

				_receiversToCleanup.Dequeue();
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