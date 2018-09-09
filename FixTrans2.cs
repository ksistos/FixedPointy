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
  public struct FixTrans2 {
    public static readonly FixTrans2 Identity = new FixTrans2(
      1, 0, 0,
      0, 1, 0
    );

    public static FixTrans2 operator *(FixTrans2 lhs, FixTrans2 rhs) {
      return new FixTrans2(
        lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21,
        lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22,
        lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13,
        lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21,
        lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22,
        lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23
      );
    }

    public static FixVec2 operator *(FixTrans2 lhs, FixVec2 rhs) {
      return new FixVec2(
        lhs.M11 * rhs.X + lhs.M12 * rhs.Y + lhs.M13,
        lhs.M21 * rhs.X + lhs.M22 * rhs.Y + lhs.M23
      );
    }

    public static FixTrans2 MakeRotation(Fix degrees) {
      var cos = FixMath.Cos(degrees);
      var sin = FixMath.Sin(degrees);
      return new FixTrans2(
        cos, -sin, 0,
        sin, cos, 0
      );
    }

    public static FixTrans2 MakeScale(FixVec2 scale) {
      return new FixTrans2(
        scale.X, 0, 0,
        0, scale.Y, 0
      );
    }

    public static FixTrans2 MakeTranslation(FixVec2 delta) {
      return new FixTrans2(
        1, 0, delta.X,
        0, 1, delta.Y
      );
    }

    public FixTrans2(
      Fix m11, Fix m12, Fix m13,
      Fix m21, Fix m22, Fix m23
    ) {
      M11 = m11;
      M12 = m12;
      M13 = m13;
      M21 = m21;
      M22 = m22;
      M23 = m23;
    }

    public FixTrans2(FixVec2 position, FixVec2 scale, Fix rotation) {
      var cos = FixMath.Cos(rotation);
      var sin = FixMath.Sin(rotation);

      M11 = cos * scale.X;
      M12 = -sin * scale.X;
      M13 = position.X;
      M21 = sin * scale.Y;
      M22 = cos * scale.Y;
      M23 = position.Y;
    }

    public Fix M11 { get; }

    public Fix M12 { get; }

    public Fix M13 { get; }

    public Fix M21 { get; }

    public Fix M22 { get; }

    public Fix M23 { get; }

    public FixTrans2 Rotate(Fix degrees) {
      return MakeRotation(degrees) * this;
    }

    public FixTrans2 Scale(FixVec2 scale) {
      return new FixTrans2(
        M11 * scale.X, M12 * scale.X, M13 * scale.X,
        M21 * scale.Y, M22 * scale.Y, M23 * scale.Y
      );
    }

    public FixTrans2 Translate(FixVec2 delta) {
      return new FixTrans2(
        M11, M12, M13 + delta.X,
        M21, M22, M23 + delta.Y
      );
    }

    public FixVec2 Apply(FixVec2 vec) {
      return this * vec;
    }

    public override string ToString() {
      return $"[[{M11}, {M12}, {M13}], [{M21}, {M22}, {M23}]]";
    }
  }
}