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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

namespace FixedPointy {
  public struct FixVec3 {
    public static readonly FixVec3 Zero = new FixVec3();
    public static readonly FixVec3 One = new FixVec3(1, 1, 1);
    public static readonly FixVec3 UnitX = new FixVec3(1, 0, 0);
    public static readonly FixVec3 UnitY = new FixVec3(0, 1, 0);
    public static readonly FixVec3 UnitZ = new FixVec3(0, 0, 1);

    public static implicit operator FixVec3(FixVec2 value) {
      return new FixVec3(value.X, value.Y, 0);
    }

    public static FixVec3 operator +(FixVec3 rhs) {
      return rhs;
    }

    public static FixVec3 operator -(FixVec3 rhs) {
      return new FixVec3(-rhs.X, -rhs.Y, -rhs.Z);
    }

    public static FixVec3 operator +(FixVec3 lhs, FixVec3 rhs) {
      return new FixVec3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
    }

    public static FixVec3 operator -(FixVec3 lhs, FixVec3 rhs) {
      return new FixVec3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
    }

    public static FixVec3 operator +(FixVec3 lhs, Fix rhs) {
      return lhs.ScalarAdd(rhs);
    }

    public static FixVec3 operator +(Fix lhs, FixVec3 rhs) {
      return rhs.ScalarAdd(lhs);
    }

    public static FixVec3 operator -(FixVec3 lhs, Fix rhs) {
      return new FixVec3(lhs.X - rhs, lhs.Y - rhs, lhs.Z - rhs);
    }

    public static FixVec3 operator *(FixVec3 lhs, Fix rhs) {
      return lhs.ScalarMultiply(rhs);
    }

    public static FixVec3 operator *(Fix lhs, FixVec3 rhs) {
      return rhs.ScalarMultiply(lhs);
    }

    public static FixVec3 operator /(FixVec3 lhs, Fix rhs) {
      return new FixVec3(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
    }

    public FixVec3(Fix x, Fix y, Fix z) {
      X = x;
      Y = y;
      Z = z;
    }

    public Fix X { get; }

    public Fix Y { get; }

    public Fix Z { get; }

    public Fix Dot(FixVec3 rhs) {
      return X * rhs.X + Y * rhs.Y + Z * rhs.Z;
    }

    public FixVec3 Cross(FixVec3 rhs) {
      return new FixVec3(
        Y * rhs.Z - Z * rhs.Y,
        Z * rhs.X - X * rhs.Z,
        X * rhs.Y - Y * rhs.X
      );
    }

    private FixVec3 ScalarAdd(Fix value) {
      return new FixVec3(X + value, Y + value, Z + value);
    }

    private FixVec3 ScalarMultiply(Fix value) {
      return new FixVec3(X * value, Y * value, Z * value);
    }

    public Fix GetMagnitude() {
      if(X == 0 && Y == 0 && Z == 0)
        return Fix.Zero;

      var n = (ulong)(X.Raw * (long)X.Raw + Y.Raw * (long)Y.Raw + Z.Raw * (long)Z.Raw);

      return new Fix((int)(FixMath.SqrtULong(n << 2) + 1) >> 1);
    }

    public FixVec3 Normalize() {
      if(X == 0 && Y == 0 && Z == 0)
        return Zero;

      var m = GetMagnitude();
      return new FixVec3(X / m, Y / m, Z / m);
    }

    public override string ToString() {
      return $"({X}, {Y}, {Z})";
    }
  }
}