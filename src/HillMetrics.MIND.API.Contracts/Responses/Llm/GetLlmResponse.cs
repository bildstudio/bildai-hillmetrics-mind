namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class GetLlmResponse : ApiResponseBase<AiLlmEntityDto>
    {
        public GetLlmResponse(AiLlmEntityDto data) : base(data)
        {

        }
    }
}
