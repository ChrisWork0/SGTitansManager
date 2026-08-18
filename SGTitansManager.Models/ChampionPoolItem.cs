namespace SGTitansManager.Models;

public class ChampionPoolItem
{
    public required Champion Champion { get; set; }
    public int ChampionId { get; set; }
    public SkillLevel SkillLevel { get; set; }
    public Guid PlayerId { get; set; }
}

public enum SkillLevel
{
    Novice,
    Intermediate,
    Advanced,
    Master
}