// (c) 2022 Oleg Fischer

using System.Threading.Tasks;
using Features;
using MessageSystem;
using UnityEngine;

public class TestFeature2 : Feature, IMessageReceiver
{
	public override bool NeedsUpdate() => false;
	
	public override async void Init()
	{
		MessageManager.StartReceivingMessage<TestMessage>(this);
		await DelayedInit();
	}

	private async Task DelayedInit()
	{
		await Task.Delay(5000);
		Debug.Log("Init Feature 2");
		MessageManager.GetMessage<OnFeatureInitialisedMessage>().Send();
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