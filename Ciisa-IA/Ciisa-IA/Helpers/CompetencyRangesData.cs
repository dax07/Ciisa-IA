namespace Ciisa_IA.Helpers
{
    public class CompetencyRangesData
    {
        public static readonly List<CompetencyLevelRange> All = new()
    {
        new()
        {
            JobLevel = "Estratégico",
            TotalRequirements = 12,
            Requirements = new()
            {
                new() { Score = 3.0, ExpectedCount = 2 },
                new() { Score = 3.5, ExpectedCount = 2 },
                new() { Score = 4.0, ExpectedCount = 4 },
                new() { Score = 4.5, ExpectedCount = 2 },
                new() { Score = 5.0, ExpectedCount = 2 }
            }
        },
        new()
        {
            JobLevel = "Funcional",
            TotalRequirements = 12,
            Requirements = new()
            {
                new() { Score = 3.0, ExpectedCount = 2 },
                new() { Score = 3.5, ExpectedCount = 2 },
                new() { Score = 4.0, ExpectedCount = 5 },
                new() { Score = 4.5, ExpectedCount = 2 },
                new() { Score = 5.0, ExpectedCount = 1 }
            }
        },
        new()
        {
            JobLevel = "Coordinador",
            TotalRequirements = 12,
            Requirements = new()
            {
                new() { Score = 3.0, ExpectedCount = 2 },
                new() { Score = 3.5, ExpectedCount = 4 },
                new() { Score = 4.0, ExpectedCount = 4 },
                new() { Score = 4.5, ExpectedCount = 2 },
                new() { Score = 5.0, ExpectedCount = 0 }
            }
        },
        new()
        {
            JobLevel = "Ejecutor",
            TotalRequirements = 12,
            Requirements = new()
            {
                new() { Score = 3.0, ExpectedCount = 3 },
                new() { Score = 3.5, ExpectedCount = 4 },
                new() { Score = 4.0, ExpectedCount = 4 },
                new() { Score = 4.5, ExpectedCount = 1 },
                new() { Score = 5.0, ExpectedCount = 0 }
            }
        },
        new()
        {
            JobLevel = "Operativo",
            TotalRequirements = 10,
            Requirements = new()
            {
                new() { Score = 3.0, ExpectedCount = 3 },
                new() { Score = 3.5, ExpectedCount = 4 },
                new() { Score = 4.0, ExpectedCount = 3 },
                new() { Score = 4.5, ExpectedCount = 0 },
                new() { Score = 5.0, ExpectedCount = 0 }
            }
        }
    };
    }
}
