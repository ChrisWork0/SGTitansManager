namespace SGTitansManager.Models;

public class Player : BaseModel
{
    public string GameName { get; set; } = "";
    public List<Position> Positions { get; set; } = [];
    public List<PlayerRank> PlayerRanks { get; set; } = [];
    public Position? MainPosition { get; set; }
    public bool Core {get; set;}
    public string CorePlayerImage { get; set; } = "";
    public bool TryOut {get; set;}
    public string Opgg { get; set; } = "";
    public List<Availability> Availabilities { get; set; } = [];
    public List<ChampionPoolItem> ChampionPool { get; set; } = [];
}

public enum Position
{
    Top,
    Jungle,
    Mid,
    Bot,
    Support
}