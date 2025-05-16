namespace HillMetrics.MIND.Infrastructure.Contracts.Database.Entity.Common
{
    public abstract class TimeableEntity
    {
        public TimeableEntity()
        {
            DtInsert = default;
            DtUpdate = default;
        }

        public TimeableEntity(DateTime dtInsert, DateTime dtUpdate)
        {
            DtInsert = dtInsert;
            DtUpdate = dtUpdate;
        }

        public DateTime DtInsert { get; set; }
        public DateTime DtUpdate { get; set; }
    }
}
