// (c) 2020 Oleg Fischer

namespace MessageSystem
{
	/// <summary>
	/// Parent class for messages.
	/// </summary>
	public abstract class Message
	{
		private int _referenceCount;

		/// <summary>
		/// Initializes message.
		/// </summary>
		/// <param name="numberOfReceivers">Number of subscribers to this message for reference counting.</param>
		public void Init(int numberOfReceivers)
		{
			_referenceCount = numberOfReceivers;
		}

		/// <summary>
		/// Called when subscriber is done processing message. When all subscribers are done, message will be recycled.
		/// </summary>
		public void OnDoneUsing()
		{
			_referenceCount--;

			if (_referenceCount == 0) MessageManager.RecycleMessage(this);
		}

		/// <summary>
		/// Broadcasts this message.
		/// </summary>
		public void Send()
		{
			MessageManager.SendMessage(this);
		}
	
	}

}