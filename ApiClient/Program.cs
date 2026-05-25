using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

if (args.Length < 3)
{
    Console.WriteLine("Użycie: dotnet run -- <url> <login> <apiToken>");
    Console.WriteLine("Przykład: dotnet run -- http://localhost:5297 admin TWOJ_TOKEN");
    Console.WriteLine("Token API znajdziesz po zalogowaniu jako admin: menu Użytkownicy.");
    return 1;
}

var baseUrl = args[0].TrimEnd('/');
var login = args[1];
var token = args[2];

using var client = new HttpClient { BaseAddress = new Uri(baseUrl + "/") };
client.DefaultRequestHeaders.Add("X-Api-Login", login);
client.DefaultRequestHeaders.Add("X-Api-Token", token);
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

Console.WriteLine($"=== Demo REST API ({baseUrl}) ===");
Console.WriteLine($"Login: {login}\n");

// 1. GET kraje
Console.WriteLine("1) GET /api/countries");
var countries = await client.GetStringAsync("api/countries");
Console.WriteLine(FormatJson(countries));
Console.WriteLine();

// 2. GET podróże
Console.WriteLine("2) GET /api/trips");
var trips = await client.GetStringAsync("api/trips");
Console.WriteLine(FormatJson(trips));

using var tripsDoc = JsonDocument.Parse(trips);
var firstTripId = tripsDoc.RootElement.ValueKind == JsonValueKind.Array && tripsDoc.RootElement.GetArrayLength() > 0
    ? tripsDoc.RootElement[0].GetProperty("id").GetInt32()
    : 0;

if (firstTripId == 0)
{
    Console.WriteLine("\nBrak podróży — pomijam POST wpisu.");
    return 0;
}

// 3. POST wpis dziennika
Console.WriteLine($"\n3) POST /api/journalentries (tripId={firstTripId})");
var newEntry = new
{
    tripId = firstTripId,
    entryDate = DateTime.Today.ToString("yyyy-MM-dd"),
    title = "Wpisy z konsoli API",
    content = "Dodane programem demonstracyjnym ApiClient.",
    rating = 4
};
var postResponse = await client.PostAsJsonAsync("api/journalentries", newEntry);
Console.WriteLine($"Status: {(int)postResponse.StatusCode} {postResponse.ReasonPhrase}");
var postBody = await postResponse.Content.ReadAsStringAsync();
if (!string.IsNullOrWhiteSpace(postBody))
    Console.WriteLine(FormatJson(postBody));

// 4. GET wpisy
Console.WriteLine("\n4) GET /api/journalentries");
var entries = await client.GetStringAsync("api/journalentries");
Console.WriteLine(FormatJson(entries));

Console.WriteLine("\n=== Koniec demo ===");
return 0;

static string FormatJson(string json)
{
    try
    {
        using var doc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
    }
    catch
    {
        return json;
    }
}
