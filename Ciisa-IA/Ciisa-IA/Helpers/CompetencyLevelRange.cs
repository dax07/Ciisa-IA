namespace Ciisa_IA.Helpers
{
    public class CompetencyLevelRange
    {
        public string JobLevel { get; set; } = null!;

        public int TotalRequirements { get; set; }

        public List<LevelRequirement> Requirements { get; set; } = null!;
    }

    public class LevelRequirement
    {
        public double Score { get; set; }
        public int ExpectedCount { get; set; }
    }
}
