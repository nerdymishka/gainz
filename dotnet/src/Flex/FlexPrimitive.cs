using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    public class FlexPrimitive : FlexMeta
    {
   
        private Type clrType;
        private object value;

        public FlexPrimitive()
        {

        }

        public FlexPrimitive(object value)
        {
            this.Value = value;
        }

        public override FlexType FlexType => FlexType.FlexPrimitive;

        public virtual object Value
        {
            get => this.value;
            set
            {
                if(this.value != value)
                {
                    if (this.clrType != null)
                        this.clrType = value.GetType();

                    this.value = value;
                }
            }
        }

        public virtual Type ClrType
        {
            get
            {
                if (this.clrType == null)
                {
                    this.clrType = typeof(object);

                    if (this.Value != null)
                        this.clrType = this.Value.GetType();
                }

                return this.clrType;
            }
            protected set
            {
                this.clrType = value;
            }
        }
        public bool Equals(FlexPrimitive value)
        {
            if (value == null)
                return false;

            return this.Value == value.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null && this.Value == null)
                return true;

            if (obj is FlexPrimitive)
                return this.Equals((FlexPrimitive)obj);

            return this.Value.Equals(obj);
        }


        public override int GetHashCode()
        {
            if(this.value == null)
                return 0;

            return this.value.GetHashCode();
        }

       

        public static implicit operator String(FlexPrimitive primitive)
        {
            return primitive.Value as string;
        }

        public static implicit operator System.Double(FlexPrimitive primitive)
        {
            if (primitive.Value == null)
                throw new InvalidFlexPrimitiveCastException(typeof(Double));

            return (Double)primitive.Value;
        }

        public static implicit operator System.Double?(FlexPrimitive primitive)
        {
            return (Double?)primitive.Value;
        }

        public static implicit operator System.Guid(FlexPrimitive primitive)
        {
            if (primitive.Value == null)
                throw new InvalidFlexPrimitiveCastException(typeof(Guid));

            return (Guid)primitive.Value;
        }

        public static implicit operator Boolean(FlexPrimitive primitive)
        {
            if (primitive.Value == null)
                throw new InvalidFlexPrimitiveCastException(typeof(Boolean));

            return (Boolean)primitive.Value;
        }

        public static implicit operator Boolean?(FlexPrimitive primitive)
        {
            return (Boolean?)primitive.Value;
        }

        public static implicit operator DateTime(FlexPrimitive primitive)
        {
            if (primitive.Value == null)
                throw new InvalidFlexPrimitiveCastException(typeof(DateTime));

            return (DateTime)primitive.Value;
        }

        public static implicit operator DateTime?(FlexPrimitive primitive)
        {
            return (DateTime?)primitive.Value;
        }

        public static implicit operator Decimal?(FlexPrimitive primitive)
        {
            if (primitive.Value == null)
                throw new InvalidFlexPrimitiveCastException(typeof(Decimal));

            return (Decimal)primitive.Value;
        }

        public static implicit operator Int32(FlexPrimitive primordial)
        {

            var v = primordial.Value;

            if (v is int)
                return (int)v;

            if (v is short)
                return (short)v;

            throw new InvalidCastException();
        }

        public static implicit operator Int32?(FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case null:
                    return null;
                case int ni:
                    return ni;
                case short si:
                    return si;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator Int64(FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case long li:
                    return li;
                case int ni:
                    return ni;
                case short si:
                    return si;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator Int64?(FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case null:
                    return null;
                case long li:
                    return li;
                case int ni:
                    return ni;
                case short si:
                    return si;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator Byte(FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case Char c:
                    return (Byte)c;
                case Byte b:
                    return b;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator Char(FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case Char c:
                    return c;
                case Byte b:
                    return (char)b;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator Char?(FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case null:
                    return null;
                case Char c:
                    return c;
                case Byte b:
                    return (char)b;
                default:
                    throw new InvalidCastException();
            }
        }


        public static implicit operator Byte?(FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case null:
                    return null;
                case Char c:
                    return (Byte)c;
                case Byte b:
                    return b;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator UInt32 (FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case uint ni:
                    return ni;
                case ushort si:
                    return si;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator UInt32? (FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case null:
                    return null;
                case uint ni:
                    return ni;
                case ushort si:
                    return si;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator UInt64 (FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case ulong li:
                    return li;
                case uint ni:
                    return ni;
                case ushort si:
                    return si;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator UInt64? (FlexPrimitive primordial)
        {
            var v = primordial.Value;
            switch (primordial.Value)
            {
                case null:
                    return null;
                case ulong li:
                    return li;
                case uint ni:
                    return ni;
                case ushort si:
                    return si;
                default:
                    throw new InvalidCastException();
            }
        }

        public static bool operator ==(FlexPrimitive left, FlexPrimitive right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (ReferenceEquals(left, null))
                return false;

            if (ReferenceEquals(right, null))
                return false;

            return left.value.Equals(right.value);
        }

        

        

        public static bool operator !=(FlexPrimitive left, FlexPrimitive right)
        {
            return !(left == right);
        }

        public void Update(object value)
        {
            this.Value = value;
        }

        public override object Unbox()
        {
            return this.Value;
        }


        public override string ToString()
        {
            if (this.value == null)
                return "null";

            return this.value.ToString();
        }

    }
}
