using DistributedDrawing.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedDrawing.Server
{
    public class DrawHub : Hub
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string drawingPath;

        public DrawHub(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
            if (environment.IsDevelopment())
            {
                drawingPath = "drawing.draw";
            } else
            {
                drawingPath = Path.Combine(_hostingEnvironment.WebRootPath, "drawing.draw");
            }
        }

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
            foreach(Line line in StaticStorage.draws)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return line;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
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

        public async Task Save()
        {
            using (FileStream fs = File.Create(drawingPath))
            {
                await JsonSerializer.SerializeAsync(fs, StaticStorage.draws);
            }
        }
        public async Task Load()
        {
            using (FileStream fs = File.OpenRead(drawingPath))
            {
                StaticStorage.draws = await JsonSerializer.DeserializeAsync<IList<Line>>(fs);
            }
            await Clients.All.SendAsync("ReceiveClear");
            foreach (Line line in StaticStorage.draws)
            {
                await Clients.All.SendAsync("ReceiveDraw", line.prevX, line.prevY, line.currX, line.currY, line.color, line.lineWidth);
            }
        }

        public async Task Clear()
        {
            await Clients.All.SendAsync("ReceiveClear");
            StaticStorage.draws.Clear();
        }
    }
}
