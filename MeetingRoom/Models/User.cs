using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomWebApp.AutoGen;

public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long UserId { get; set; }

    [DataType(DataType.Text)]
    public string Username { get; set; } = null!;

    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;

    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public long? RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Roles { get; set; } = new Role();

    [InverseProperty("Users")]
    public virtual ICollection<BookedRoom> BookedRooms { get; set; } = new List<BookedRoom>();
}
