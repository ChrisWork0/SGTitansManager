namespace SGTitansManager.Models;

public class History : BaseModel
{
    public string Team { get; set; } = "Soestgaming Titans";
    public string TeamAbbreviation {get; set;} = "SGC";
    public int TeamWins { get; set; }
    public string Opponent { get; set; } = "";
    public string OpponentAbbreviation  { get; set; } = "";
    public int OpponentWins { get; set; }
    public int Games => TeamWins + OpponentWins;
    public Side[] SidesTeam { get; set; } = Array.Empty<Side>();
    public Side[] SidesOpponent => SidesTeam.
        Select(s => s == Side.Blue ? Side.Red : Side.Blue).ToArray();
    public List<string> ImageDetails { get; set; } = [];
}

public enum Side
{
    Blue,
    Red
}