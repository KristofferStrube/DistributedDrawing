using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDrawing.Shared
{
    public class Line
    {
        public Line(){}
        public Line(double prevX, double prevY, double currX, double currY, string color, float lineWidth)
        {
            this.prevX = prevX;
            this.prevY = prevY;
            this.currX = currX;
            this.currY = currY;
            this.color = color;
            this.lineWidth = lineWidth;
        }
        public double prevX { get; set; }
        public double currX { get; set; }
        public double prevY { get; set; }
        public double currY { get; set; }
        public string color { get; set; }
        public float lineWidth { get; set; }
    }
}
