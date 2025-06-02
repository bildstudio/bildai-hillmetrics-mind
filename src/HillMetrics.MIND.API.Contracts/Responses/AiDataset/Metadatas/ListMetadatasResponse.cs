namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset.Metadatas
{
    public class ListMetadatasResponse : ApiPagedResponseBase<FinancialDataPointElementMetadataDto>
    {
        public ListMetadatasResponse(IEnumerable<FinancialDataPointElementMetadataDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
