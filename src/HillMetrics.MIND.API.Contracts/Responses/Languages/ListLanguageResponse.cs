namespace HillMetrics.MIND.API.Contracts.Responses.Languages
{
    public class ListLanguageResponse : ApiPagedResponseBase<LanguageDto>
    {
        public ListLanguageResponse(IEnumerable<LanguageDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
