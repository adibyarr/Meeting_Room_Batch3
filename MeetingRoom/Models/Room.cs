using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomWebApp.AutoGen;

public partial class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RoomId { get; set; }

	[Required]
	public string RoomName { get; set; } = null!;

	public long? Capacity { get; set; }

    public string? Description { get; set; }
    
    [InverseProperty("Rooms")]
    public virtual ICollection<BookedRoom> BookedRooms { get; set;} = new List<BookedRoom>();
}
