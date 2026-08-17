namespace SGTitansManager.Models;

public class Champion
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Image { get; set; } = "";

    public List<Tag> Tags { get; set; } = [];
}

public enum Tag
{
    Fighter,
    Mage,
    Bruiser,
    Tank,
    Assassin,
    Marksman,
    Support
}