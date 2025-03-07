using AutoMapper;
using HillMetrics.MIND.API.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using HillMetrics.Normalized.Domain.Contracts.AI;
using HillMetrics.MIND.API.Contracts.Responses.Llm;
using HillMetrics.MIND.API.Tests.Fakers;
using Microsoft.AspNetCore.Http;
using HillMetrics.Normalized.Domain.Contracts.AI.Models;

namespace HillMetrics.MIND.API.Tests.Mappers
{
    [TestFixture]
    public class LlmMappingProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<LlmMappingProfile>());
            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }

        [Test]
        public void Automapper_AiLlmEntity_AiLlmEntityDto()
        {
            var aiLlmEntityFaker = new AiLlmEntityFaker(2);
            var domainEntity = aiLlmEntityFaker.Generate();

            AiLlmEntityDto dto = _mapper.Map<AiLlmEntityDto>(domainEntity);

            Assert.That(dto, Is.Not.Null);
            Assert.That(dto.History, Is.Not.Null);
            Assert.Multiple(() => {
                Assert.That(domainEntity.Id, Is.EqualTo(dto.Id));
                Assert.That(domainEntity.DocumentationUrl, Is.EqualTo(dto.DocumentationUrl));
                Assert.That(domainEntity.LogoUrl, Is.EqualTo(dto.LogoUrl));
                Assert.That(domainEntity.Provider, Is.EqualTo(dto.Provider));
                Assert.That(domainEntity.Name, Is.EqualTo(dto.Name));

                Assert.That(domainEntity.History.Count, Is.EqualTo(dto.History.Count));
                Assert.That(domainEntity.History[0].Id, Is.EqualTo(dto.History[0].Id));
                Assert.That(domainEntity.History[0].Context, Is.EqualTo(dto.History[0].Context));
                Assert.That(domainEntity.History[0].Prompt, Is.EqualTo(dto.History[0].Prompt));
                Assert.That(domainEntity.History[0].Response, Is.EqualTo(dto.History[0].Response));
            });
        }

        [Test]
        public void Automapper_AiModelPrompt_AiModelPromptDto()
        {
            var faker = new AiModelPromptFaker();
            var domainEntity = faker.Generate();

            AiModelPromptDto dto = _mapper.Map<AiModelPromptDto>(domainEntity);
            Assert.That(dto, Is.Not.Null);
            Assert.Multiple(() => {
                Assert.That(domainEntity.Id, Is.EqualTo(dto.Id));
                Assert.That(domainEntity.Name, Is.EqualTo(dto.Name));
                Assert.That(domainEntity.ProductType, Is.EqualTo(dto.ProductType));
                Assert.That(domainEntity.DataType, Is.EqualTo(dto.DataType));
                //Assert.That(domainEntity.Prompt, Is.EqualTo(dto.Prompt));
            });
        }

        //CreateMap<CreatePromptRequest, SaveAiModelPromptModel>(MemberList.Source);
        [Test]
        public void ManualMappings_CreatePromptRequest_SaveAiModelPromptModel()
        {
            var faker = new CreatePromptRequestFaker();
            var request = faker.Generate();
            var mockedFormFile = MockFormFile();
            request.File = mockedFormFile.FormFile;

            SaveAiModelPromptModel model = request.ToSaveAiModelPromptModel();

            Assert.That(model, Is.Not.Null);
            Assert.Multiple(() => {
                Assert.That(model.Id, Is.Null);
                Assert.That(model.Name, Is.EqualTo(request.Name));
                Assert.That(model.PromptFile, Is.Not.Null);
                Assert.That(model.PromptFile!.FileName, Is.EqualTo(mockedFormFile.FileName));
                Assert.That(model.ProductType, Is.EqualTo(request.ProductType));
                Assert.That(model.DataType, Is.EqualTo(request.DataType));
            });
        }

        //CreateMap<UpdatePromptRequest, SaveAiModelPromptModel>(MemberList.Source);

        [Test]
        public void ManualMappings_UpdatePromptRequest_NoFile_SaveAiModelPromptModel()
        {
            int promptId = 10;
            var faker = new UpdatePromptRequestFaker();
            var request = faker.Generate();

            SaveAiModelPromptModel model = request.ToSaveAiModelPromptModel(promptId: promptId);

            Assert.That(model, Is.Not.Null);
            Assert.Multiple(() => {
                Assert.That(model.Id, Is.EqualTo(promptId));
                Assert.That(model.Name, Is.EqualTo(request.Name));
                Assert.That(model.PromptFile, Is.Null);
                Assert.That(model.ProductType, Is.EqualTo(request.ProductType));
                Assert.That(model.DataType, Is.EqualTo(request.DataType));
            });
        }

        [Test]
        public void ManualMappings_UpdatePromptRequest_FileUpdated_SaveAiModelPromptModel()
        {

            int promptId = 10;
            var faker = new UpdatePromptRequestFaker();
            var request = faker.Generate();
            var mockedFormFile = MockFormFile();
            request.File = mockedFormFile.FormFile;

            SaveAiModelPromptModel model = request.ToSaveAiModelPromptModel(promptId: promptId);

            Assert.That(model, Is.Not.Null);
            Assert.Multiple(() => {
                Assert.That(model.Id, Is.EqualTo(promptId));
                Assert.That(model.Name, Is.EqualTo(request.Name));
                Assert.That(model.PromptFile, Is.Not.Null);
                Assert.That(model.PromptFile.FileName, Is.EqualTo(mockedFormFile.FileName));
                Assert.That(model.ProductType, Is.EqualTo(request.ProductType));
                Assert.That(model.DataType, Is.EqualTo(request.DataType));
            });
        }

        [Test]
        public void Automapper_UpdatePromptRequest_NoFile_SaveAiModelPromptModel()
        {
            var faker = new UpdatePromptRequestFaker();
            var request = faker.Generate();

            SaveAiModelPromptModel model = request.ToSaveAiModelPromptModel(promptId: 10);

            Assert.That(model, Is.Not.Null);
            Assert.Multiple(() => {
                Assert.That(model.Id, Is.EqualTo(10));
                Assert.That(model.Name, Is.EqualTo(request.Name));
                Assert.That(model.PromptFile, Is.Null);
                Assert.That(model.ProductType, Is.EqualTo(request.ProductType));
                Assert.That(model.DataType, Is.EqualTo(request.DataType));
            });
        }

        [Test]
        public void Automapper_PromptSearchRequest_AiModelPromptSearch()
        {
            var faker = new PromptSearchRequestFaker();
            var request = faker.Generate();

            AiModelPromptSearch model = _mapper.Map<AiModelPromptSearch>(request);
            Assert.That(model, Is.Not.Null);
            Assert.Multiple(() => {
                //Assert.That(model.Content, Is.EqualTo(request.Content));
                Assert.That(model.Name, Is.EqualTo(request.Name));
                Assert.That(model.DataType, Is.EqualTo(request.DataType));
                Assert.That(model.ProductType, Is.EqualTo(request.ProductType));

                Assert.That(model.Pagination, Is.Not.Null);
                Assert.That(model.Pagination.PageNumber, Is.EqualTo(request.Pagination.PageNumber));
                Assert.That(model.Pagination.PageSize, Is.EqualTo(request.Pagination.PageSize));

                Assert.That(model.Sorting, Is.Not.Null);
                Assert.That(model.Sorting, Has.Count.EqualTo(1));

                Assert.That(model.Sorting[0].Field, Is.EqualTo(request.Sorting.Field));
                Assert.That(model.Sorting[0].Direction.ToString(), Is.EqualTo(request.Sorting.Direction.ToString()));
            });
        }


        private FormFileWrapper MockFormFile(string fileName = "test.txt")
        {
            var bytes = File.ReadAllBytes($"Files/{fileName}");
            var memoryStream = new MemoryStream(bytes);
            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "test", "test.txt");

            return new FormFileWrapper(formFile, fileName);
        }
    }

    internal class FormFileWrapper
    {
        public FormFileWrapper(IFormFile formFile, string fileName)
        {
            FormFile = formFile;
            FileName = fileName;
        }

        public IFormFile FormFile { get; set; }
        public string FileName { get; set; }
    }
}
