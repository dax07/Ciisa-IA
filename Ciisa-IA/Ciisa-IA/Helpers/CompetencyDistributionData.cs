namespace Ciisa_IA.Helpers
{
    public static class CompetencyDistributionData
    {
        public static readonly List<CompetencyDistribution> All = new()
        {
            new CompetencyDistribution
            {
                JobLevel = "Estratégico",
                CompetencyCategories = new Dictionary<string, int>
                {
                    { "Estratégicas", 1 },
                    { "Funcionales", 8 },
                    { "Operativas", 1 },
                    { "Sociales", 2 },
                    { "Intrapersonales", 0 }
                }
            },
            new CompetencyDistribution
            {
                JobLevel = "Funcional",
                CompetencyCategories = new Dictionary<string, int>
                {
                    { "Estratégicas", 0 },
                    { "Funcionales", 9 },
                    { "Operativas", 1 },
                    { "Sociales", 2 },
                    { "Intrapersonales", 0 }
                }
            },
            new CompetencyDistribution
            {
                JobLevel = "Coordinador",
                CompetencyCategories = new Dictionary<string, int>
                {
                    { "Estratégicas", 0 },
                    { "Funcionales", 5 },
                    { "Operativas", 4 },
                    { "Sociales", 3 },
                    { "Intrapersonales", 0 }
                }
            },
            new CompetencyDistribution
            {
                JobLevel = "Ejecutor",
                CompetencyCategories = new Dictionary<string, int>
                {
                    { "Estratégicas", 0 },
                    { "Funcionales", 4 },
                    { "Operativas", 5 },
                    { "Sociales", 3 },
                    { "Intrapersonales", 0 }
                }
            }
        };
    }

    public class CompetencyDistribution
    {
        public string JobLevel { get; set; }= null!;
        public Dictionary<string, int> CompetencyCategories { get; set; }=null!;
    }
}
