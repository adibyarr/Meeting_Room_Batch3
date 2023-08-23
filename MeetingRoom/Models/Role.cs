using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRoomWebApp.AutoGen;

public partial class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RoleId { get; set; }

    public string RoleName { get; set; } = null!;
    
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}