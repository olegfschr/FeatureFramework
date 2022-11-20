// (c) 2022 Oleg Fischer

using MessageSystem;

public class TestMessage : Message
{
	public string MessageText { get; private set; }
	public int Number { get; private set; }
	public System.Action Function { get; private set; }

	public TestMessage SetData(string messageText, int number, System.Action function)
	{
		MessageText = messageText;
		Number = number;
		Function = function;

		return this;
	}
}