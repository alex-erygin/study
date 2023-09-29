using Microsoft.AspNetCore.Http.Extensions;
using Orleans.Runtime;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("urls");
});


var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/shorten/{redirect}",
    async (IGrainFactory grains, HttpRequest request, string redirect) =>
    {
        // Create a unique, short ID
        var id = Guid.NewGuid().GetHashCode().ToString("X");
        var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(id);
        await shortenerGrain.SetUrl(redirect);
        
        // Return the shortened URL for later use
        var resultBuilder = new UriBuilder($"{ request.Scheme }://{ request.Host.Value}")
        {
            Path = $"/go/{id}"
        };
        
        return Results.Ok(resultBuilder.Uri);
    });

app.MapGet("/go/{id}",
    async (IGrainFactory grains, string id) =>
    {
        var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(id);
        var url = await shortenerGrain.GetUrl();
        return Results.Redirect("https://"+ url);
    });

app.Run();


public interface IUrlShortenerGrain : IGrainWithStringKey
{
    Task SetUrl(string fullUrl);
    Task<string> GetUrl();
}

public class UrlShortenerGrain : Grain, IUrlShortenerGrain
{
    private readonly IPersistentState<UrlDetails> _state;

    public UrlShortenerGrain(
        [PersistentState(
            stateName: "url",
            storageName: "urls")] 
            IPersistentState<UrlDetails> state)
    {
        _state = state;
    }
    
    public async Task SetUrl(string fullUrl)
    {
        _state.State = new UrlDetails { ShortenedRouteSegment = this.GetPrimaryKeyString(), FullUrl = fullUrl };
        await _state.WriteStateAsync();
    }

    public Task<string> GetUrl()
    {
        return Task.FromResult(_state.State.FullUrl);
    }
}

[GenerateSerializer]
public record UrlDetails
{
    public string FullUrl { get; set; }
    public string ShortenedRouteSegment { get; set; }
}