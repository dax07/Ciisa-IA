namespace Ciisa_IA.Dtos
{
    public class CompetencyProfileDTO
    {
        public required string JobTitle { get; set; }
        public required string JobLevel { get; set; } 
        public required List<CompetencyDetailDTO> Competencies { get; set; }
    }

    public class CompetencyDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Level { get; set; }
    }
}
