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

namespace FixedPointy {
  public struct FixVec2 {
    public static readonly FixVec2 Zero = new FixVec2();
    public static readonly FixVec2 One = new FixVec2(1, 1);
    public static readonly FixVec2 UnitX = new FixVec2(1, 0);
    public static readonly FixVec2 UnitY = new FixVec2(0, 1);

    public static FixVec2 operator +(FixVec2 rhs) {
      return rhs;
    }

    public static FixVec2 operator -(FixVec2 rhs) {
      return new FixVec2(-rhs.X, -rhs.Y);
    }

    public static FixVec2 operator +(FixVec2 lhs, FixVec2 rhs) {
      return new FixVec2(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }

    public static FixVec2 operator -(FixVec2 lhs, FixVec2 rhs) {
      return new FixVec2(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static FixVec2 operator +(FixVec2 lhs, Fix rhs) {
      return lhs.ScalarAdd(rhs);
    }

    public static FixVec2 operator +(Fix lhs, FixVec2 rhs) {
      return rhs.ScalarAdd(lhs);
    }

    public static FixVec2 operator -(FixVec2 lhs, Fix rhs) {
      return new FixVec2(lhs.X - rhs, lhs.Y - rhs);
    }

    public static FixVec2 operator *(FixVec2 lhs, Fix rhs) {
      return lhs.ScalarMultiply(rhs);
    }

    public static FixVec2 operator *(Fix lhs, FixVec2 rhs) {
      return rhs.ScalarMultiply(lhs);
    }

    public static FixVec2 operator /(FixVec2 lhs, Fix rhs) {
      return new FixVec2(lhs.X / rhs, lhs.Y / rhs);
    }

    public static bool operator ==(FixVec2 lhs, FixVec2 rhs) {
      return lhs.X == rhs.X && lhs.Y == rhs.Y;
    }

    public static bool operator !=(FixVec2 lhs, FixVec2 rhs) {
      return !(lhs == rhs);
    }

    public FixVec2(Fix x, Fix y) {
      X = x;
      Y = y;
    }

    public Fix X { get; }

    public Fix Y { get; }

    public Fix Dot(FixVec2 rhs) {
      return X * rhs.X + Y * rhs.Y;
    }

    public Fix Cross(FixVec2 rhs) {
      return X * rhs.Y - Y * rhs.X;
    }

    private FixVec2 ScalarAdd(Fix value) {
      return new FixVec2(X + value, Y + value);
    }

    private FixVec2 ScalarMultiply(Fix value) {
      return new FixVec2(X * value, Y * value);
    }

    public Fix GetMagnitude() {
      if (X == 0 && Y == 0)
        return Fix.Zero;

      var n = (ulong)(X.Raw * (long)X.Raw + Y.Raw * (long)Y.Raw);

      return new Fix((int)(FixMath.SqrtULong(n << 2) + 1) >> 1);
    }

    public FixVec2 Normalize() {
      if (X == 0 && Y == 0)
        return Zero;

      var m = GetMagnitude();

      return new FixVec2(X / m, Y / m);
    }

    public FixVec2 Rotate(Fix degree) {
      var cos = FixMath.Cos(degree);
      var sin = FixMath.Sin(degree);
      return new FixVec2(X * cos - Y * sin, X * sin + Y * cos);
    }

    public override bool Equals(object obj) {
      var v = obj as FixVec2?;
      return v.HasValue && X == v.Value.X && Y == v.Value.Y;
    }

    public override int GetHashCode() {
      return X.GetHashCode() ^ (Y.GetHashCode() << 2);
    }

    public override string ToString() {
      return $"({X}, {Y})";
    }
  }
}