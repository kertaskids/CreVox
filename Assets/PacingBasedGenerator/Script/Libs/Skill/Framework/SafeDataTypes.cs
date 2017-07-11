using UnityEngine;

namespace Skill.Framework
{
    public struct SafeInt
    {
        private bool _IsSafed;
        private int _MixKey;
        private int _Value;

        public SafeInt(int value = 0)
        {
            _IsSafed = true;
            _MixKey = Random.Range(9876, 987654321);
            this._Value = value ^ _MixKey;
        }

        public int Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _MixKey = Random.Range(14673, 9943658);
                    _Value = 0 ^ _MixKey;
                }
                return _Value ^ _MixKey;
            }
        }

        public int Key { get { return _MixKey; } }
        public int MixedValue { get { return _Value; } }
        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }

        public override string ToString() { return Value.ToString(); }
        public static SafeInt operator +(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value + i2.Value); }
        public static SafeInt operator -(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value - i2.Value); }
        public static SafeInt operator *(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value * i2.Value); }
        public static SafeInt operator /(SafeInt i1, SafeInt i2) { return new SafeInt(i1.Value / i2.Value); }

        public static bool operator ==(SafeInt x, SafeInt y) { return x.Value == y.Value; }
        public static bool operator !=(SafeInt x, SafeInt y) { return x.Value != y.Value; }


        public static bool operator <(SafeInt i1, SafeInt i2) { return i1.Value < i2.Value; }
        public static bool operator >(SafeInt i1, SafeInt i2) { return i1.Value > i2.Value; }

        public static bool operator <=(SafeInt i1, SafeInt i2) { return i1.Value <= i2.Value; }
        public static bool operator >=(SafeInt i1, SafeInt i2) { return i1.Value >= i2.Value; }

        public static SafeInt operator ++(SafeInt i1) { return new SafeInt(i1.Value + 1); }
        public static SafeInt operator --(SafeInt i1) { return new SafeInt(i1.Value - 1); }
        public static SafeInt operator -(SafeInt i1) { return new SafeInt(i1.Value * -1); }

        public static implicit operator SafeInt(int i) { return new SafeInt(i); }

        public static implicit operator int (SafeInt si) { return si.Value; }
    }
    public struct SafeFloat
    {
        private bool _IsSafed;
        private float _Offset;
        private float _Value;

        public SafeFloat(float value = 0)
        {
            _IsSafed = true;
            _Offset = Random.Range(-1000, 1000);
            this._Value = value + _Offset;
        }

        public float Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _Offset = Random.Range(-1000, 1000);
                    this._Value = 0 + _Offset;
                }
                return _Value - _Offset;
            }
        }

        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }
        public override string ToString() { return Value.ToString(); }
        public static SafeFloat operator +(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value + f2.Value); }
        public static SafeFloat operator -(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value - f2.Value); }
        public static SafeFloat operator *(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value * f2.Value); }
        public static SafeFloat operator /(SafeFloat f1, SafeFloat f2) { return new SafeFloat(f1.Value / f2.Value); }

        public static bool operator ==(SafeFloat x, SafeFloat y) { return x.Value == y.Value; }
        public static bool operator !=(SafeFloat x, SafeFloat y) { return x.Value != y.Value; }

        public static bool operator <(SafeFloat i1, SafeFloat i2) { return i1.Value < i2.Value; }
        public static bool operator >(SafeFloat i1, SafeFloat i2) { return i1.Value > i2.Value; }

        public static bool operator <=(SafeFloat i1, SafeFloat i2) { return i1.Value <= i2.Value; }
        public static bool operator >=(SafeFloat i1, SafeFloat i2) { return i1.Value >= i2.Value; }


        public static SafeFloat operator ++(SafeFloat f1) { return new SafeFloat(f1.Value + 1); }
        public static SafeFloat operator --(SafeFloat f1) { return new SafeFloat(f1.Value - 1); }
        public static SafeFloat operator -(SafeFloat f1) { return new SafeFloat(f1.Value * -1); }


        public static implicit operator SafeFloat(float f) { return new SafeFloat(f); }

        public static implicit operator float (SafeFloat sf) { return sf.Value; }

    }
    public struct SafeBool
    {
        private bool _IsSafed;
        private int _Bit;
        private int _Value;

        public SafeBool(bool value = false)
        {
            _IsSafed = true;
            _Bit = Random.Range(1, 31);
            this._Value = (value) ? 1 << _Bit : 0;
        }

        public bool Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _Bit = Random.Range(1, 31);
                    this._Value = 0;
                }
                return (_Value & (1 << _Bit)) == (1 << _Bit);
            }
        }

        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }

        public override string ToString() { return Value.ToString(); }

        public static bool operator ==(SafeBool x, SafeBool y) { return x.Value == y.Value; }
        public static bool operator !=(SafeBool x, SafeBool y) { return x.Value != y.Value; }

        public static implicit operator SafeBool(bool b) { return new SafeBool(b); }

        public static implicit operator bool (SafeBool sb) { return sb.Value; }



    }
    public struct SafeLong
    {
        private bool _IsSafed;
        private long _MixKey;
        private long _Value;

        public SafeLong(long value = 0)
        {
            _IsSafed = true;
            _MixKey = Random.Range(9876, 987654321);
            this._Value = value ^ _MixKey;
        }

        public long Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _MixKey = Random.Range(14673, 9943658);
                    _Value = 0 ^ _MixKey;
                }
                return _Value ^ _MixKey;
            }
        }

        public long Key { get { return _MixKey; } }
        public long MixedValue { get { return _Value; } }
        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }

        public override string ToString() { return Value.ToString(); }
        public static SafeLong operator +(SafeLong i1, SafeLong i2) { return new SafeLong(i1.Value + i2.Value); }
        public static SafeLong operator -(SafeLong i1, SafeLong i2) { return new SafeLong(i1.Value - i2.Value); }
        public static SafeLong operator *(SafeLong i1, SafeLong i2) { return new SafeLong(i1.Value * i2.Value); }
        public static SafeLong operator /(SafeLong i1, SafeLong i2) { return new SafeLong(i1.Value / i2.Value); }

        public static bool operator ==(SafeLong x, SafeLong y) { return x.Value == y.Value; }
        public static bool operator !=(SafeLong x, SafeLong y) { return x.Value != y.Value; }


        public static bool operator <(SafeLong i1, SafeLong i2) { return i1.Value < i2.Value; }
        public static bool operator >(SafeLong i1, SafeLong i2) { return i1.Value > i2.Value; }

        public static bool operator <=(SafeLong i1, SafeLong i2) { return i1.Value <= i2.Value; }
        public static bool operator >=(SafeLong i1, SafeLong i2) { return i1.Value >= i2.Value; }

        public static SafeLong operator ++(SafeLong i1) { return new SafeLong(i1.Value + 1); }
        public static SafeLong operator --(SafeLong i1) { return new SafeLong(i1.Value - 1); }
        public static SafeLong operator -(SafeLong i1) { return new SafeLong(i1.Value * -1); }

        public static implicit operator SafeLong(long i) { return new SafeLong(i); }

        public static implicit operator long (SafeLong si) { return si.Value; }



        public int[] ToDoubleInt()
        {
            int a1 = (int)(Value & uint.MaxValue);
            int a2 = (int)(Value >> 32);
            return new int[] { a1, a2 };
        }

        public void FromDoubleInt(int a1, int a2)
        {
            long b = a2;
            b = b << 32;
            b = b | (uint)a1;

            if (!_IsSafed)
            {
                _IsSafed = true;
                _MixKey = Random.Range(14673, 9943658);
                _Value = 0 ^ _MixKey;
            }
            this._Value = b ^ _MixKey;
        }
    }

    public struct SafeUInt
    {
        private bool _IsSafed;
        private uint _MixKey;
        private uint _Value;

        public SafeUInt(uint value = 0)
        {
            _IsSafed = true;
            _MixKey = (uint)Random.Range(9876, 987654321);
            this._Value = value ^ _MixKey;
        }

        public uint Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _MixKey = (uint)Random.Range(14673, 9943658);
                    _Value = 0 ^ _MixKey;
                }
                return _Value ^ _MixKey;
            }
        }

        public uint Key { get { return _MixKey; } }
        public uint MixedValue { get { return _Value; } }
        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }

        public override string ToString() { return Value.ToString(); }
        public static SafeUInt operator +(SafeUInt i1, SafeUInt i2) { return new SafeUInt(i1.Value + i2.Value); }
        public static SafeUInt operator -(SafeUInt i1, SafeUInt i2) { return new SafeUInt(i1.Value - i2.Value); }
        public static SafeUInt operator *(SafeUInt i1, SafeUInt i2) { return new SafeUInt(i1.Value * i2.Value); }
        public static SafeUInt operator /(SafeUInt i1, SafeUInt i2) { return new SafeUInt(i1.Value / i2.Value); }

        public static bool operator ==(SafeUInt x, SafeUInt y) { return x.Value == y.Value; }
        public static bool operator !=(SafeUInt x, SafeUInt y) { return x.Value != y.Value; }


        public static bool operator <(SafeUInt i1, SafeUInt i2) { return i1.Value < i2.Value; }
        public static bool operator >(SafeUInt i1, SafeUInt i2) { return i1.Value > i2.Value; }

        public static bool operator <=(SafeUInt i1, SafeUInt i2) { return i1.Value <= i2.Value; }
        public static bool operator >=(SafeUInt i1, SafeUInt i2) { return i1.Value >= i2.Value; }

        public static SafeUInt operator ++(SafeUInt i1) { return new SafeUInt(i1.Value + 1); }
        public static SafeUInt operator --(SafeUInt i1) { return new SafeUInt(i1.Value - 1); }

        public static implicit operator SafeUInt(uint i) { return new SafeUInt(i); }

        public static implicit operator uint (SafeUInt si) { return si.Value; }
    }

    public struct SafeULong
    {
        private bool _IsSafed;
        private ulong _MixKey;
        private ulong _Value;

        public SafeULong(ulong value = 0)
        {
            _IsSafed = true;
            _MixKey = (ulong)Random.Range(9876, 987654321);
            this._Value = value ^ _MixKey;
        }

        public ulong Value
        {
            get
            {
                if (!_IsSafed)
                {
                    _IsSafed = true;
                    _MixKey = (ulong)Random.Range(14673, 9943658);
                    _Value = 0 ^ _MixKey;
                }
                return _Value ^ _MixKey;
            }
        }

        public ulong Key { get { return _MixKey; } }
        public ulong MixedValue { get { return _Value; } }
        public override int GetHashCode() { return Value.GetHashCode(); }
        public override bool Equals(object obj) { return Value.Equals(obj); }

        public override string ToString() { return Value.ToString(); }
        public static SafeULong operator +(SafeULong i1, SafeULong i2) { return new SafeULong(i1.Value + i2.Value); }
        public static SafeULong operator -(SafeULong i1, SafeULong i2) { return new SafeULong(i1.Value - i2.Value); }
        public static SafeULong operator *(SafeULong i1, SafeULong i2) { return new SafeULong(i1.Value * i2.Value); }
        public static SafeULong operator /(SafeULong i1, SafeULong i2) { return new SafeULong(i1.Value / i2.Value); }

        public static bool operator ==(SafeULong x, SafeULong y) { return x.Value == y.Value; }
        public static bool operator !=(SafeULong x, SafeULong y) { return x.Value != y.Value; }


        public static bool operator <(SafeULong i1, SafeULong i2) { return i1.Value < i2.Value; }
        public static bool operator >(SafeULong i1, SafeULong i2) { return i1.Value > i2.Value; }

        public static bool operator <=(SafeULong i1, SafeULong i2) { return i1.Value <= i2.Value; }
        public static bool operator >=(SafeULong i1, SafeULong i2) { return i1.Value >= i2.Value; }

        public static SafeULong operator ++(SafeULong i1) { return new SafeULong(i1.Value + 1); }
        public static SafeULong operator --(SafeULong i1) { return new SafeULong(i1.Value - 1); }


        public static implicit operator SafeULong(ulong l) { return new SafeULong(l); }

        public static implicit operator ulong (SafeULong si) { return si.Value; }



        public uint[] ToDoubleUInt()
        {
            uint a1 = (uint)(Value & uint.MaxValue);
            uint a2 = (uint)(Value >> 32);
            return new uint[] { a1, a2 };
        }

        public void FromDoubleUInt(uint a1, uint a2)
        {
            ulong b = a2;
            b = b << 32;
            b = b | (uint)a1;

            if (!_IsSafed)
            {
                _IsSafed = true;
                _MixKey = (uint)Random.Range(14673, 9943658);
                _Value = 0 ^ _MixKey;
            }
            this._Value = b ^ _MixKey;
        }
    }
}