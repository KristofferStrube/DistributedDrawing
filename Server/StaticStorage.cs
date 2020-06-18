using DistributedDrawing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDrawing.Server
{
    public static class StaticStorage
    {
        public static IList<Line> draws = new List<Line>();
    }
}
