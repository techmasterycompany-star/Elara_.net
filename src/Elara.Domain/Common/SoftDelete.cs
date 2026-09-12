namespace Elara.Domain.Common
{
    public abstract class SoftDelete : BaseEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
