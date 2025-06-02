namespace HillMetrics.MIND.API.Contracts.Responses.AiDataset.Metadatas
{
    public class GetMetadataResponse : ApiResponseBase<FinancialDataPointElementMetadataDto>
    {
        public GetMetadataResponse(FinancialDataPointElementMetadataDto data) : base(data)
        {
        }
    }
}
