// (c) 2022 Oleg Fischer

using Features;
using MessageSystem;
using UnityEngine;

public class TestFeature2 : Feature, IMessageReceiver
{
	public override bool NeedsUpdate() => false;
	
	public override void Init()
	{
		Debug.Log("Init Feature 2");
		MessageManager.StartReceivingMessage<TestMessage>(this);
		base.Init();
	}

	public override void Cleanup()
	{
		MessageManager.StopReceivingAllMessages(this);
		Debug.Log("Dispose Feature 2");
	}

	public void MessageReceived(Message message)
	{
		switch (message)
		{
			case TestMessage testMessage:
				Debug.Log($"Received message with text {testMessage.MessageText} and number {testMessage.Number}");
				testMessage.Function();
				break;
		}
		
		message.OnDoneUsing();
	}
	
}