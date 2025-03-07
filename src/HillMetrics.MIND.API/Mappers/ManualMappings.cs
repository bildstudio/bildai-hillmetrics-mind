using Ardalis.GuardClauses;
using AutoMapper;
using HillMetrics.MIND.API.Contracts.Requests.Llm;
using HillMetrics.Normalized.Domain.Contracts.AI.Models;
using HillMetrics.Normalized.Domain.Contracts.Files;

namespace HillMetrics.MIND.API.Mappers
{
    public static class ManualMappings
    {
        public static SaveAiModelPromptModel ToSaveAiModelPromptModel(
            this CreatePromptRequest request
            )
        {
            var model = new SaveAiModelPromptModel
            {
                DataType = request.DataType,
                ProductType = request.ProductType,
                Name = request.Name
            };

            var memoryStream = new MemoryStream();
            request.File.CopyTo(memoryStream);
            memoryStream.Position = 0;

            model.PromptFile = new FileStreamModel(memoryStream, request.File.FileName);


            return model;
        }

        public static SaveAiModelPromptModel ToSaveAiModelPromptModel(
            this UpdatePromptRequest request,
            int promptId
            )
        {
            var model = new SaveAiModelPromptModel
            {
                Id = promptId,
                DataType = request.DataType,
                ProductType = request.ProductType,
                Name = request.Name
            };

            if(request.File != null)
            {
                var memoryStream = new MemoryStream();
                request.File.CopyTo(memoryStream);
                memoryStream.Position = 0;

                model.PromptFile = new FileStreamModel(memoryStream, request.File.FileName);
            }


            return model;
        }

        public static ExtractFinancialDataLlmModel ToExtractFinancialDataLlmModel(this ExtractDataLlmRequest request)
        {

            ExtractFinancialDataLlmModel model = new ExtractFinancialDataLlmModel
            {
                AiModelsIds = request.AiModelsIds,
                PromptId = request.PromptId
            };

            return model;
        }
    }
}
