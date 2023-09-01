using System.Security.Claims;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace CalendarAPI;

public static class CalendarManager
{
	public static CalendarService GenerateService(UserCredential credential)
	{
		var service = new CalendarService(new BaseClientService.Initializer()
		{
			HttpClientInitializer = credential
		});

		return service;
	}

	public static Calendar GenerateCalendar(CalendarService service, string? calId) // calendar's Id 
	{
		var calendar = service.Calendars.Get(calId).Execute();
		return calendar;
	}

	public static Calendar CreateCalendar(CalendarService service, Calendar calendar)
	{
		return service.Calendars.Insert(calendar).Execute();
	}

	public static string UpdateCalendar(CalendarService service, Calendar calendar, string calId) {
		Calendar updatedCalendar = service.Calendars.Update(calendar, calId).Execute();
		return updatedCalendar.ETag;
	}

	public static void DeleteCalendar(CalendarService service, string calId)
	{
		service.Calendars.Delete(calId).Execute();
	}

	public static Events MakeRequest(
		CalendarService service,
		Calendar calendar,
		DateTime timeMin,
		DateTime timeMax
	)
	{
		EventsResource.ListRequest listRequest = service.Events.List(calendar.Id);
		listRequest.TimeMin = timeMin;
		listRequest.TimeMax = timeMax;
		listRequest.TimeZone = "Asia/Jakarta";
		listRequest.ShowDeleted = false;
		listRequest.SingleEvents = true;
		// listRequest.MaxResults = 10;
		listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

		Events events = listRequest.Execute();

		return events;
	}

	public static List<Event> GetEventList(Events events)
	{
		return events.Items.ToList();
	}

	[Obsolete]
	public static bool ListingEvents(List<Event> events)
	{
		if (events != null && events.Count > 0)
		{
			foreach (var singleEvent in events)
			{
				Console.WriteLine("------------------------------");
				Console.WriteLine($"Event Id 			: {singleEvent.Id}");
				Console.WriteLine($"Event Summary 		: {singleEvent.Summary}");
				Console.WriteLine($"Event Location 		: {singleEvent.Location}");
				Console.WriteLine($"Event Description 	: {singleEvent.Description}");
				Console.WriteLine($"Event Start/TimeZone: {singleEvent.Start.DateTime.ToString()}, {singleEvent.Start.TimeZone}");
				Console.WriteLine($"Event End/Timezone	: {singleEvent.End.DateTime.ToString()}, {singleEvent.End.TimeZone}");
				Console.WriteLine($"Event Attendees		: {singleEvent.Attendees}");
			}
		}
		else
		{
			Console.WriteLine("No upcoming events found.");
		}
		return true;
	}

	public static bool CreateEvent(
		CalendarService service,
		Calendar calendar,
		string summary,
		string description,
		EventDateTime start,
		EventDateTime end,
		IList<EventAttendee> attendees
	)
	{
		Event eventInsert = new Event()
		{
			Summary = summary,
			Description = description,
			Start = start,
			End = end,
			Attendees = attendees
		};

		var InsertRequest = service.Events.Insert(eventInsert, calendar.Id);

		try
		{
			InsertRequest.Execute();
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Insert event failed: " + ex.Message);
			try
			{
				service.Events.Update(eventInsert, calendar.Id, eventInsert.Id).Execute();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine("Update event failed: " + e.Message);
				return false;
			}
		}
	}

	// internal static Events MakeRequest(CalendarService service, Calendar calendar, DateTime start, DateTime end)
	// {
	//     throw new NotImplementedException();
	// }

	// internal static Event MakeRequest(CalendarService service, Calendar calendar, string startDate)
	// {
	//     throw new NotImplementedException();
	// }
}