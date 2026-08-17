using System.Text.Json;
using ChampionImporter.Dtos;
using Microsoft.Extensions.Configuration;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;

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
        Console.WriteLine("Press Enter to start import.");
        Console.ReadLine();
        var championsJson = File.OpenRead(_jsonPath);
        Console.WriteLine($"Read '{_jsonPath}' as stream...");
        try
        {
            await foreach (ChampionDto? champion in JsonSerializer
                               .DeserializeAsyncEnumerable<ChampionDto>(championsJson))
            {
                if (champion == null) continue;
                var championData = new Champion
                {
                    Id = champion.Id,
                    Name = champion.Name,
                    Image = await ImageExtractor.GetImageFromClient(_url, champion.ImagePath),
                    Tags = champion.Tags.Select(t => Enum.Parse<Tag>(t, ignoreCase: true)).ToList()
                };
                _dbContext.Champions.Add(championData);
                Console.WriteLine($"Imported '{champion.Name} (ID: {champion.Id})'");
            }

            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Import completed.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
        
    }
}