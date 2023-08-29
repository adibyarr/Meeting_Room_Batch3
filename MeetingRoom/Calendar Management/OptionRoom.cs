public class OptionRoom
{
    public string? RoomName { get; set; }
    public long? Capacity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public OptionRoom(string? roomName, long? capacity, DateTime? startDate, DateTime? endDate)
    {
        RoomName = roomName;
        Capacity = capacity;
        StartDate = startDate;
        EndDate = endDate;
    }
}