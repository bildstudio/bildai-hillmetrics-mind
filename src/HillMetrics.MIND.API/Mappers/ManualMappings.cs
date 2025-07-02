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

            model.PromptFile = request.File.ToFileStreamModel();


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

            model.PromptFile = request.File.ToFileStreamModel();


            return model;
        }

        public static ExtractFinancialDataLlmModel ToExtractFinancialDataLlmModel(this ExtractDataLlmRequest request)
        {

            ExtractFinancialDataLlmModel model = new ExtractFinancialDataLlmModel
            {
                AiModelsIds = request.AiModelsIds,
                PromptId = request.PromptId
            };

            model.PromptFile = request.File.ToFileStreamModel();

            return model;
        }

        public static SaveAiLlmModel ToSaveAiLlmModel(this CreateLlmRequest request)
        {
            return new SaveAiLlmModel
            {
                HostProvider = request.HostProvider,
                Name = request.Name,
                Provider = request.Provider,
                DocumentationUrl = request.DocumentationUrl,
                LogoUrl = request.LogoUrl,
                ApiKey = request.ApiKey,
                BaseUrl = request.BaseUrl
            };
        }

        public static SaveAiLlmModel ToSaveAiLlmModel(this UpdateLlmRequest request, int id)
        {
            return new SaveAiLlmModel
            {
                Id = id,
                HostProvider = request.HostProvider,
                Name = request.Name,
                Provider = request.Provider,
                DocumentationUrl = request.DocumentationUrl,
                LogoUrl = request.LogoUrl,
                ApiKey = request.ApiKey,
                BaseUrl = request.BaseUrl
            };
        }

        private static FileStreamModel? ToFileStreamModel(this IFormFile? formFile)
        {
            if (formFile == null)
                return null;

            var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            memoryStream.Position = 0;

            FileStreamModel model = new FileStreamModel(memoryStream, formFile.FileName);

            return model;
        }
    }
}