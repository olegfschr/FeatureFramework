// (c) 2022 Oleg Fischer

using MessageSystem;

namespace Features
{
	/// <summary>
	/// Manager class for a single feature of the game.
	/// </summary>
	public abstract class Feature
	{
		/// <summary>
		/// Check if the Feature needs to run the Update function.
		/// </summary>
		/// <returns>True, if needs to run the Update function</returns>
		public abstract bool NeedsUpdate();

		/// <summary>
		/// <para>Used to run necessary initialization processes.</para>
		/// Make sure to send the <see cref="StartGameMessage"/> when feature is ready to be used.
		/// If some async methods are used, do not call <c>base.Init()</c>
		/// </summary>
		public virtual void Init()
		{
			MessageProvider.GetMessage<OnFeatureInitialisedMessage>().Send();
		}
		
		/// <summary>
		/// Normally runs every frame if <see cref="NeedsUpdate"/> returns true.
		/// </summary>
		public virtual void Update(){}

		/// <summary>
		/// Called when feature is disposed.
		/// </summary>
		public virtual void Cleanup() {}
	}
	
}