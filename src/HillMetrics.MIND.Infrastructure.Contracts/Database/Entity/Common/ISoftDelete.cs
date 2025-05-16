namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common
{
    public interface ISoftDelete
    {
        /// <summary>
        /// Indicates if the entity is deleted
        /// </summary>
        bool IsDeleted { get; set; }
    }

    public interface IAuditable
    {
        string? AuditReference { get; set; }
    }

    public static class AuditableExtensions
    {
        public static string GetAuditReference(this object instance, object[] keys)
        {
            if (instance == null) return string.Empty;

            if (instance is IAuditable auditable)
                return auditable.AuditReference!;

            return string.Join('_', keys);
        }
    }
}
