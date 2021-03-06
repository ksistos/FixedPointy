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
using System.Globalization;
using System.Text;

namespace FixedPointy {
  public struct FixConst {
    public static explicit operator double(FixConst f) {
      return (f.Raw >> 32) + (uint)f.Raw / (uint.MaxValue + 1.0);
    }

    public static implicit operator FixConst(double value) {
      if(value < int.MinValue || value >= int.MaxValue + 1L)
        throw new OverflowException();

      var floor = Math.Floor(value);
      return new FixConst(((long)floor << 32) + (long)((value - floor) * (uint.MaxValue + 1.0) + 0.5));
    }

    public static implicit operator Fix(FixConst value) {
      return new Fix((int)((value.Raw + (1 << (32 - Fix.FRACTIONAL_BITS - 1))) >> (32 - Fix.FRACTIONAL_BITS)));
    }

    public static explicit operator int(FixConst value) {
      if(value.Raw > 0)
        return (int)(value.Raw >> 32);

      return (int)((value.Raw + uint.MaxValue) >> 32);
    }

    public static implicit operator FixConst(int value) {
      return new FixConst(value << 32);
    }

    public static bool operator ==(FixConst lhs, FixConst rhs) {
      return lhs.Raw == rhs.Raw;
    }

    public static bool operator !=(FixConst lhs, FixConst rhs) {
      return lhs.Raw != rhs.Raw;
    }

    public static bool operator >(FixConst lhs, FixConst rhs) {
      return lhs.Raw > rhs.Raw;
    }

    public static bool operator >=(FixConst lhs, FixConst rhs) {
      return lhs.Raw >= rhs.Raw;
    }

    public static bool operator <(FixConst lhs, FixConst rhs) {
      return lhs.Raw < rhs.Raw;
    }

    public static bool operator <=(FixConst lhs, FixConst rhs) {
      return lhs.Raw <= rhs.Raw;
    }

    public static FixConst operator +(FixConst value) {
      return value;
    }

    public static FixConst operator -(FixConst value) {
      return new FixConst(-value.Raw);
    }

    public FixConst(long raw) {
      Raw = raw;
    }

    public long Raw { get; }

    public override bool Equals(object obj) {
      return obj is FixConst && (FixConst)obj == this;
    }

    public override int GetHashCode() {
      return Raw.GetHashCode();
    }

    public override string ToString() {
      var sb = new StringBuilder();
      if(Raw < 0)
        sb.Append(CultureInfo.CurrentCulture.NumberFormat.NegativeSign);
      long abs = (int)this;
      abs = abs < 0 ? -abs : abs;
      sb.Append(abs.ToString());
      var fraction = (ulong)(Raw & uint.MaxValue);
      if(fraction == 0)
        return sb.ToString();

      fraction = Raw < 0 ? uint.MaxValue + 1L - fraction : fraction;
      fraction *= 1000000000L;
      fraction += (uint.MaxValue + 1L) >> 1;
      fraction >>= 32;

      sb.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
      sb.Append(fraction.ToString("D9").TrimEnd('0'));
      return sb.ToString();
    }
  }
}