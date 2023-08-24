using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoomWebApp.AutoGen;

public partial class BookedRoom
{
    [Key]
    public long BookedRoomId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly Date { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeOnly StartTime { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeOnly EndTime { get; set; }

    public string? Description { get; set; }

    public long RoomId { get; set; }

    public long UserId { get; set; }

    [ForeignKey("RoomId")]
    [InverseProperty("BookedRooms")]
    public virtual Room Rooms { get; set; } = new Room();

    [ForeignKey("UserId")]
    [InverseProperty("BookedRooms")]
    public virtual User Users { get; set; } = new User();
}