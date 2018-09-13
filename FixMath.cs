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

namespace FixedPointy {
  public static partial class FixMath {
    public static readonly Fix Pi;
    public static readonly Fix E;
    private static Fix _log2E;
    private static Fix _log210;
    private static readonly Fix Ln2;
    private static readonly Fix Log102;
    private static readonly Fix[] QuarterSine;
    private static readonly Fix[] CordicAngles;
    private static Fix[] _cordicGains;

    static FixMath() {
      if(QuarterSineResPower >= Fix.FRACTIONAL_BITS)
        throw new Exception("_quarterSineResPower must be less than Fix.FractionalBits.");
      if(QuarterSineConsts.Length != 90 * (1 << QuarterSineResPower) + 1)
        throw new Exception("_quarterSineConst.Length must be 90 * 2^(_quarterSineResPower) + 1.");

      Pi = PiConst;
      E = EConst;
      _log2E = Log2EConst;
      _log210 = Log210Const;
      Ln2 = Ln2Const;
      Log102 = Log102Const;

      QuarterSine = Array.ConvertAll(QuarterSineConsts, c => (Fix)c);
      CordicAngles = Array.ConvertAll(CordicAngleConsts, c => (Fix)c);
      _cordicGains = Array.ConvertAll(CordicGainConsts, c => (Fix)c);
    }

    public static Fix Abs(Fix value) {
      return value.Raw < 0 ? new Fix(-value.Raw) : value;
    }

    public static Fix Sign(Fix value) {
      if(value < 0)
        return -1;
      if(value > 0)
        return 1;

      return 0;
    }

    public static Fix Ceiling(Fix value) {
      return new Fix((value.Raw + Fix.FRACTION_MASK) & Fix.INTEGER_MASK);
    }

    public static Fix Floor(Fix value) {
      return new Fix(value.Raw & Fix.INTEGER_MASK);
    }

    public static Fix Truncate(Fix value) {
      if(value < 0)
        return new Fix((value.Raw + Fix.FRACTION_RANGE) & Fix.INTEGER_MASK);

      return new Fix(value.Raw & Fix.INTEGER_MASK);
    }

    public static Fix Round(Fix value) {
      return new Fix((value.Raw + (Fix.FRACTION_RANGE >> 1)) & ~Fix.FRACTION_MASK);
    }

    public static Fix Min(Fix v1, Fix v2) {
      return v1 < v2 ? v1 : v2;
    }

    public static Fix Max(Fix v1, Fix v2) {
      return v1 > v2 ? v1 : v2;
    }

    public static Fix Sqrt(Fix value) {
      if(value.Raw < 0)
        throw new ArgumentOutOfRangeException(nameof(value), "Value must be non-negative.");

      if(value.Raw == 0)
        return 0;

      return new Fix((int)(SqrtULong((ulong)value.Raw << (Fix.FRACTIONAL_BITS + 2)) + 1) >> 1);
    }

    internal static uint SqrtULong(ulong n) {
      ulong x = 1L << ((31 + Fix.FRACTIONAL_BITS + 2 + 1) / 2);
      while(true) {
        var y = (x + n / x) >> 1;
        if(y >= x)
          return (uint)x;

        x = y;
      }
    }

    public static Fix Sin(Fix degrees) {
      return CosRaw(degrees.Raw - (90 << Fix.FRACTIONAL_BITS));
    }

    public static Fix Cos(Fix degrees) {
      return CosRaw(degrees.Raw);
    }

    private static Fix CosRaw(int raw) {
      raw = raw < 0 ? -raw : raw;
      var t = raw & ((1 << (Fix.FRACTIONAL_BITS - QuarterSineResPower)) - 1);
      raw = raw >> (Fix.FRACTIONAL_BITS - QuarterSineResPower);

      if(t == 0)
        return CosRawLookup(raw);

      var v1 = CosRawLookup(raw);
      var v2 = CosRawLookup(raw + 1);

      return new Fix((int)(
        (
          (long)v1.Raw * ((1 << (Fix.FRACTIONAL_BITS - QuarterSineResPower)) - t)
          + (long)v2.Raw * t
          + (1 << (Fix.FRACTIONAL_BITS - QuarterSineResPower - 1))
        )
        >> (Fix.FRACTIONAL_BITS - QuarterSineResPower)
      ));
    }

    private static Fix CosRawLookup(int raw) {
      raw %= 360 * (1 << QuarterSineResPower);

      if(raw < 90 * (1 << QuarterSineResPower))
        return QuarterSine[90 * (1 << QuarterSineResPower) - raw];

      if(raw < 180 * (1 << QuarterSineResPower)) {
        raw -= 90 * (1 << QuarterSineResPower);
        return -QuarterSine[raw];
      }

      if(raw < 270 * (1 << QuarterSineResPower)) {
        raw -= 180 * (1 << QuarterSineResPower);
        return -QuarterSine[90 * (1 << QuarterSineResPower) - raw];
      }

      raw -= 270 * (1 << QuarterSineResPower);
      return QuarterSine[raw];
    }

    public static Fix Tan(Fix degrees) {
      return Sin(degrees) / Cos(degrees);
    }

    public static Fix Asin(Fix value) {
      return Atan2(value, Sqrt((1 + value) * (1 - value)));
    }

    public static Fix Acos(Fix value) {
      return Atan2(Sqrt((1 + value) * (1 - value)), value);
    }

    public static Fix Atan(Fix value) {
      return Atan2(value, 1);
    }

    public static Fix Atan2(Fix y, Fix x) {
      if(x == 0 && y == 0)
        throw new ArgumentOutOfRangeException("x and y", "x and y cannot both be 0.");

      Fix angle = 0;
      Fix xNew, yNew;

      if(x < 0) {
        if(y < 0) {
          xNew = -y;
          yNew = x;
          angle = -90;
        }
        else if(y > 0) {
          xNew = y;
          yNew = -x;
          angle = 90;
        }
        else {
          xNew = x;
          yNew = y;
          angle = 180;
        }

        x = xNew;
        y = yNew;
      }

      for(var i = 0; i < Fix.FRACTIONAL_BITS + 2; i++) {
        if(y > 0) {
          xNew = x + (y >> i);
          yNew = y - (x >> i);
          angle += CordicAngles[i];
        }
        else if(y < 0) {
          xNew = x - (y >> i);
          yNew = y + (x >> i);
          angle -= CordicAngles[i];
        }
        else {
          break;
        }

        x = xNew;
        y = yNew;
      }

      return angle;
    }

    public static Fix Exp(Fix value) {
      return Pow(E, value);
    }

    public static Fix Pow(Fix b, Fix exp) {
      if(b == 1 || exp == 0)
        return 1;

      int intPow;
      Fix intFactor;
      if((exp.Raw & Fix.FRACTION_MASK) == 0) {
        intPow = (exp.Raw + (Fix.FRACTION_RANGE >> 1)) >> Fix.FRACTIONAL_BITS;
        Fix t;
        int p;
        if(intPow < 0) {
          t = 1 / b;
          p = -intPow;
        }
        else {
          t = b;
          p = intPow;
        }

        intFactor = 1;
        while(p > 0) {
          if((p & 1) != 0)
            intFactor *= t;
          t *= t;
          p >>= 1;
        }

        return intFactor;
      }

      exp *= Log(b, 2);
      intPow = (exp.Raw + (Fix.FRACTION_RANGE >> 1)) >> Fix.FRACTIONAL_BITS;
      intFactor = intPow < 0 ? Fix.One >> -intPow : Fix.One << intPow;

      var x = (
                (exp.Raw - (intPow << Fix.FRACTIONAL_BITS)) * Ln2Const.Raw
                + (Fix.FRACTION_RANGE >> 1)
              ) >> Fix.FRACTIONAL_BITS;
      if(x == 0)
        return intFactor;

      var fracFactor = x;
      var xa = x;
      for(var i = 2; i < InvFactConsts.Length; i++) {
        if(xa == 0)
          break;

        xa *= x;
        xa += 1L << (32 - 1);
        xa >>= 32;
        var p = xa * InvFactConsts[i].Raw;
        p += 1L << (32 - 1);
        p >>= 32;
        fracFactor += p;
      }

      return new Fix((int)(((intFactor.Raw * fracFactor + (1L << (32 - 1))) >> 32) + intFactor.Raw));
    }

    public static Fix Log(Fix value) {
      return Log2(value) * Ln2;
    }

    public static Fix Log(Fix value, Fix b) {
      if(b == 2)
        return Log2(value);

      if(b == E)
        return Log(value);
      if(b == 10)
        return Log10(value);

      return Log2(value) / Log2(b);
    }

    public static Fix Log10(Fix value) {
      return Log2(value) * Log102;
    }

    private static Fix Log2(Fix value) {
      if(value <= 0)
        throw new ArgumentOutOfRangeException(nameof(value), "Value must be positive.");

      var x = (uint)value.Raw;
      var b = 1U << (Fix.FRACTIONAL_BITS - 1);
      uint y = 0;

      while(x < 1U << Fix.FRACTIONAL_BITS) {
        x <<= 1;
        y -= 1U << Fix.FRACTIONAL_BITS;
      }

      while(x >= 2U << Fix.FRACTIONAL_BITS) {
        x >>= 1;
        y += 1U << Fix.FRACTIONAL_BITS;
      }

      ulong z = x;

      for(var i = 0; i < Fix.FRACTIONAL_BITS; i++) {
        z = (z * z) >> Fix.FRACTIONAL_BITS;
        if(z >= 2U << Fix.FRACTIONAL_BITS) {
          z >>= 1;
          y += b;
        }

        b >>= 1;
      }

      return new Fix((int)y);
    }
  }
}