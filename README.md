# DistributedDrawing
A  distributed drawing tool that uses an external SignalR Hub to sync drawings between clients. It also saves the drawings automatically.

## Demo
It can be demoed at [kristofferstrube.github.io/DistributedDrawing](https://kristofferstrube.github.io/DistributedDrawing/)

## Frontend
I have experimented with a couple of different ways to draw the lines.

The first approach was to add a `@foreach` loop that renders `<Line />` tags in the SVG using razor syntax, but that caused a big delay as all previous lines had to be drawn each time a single line was added

Next I looked at using the [Canvas](https://github.com/BlazorExtensions/Canvas) package by the people behind [BlazorExtensions](https://github.com/BlazorExtensions). This used a canvas instead to render lines which is very effective compared to SVG elements. I had a working version of this, but I did not continue with this as I wanted to be able to potentially manipulate the drawn elements in the future without having to learn how to use graphics libraries.

Next I looked at using JSInterop to add/clear `<Line />` tags from JavaScript, so that I knew that Blazor would not handle when these are rendered.

I have later found that you can do this smarter by creating a component for a SVG `line` like I helped doing in [PetaBridge](https://github.com/petabridge)'s project [DrawTogether.NET](https://github.com/petabridge/DrawTogether.NET/blob/dev/src/DrawTogether.UI/Server/Components/Curve.razor#L16).

## Backend
The backend is not part of this repo, but it not the most complex if you are familiar to SignalR.

```csharp
public class StaticStorage
{
    public static IList<Line> draws = new List<Line>();
    public static IDictionary<string, string> users = new Dictionary<string, string>();
}
    
public class DrawHub : Hub
{
    private readonly string drawingPath = "drawing.draw";

    public override async Task OnConnectedAsync()
    {
        if (StaticStorage.draws.Count() == 0)
        {
            using (FileStream fs = File.OpenRead(drawingPath))
            {
                StaticStorage.draws = await JsonSerializer.DeserializeAsync<IList<Line>>(fs);
            }
        }
        await base.OnConnectedAsync();
    }

    public async Task<int> CountLines()
    {
        return StaticStorage.draws.Count();
    }

    public async IAsyncEnumerable<Line> StartLines([EnumeratorCancellation]
    CancellationToken cancellationToken)
    {
        foreach (Line line in StaticStorage.draws)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return line;
        }
    }

    public async Task<List<string>> StartUsers(string user)
    {
        StaticStorage.users.Add(Context.ConnectionId, user);
        await Clients.Others.SendAsync("ReceiveUsers", StaticStorage.users.Values.ToList());
        return StaticStorage.users.Values.ToList();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        StaticStorage.users.Remove(Context.ConnectionId);
        await Clients.Others.SendAsync("ReceiveUsers", StaticStorage.users.Values.ToList());
        using (FileStream fs = File.Create(drawingPath))
        {
            await JsonSerializer.SerializeAsync(fs, StaticStorage.draws);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task Draw(double prevX, double prevY, double currX, double currY, string color, float lineWidth)
    {
        await Clients.Others.SendAsync("ReceiveDraw", prevX, prevY, currX, currY, color, lineWidth);
        StaticStorage.draws.Add(new Line(prevX, prevY, currX, currY, color, lineWidth));
    }

    public async Task Clear()
    {
        await Clients.All.SendAsync("ReceiveClear");
        StaticStorage.draws.Clear();
    }
}
```
