using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarAPI
{
    public class OptionRoom
    {
        public OptionRoom(string roomName, DateTime startDate, DateTime endDate)
        {
            RoomName = roomName;
            StartDate = startDate;
            EndDate = endDate;
        }

        public string? RoomName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}