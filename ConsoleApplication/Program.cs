using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Program
{
    static void Main()
    {
        ShowHoldingsAsync();
        Console.ReadLine();
    }

    static async void ShowHoldingsAsync()
    {
        IEnumerable<Holding>? holdings = new List<Holding>();
        using (HttpClient client = new HttpClient()) 
        {
            client.BaseAddress = new Uri("https://localhost:7213/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync("https://localhost:7213/api/Holdings");
            holdings = await JsonSerializer.DeserializeAsync<List<Holding>>(await streamTask);
        }

        var count = 1;
        Console.WriteLine("Portfolio holdings:\n");
        Console.WriteLine("------------------------------------------------");
        foreach (var holding in holdings)
            Console.WriteLine("{0,-3}{1,-10}{2,-20}{3,-10}", $"{count++}.", holding.TickerExchange, holding.Name, holding.Weight.ToString("P2"));
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine("{0,33}{1,-10}", string.Empty, holdings.Sum(_ => _.Weight).ToString("P2"));
    }
}

public class Holding
{
    [JsonPropertyName("tickerExchange")]
    public string TickerExchange { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("weight")]
    public double Weight { get; set; }
}