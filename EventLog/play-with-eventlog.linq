<Query Kind="Program" />

void Main()
{
	var toy = new EventLogToy();
	toy.Init();
	toy.WriteEntry("Hello Log!!!", EventLogEntryType.FailureAudit, 22, 1, new byte[1] {0});
}

public class EventLogToy
{
	private static readonly string eventSourseName = "HulioSoftware";
	private static readonly string logName = "Security";

	public EventLogToy Init()
	{
		if (!EventLog.SourceExists(eventSourseName))
		{
			EventLog.CreateEventSource(eventSourseName, logName);
		}
		
		return this;
	}

	public EventLogToy DumpExistingLogs()
	{
		var allLogs = EventLog.GetEventLogs();
		allLogs.Dump();
		return this;
	}

	public EventLogToy KillEventSourse(string name)
	{
		EventLog.DeleteEventSource(name);
		return this;	
	}

	public EventLogToy WriteEntry(string entry, EventLogEntryType entryType, int eventId, short category, byte[] rawData)
	{
		EventLog.WriteEntry(eventSourseName, entry, entryType, eventId, category, rawData);
		return this;
	}
	
	
}
// Define other methods and classes here