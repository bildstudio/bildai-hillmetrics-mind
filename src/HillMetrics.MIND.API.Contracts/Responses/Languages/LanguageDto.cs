namespace HillMetrics.MIND.API.Contracts.Responses.Languages
{
    public class LanguageDto
    {
        public LanguageDto(int id, string name, string twoLetterCode, bool isActive)
        {
            Id = id;
            Name = name;
            TwoLetterCode = twoLetterCode;
            IsActive = isActive;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TwoLetterCode { get; set; }
        public bool IsActive { get; set; }
    }
}
