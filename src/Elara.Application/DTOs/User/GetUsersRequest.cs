using Elara.Application.DTOs.Common;
using RoleEnum = Elara.Domain.Enums.Role;

namespace Elara.Application.DTOs.User
{
    public class GetUsersRequest : PaginationRequest
    {
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? EmailConfirmed { get; set; }
        public RoleEnum? Role { get; set; }
        public string? Search { get; set; }
    }

}
