// (c) 2020 Oleg Fischer

using System;
using System.Collections.Generic;

namespace MessageSystem
{
	public static class MessageManager
	{
		private static Dictionary<Type, HashSet<IMessageReceiver>> _messages;
		private static HashSet<IMessageReceiver> _disabledReceivers;

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
		

		/// <summary>
		/// Gen number of listeners for given message.
		/// </summary>
		/// <typeparam name="T">Message Type.</typeparam>
		/// <returns>Number of listeners.</returns>
		public static int NumberOfListeners<T>() where T : Message 
			=> _messages.ContainsKey(typeof(T)) ? _messages[typeof(T)].Count : -1;

	}

}