using System.Text.Json;
using ChampionImporter.Dtos;
using Microsoft.Extensions.Configuration;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using Spectre.Console;

namespace ChampionImporter;

public class ImportService
{
    private readonly ManagerContext _dbContext;
    private readonly string _url;
    private readonly string _jsonPath = "Data/champions.json";

    public ImportService(ManagerContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _url = config.GetSection("DataDragon").GetSection("ChampionImageUrl").Value
               ?? throw new Exception("Missing configuration section 'DataDragon'.");
    }

    public async Task StartImport()
    {
        var figlet = new FigletText("ChampionImporter").Color(Color.Blue);
        AnsiConsole.Write(figlet);
        AnsiConsole.MarkupLine("\n[cyan]Press Enter to start import.[/]\n");
        Console.ReadLine();
        AnsiConsole.Clear();
        var championsJson = File.OpenRead(_jsonPath);
        AnsiConsole.MarkupLine($"[cyan]Read '{_jsonPath}' as stream...[/]\n");
        try
        {
            var root = await JsonSerializer.DeserializeAsync<ChampionRootDto>(championsJson);
            if (root == null) throw new Exception("Couldn't read the json file.");
            var table = new Table()
                .AddColumn("Champion")
                .AddColumn("Status");
            
            await AnsiConsole.Live(table)
                .StartAsync(async ctx =>
                {
                    foreach (var (_, champion) in root.Data)
                        await MapAndAddChampion(champion, ctx, table);
                });
            
            await _dbContext.SaveChangesAsync();
            AnsiConsole.MarkupLine("[green]✔︎ Import completed.[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLineInterpolated($"[bold red]✗ Error: {e.Message}[/]");
            throw;
        }

    }

    private async Task MapAndAddChampion(ChampionDto champion, LiveDisplayContext ctx, Table table)
    {
        if (_dbContext.Champions.Any(c => c.Id == champion.Id))
        {
            table.AddRow(champion.Name, "[gray]skipped[/]");
            ctx.Refresh();
            return;
        }
            
        var championData = new Champion
        {
            Id = champion.Id,
            Name = champion.Name,
            Image = await ImageExtractor.GetImageFromClient(_url, champion.Image.Full),
            Tags = champion.Tags.Select(t => Enum.Parse<Tag>(t, ignoreCase: true)).ToList()
        };
        _dbContext.Champions.Add(championData);
        
        table.AddRow(champion.Name, "[green]imported[/]");
        ctx.Refresh();
    }

}