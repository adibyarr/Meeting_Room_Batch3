public class OptionRoom
{
    public string? RoomName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long? Capacity { get; set; }

    public OptionRoom(string? roomName, DateTime? startDate, DateTime? endDate, long? capacity)
    {
        RoomName = roomName;
        StartDate = startDate;
        EndDate = endDate;
        Capacity = capacity;
    }
}