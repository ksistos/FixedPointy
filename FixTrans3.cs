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
  public struct FixTrans3 {
    public static readonly FixTrans3 Identity = new FixTrans3(
      1, 0, 0, 0,
      0, 1, 0, 0,
      0, 0, 1, 0
    );

    public static FixTrans3 operator *(FixTrans3 lhs, FixTrans3 rhs) {
      return new FixTrans3(
        lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31,
        lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32,
        lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33,
        lhs.M11 * rhs.M14 + lhs.M12 * rhs.M24 + lhs.M13 * rhs.M34 + lhs.M14,
        lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31,
        lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32,
        lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33,
        lhs.M21 * rhs.M14 + lhs.M22 * rhs.M24 + lhs.M23 * rhs.M34 + lhs.M24,
        lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31,
        lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32,
        lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33,
        lhs.M31 * rhs.M14 + lhs.M32 * rhs.M24 + lhs.M33 * rhs.M34 + lhs.M34
      );
    }

    public static FixVec3 operator *(FixTrans3 lhs, FixVec3 rhs) {
      return new FixVec3(
        lhs.M11 * rhs.X + lhs.M12 * rhs.Y + lhs.M13 * rhs.Z + lhs.M14,
        lhs.M21 * rhs.X + lhs.M22 * rhs.Y + lhs.M23 * rhs.Z + lhs.M24,
        lhs.M31 * rhs.X + lhs.M32 * rhs.Y + lhs.M33 * rhs.Z + lhs.M34
      );
    }

    public static FixTrans3 MakeRotationZ(Fix degrees) {
      var cos = FixMath.Cos(degrees);
      var sin = FixMath.Sin(degrees);
      return new FixTrans3(
        cos, -sin, 0, 0,
        sin, cos, 0, 0,
        0, 0, 1, 0
      );
    }

    public static FixTrans3 MakeRotationY(Fix degrees) {
      var cos = FixMath.Cos(degrees);
      var sin = FixMath.Sin(degrees);
      return new FixTrans3(
        cos, 0, sin, 0,
        0, 1, 0, 0,
        -sin, 0, cos, 0
      );
    }

    public static FixTrans3 MakeRotationX(Fix degrees) {
      var cos = FixMath.Cos(degrees);
      var sin = FixMath.Sin(degrees);
      return new FixTrans3(
        1, 0, 0, 0,
        0, cos, -sin, 0,
        0, sin, cos, 0
      );
    }

    public static FixTrans3 MakeRotation(FixVec3 degrees) {
      return MakeRotationX(degrees.X)
        .RotateY(degrees.Y)
        .RotateZ(degrees.Z);
    }

    public static FixTrans3 MakeScale(FixVec3 scale) {
      return new FixTrans3(
        scale.X, 0, 0, 0,
        0, scale.Y, 0, 0,
        0, 0, scale.Z, 0
      );
    }

    public static FixTrans3 MakeTranslation(FixVec3 delta) {
      return new FixTrans3(
        1, 0, 0, delta.X,
        0, 1, 0, delta.Y,
        0, 0, 1, delta.Z
      );
    }

    public FixTrans3(
      Fix m11, Fix m12, Fix m13, Fix m14,
      Fix m21, Fix m22, Fix m23, Fix m24,
      Fix m31, Fix m32, Fix m33, Fix m34
    ) {
      M11 = m11;
      M12 = m12;
      M13 = m13;
      M14 = m14;
      M21 = m21;
      M22 = m22;
      M23 = m23;
      M24 = m24;
      M31 = m31;
      M32 = m32;
      M33 = m33;
      M34 = m34;
    }

    // todo: move to quaternions
    public FixTrans3(FixVec3 position, FixVec3 scale, FixVec3 rotation) {
      this = MakeRotationX(rotation.X)
        .RotateY(rotation.Y)
        .RotateZ(rotation.Z)
        .Scale(scale)
        .Translate(position);
    }

    public Fix M11 { get; }
    public Fix M12 { get; }
    public Fix M13 { get; }
    public Fix M14 { get; }
    public Fix M21 { get; }
    public Fix M22 { get; }
    public Fix M23 { get; }
    public Fix M24 { get; }
    public Fix M31 { get; }
    public Fix M32 { get; }
    public Fix M33 { get; }
    public Fix M34 { get; }
    
    public FixTrans3 RotateZ(Fix degrees) {
      return MakeRotationZ(degrees) * this;
    }

    public FixTrans3 RotateY(Fix degrees) {
      return MakeRotationY(degrees) * this;
    }

    public FixTrans3 RotateX(Fix degrees) {
      return MakeRotationX(degrees) * this;
    }

    public FixTrans3 Rotate(FixVec3 degrees) {
      return MakeRotation(degrees);
    }

    public FixTrans3 Scale(FixVec3 scale) {
      return new FixTrans3(
        M11 * scale.X, M12 * scale.X, M13 * scale.X, M14 * scale.X,
        M21 * scale.Y, M22 * scale.Y, M23 * scale.Y, M24 * scale.Y,
        M31 * scale.Z, M32 * scale.Z, M33 * scale.Z, M34 * scale.Z
      );
    }

    public FixTrans3 Translate(FixVec3 delta) {
      return new FixTrans3(
        M11, M12, M13, M14 + delta.X,
        M21, M22, M23, M24 + delta.Y,
        M31, M32, M33, M34 + delta.Z
      );
    }

    public FixVec3 Apply(FixVec3 vec) {
      return this * vec;
    }

    public override string ToString() {
      return $"[[{M11}, {M12}, {M13}], [{M21}, {M22}, {M23}]]";
    }
  }
}