using Elara.Domain.Common;
using Elara.Domain.Enums;

namespace Elara.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
