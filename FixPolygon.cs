/* FixedPointy - A simple fixed-point math library for C#.
 *
 * Copyright (c) 2018 Dmitry Gayazov
 * Copyright (c) 2013 Jameson Ernst
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace FixedPointy {
  public struct FixPolygon {
    public FixVec2[] Edges { get; }
    public FixVec2[] Points { get; }

    public FixPolygon(IEnumerable<FixVec2> points) {
      Points = points.ToArray();
      Edges = new FixVec2[Points.Length];
      BuildEdges();
    }

    private FixPolygon(IEnumerable<FixVec2> points, IEnumerable<FixVec2> edges) {
      Points = points.ToArray();
      Edges = edges.ToArray();
    }

    private void BuildEdges() {
      for (var p = 0; p < Points.Length; p++) {
        var p1 = Points[p];
        var p2 = p + 1 >= Points.Length ? Points[0] : Points[p + 1];
        Edges[p] = p2 - p1;
      }
    }

    public FixVec2 Center {
      get {
        Fix totalX = 0;
        Fix totalY = 0;
        foreach (var p in Points) {
          totalX += p.X;
          totalY += p.Y;
        }

        return new FixVec2(totalX / Points.Length, totalY / Points.Length);
      }
    }

    public FixPolygon Rotate(int degree) {
      var points = new List<FixVec2>();
      Array.ForEach(Points, p => points.Add(p.Rotate(degree)));
      return new FixPolygon(points);
    }

    public static FixPolygon operator +(FixPolygon polygon, FixVec2 vector) {
      var points = polygon.Points.Select(t => t + vector).ToList();
      return new FixPolygon(points, polygon.Edges);
    }

    public override string ToString() {
      return Points.Aggregate("", (acc, p) => acc + p + " ");
    }
  }
}