// (c) 2022 Oleg Fischer

using Features;
using MessageSystem;
using UnityEngine;

public class TestFeature1 : Feature, IMessageReceiver
{
	public override bool NeedsUpdate() => false;

	public override void Init()
	{
		Debug.Log("Init Feature 1");
		MessageManager.StartReceivingMessage<StartGameMessage>(this);
		base.Init();
	}

	public override void Cleanup()
	{
		MessageManager.StopReceivingAllMessages(this);
		Debug.Log("Dispose Feature 1");
	}

	public void MessageReceived(Message message)
	{
		switch (message)
		{
			case StartGameMessage:
				Debug.Log("Start game");
				MessageProvider.GetMessage<TestMessage>()
					.SetData("This is test message", 12, () =>
					{
						Debug.Log("Called from FUNCTION Callback");
					})
					.Send();
				break;
		}
		
		message.OnDoneUsing();
	}
}