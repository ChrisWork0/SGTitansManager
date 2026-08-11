namespace SGTitansManager.Models;

public class PlayerRank : BaseModel
{
    public RankType RankType { get; set; }
    public Rank Rank { get; set; }
    public int Division { get; set; }
    public int LeaguePoints { get; set; }
    public Player? Player { get; set; }
    public Guid PlayerId { get; set; }
}

public enum Rank
{
    Challenger,
    GrandMaster,
    Master,
    Diamond,
    Emerald,
    Platinum,
    Gold,
    Silver,
    Bronze,
    Iron
}

public enum RankType
{
    SoloDuo,
    Flex
}