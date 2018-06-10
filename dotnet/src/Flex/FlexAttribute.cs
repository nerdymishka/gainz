using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    public class FlexAttribute : FObject, IEquatable<FlexAttribute>
    {
        public FlexSymbol Name { get; protected set; }

        public string Value { get; set; }

        internal int HashCode { get; set; }

        public override FlexType FlexType => FlexType.FlexAttribute;

        public FlexAttribute(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentEmptyException(nameof(name));

            this.Name = name;
            this.Value = value;
        }

        public static bool operator ==(FlexAttribute left, FlexAttribute right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (ReferenceEquals(left, null))
                return false;

            if (ReferenceEquals(right, null))
                return false;

            return left.Name == right.Name && left.Value == right.Value;
        }

        public static bool operator !=(FlexAttribute left, FlexAttribute right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is FlexAttribute)
                return this.Equals((FlexAttribute)obj);

            return base.Equals(obj);
        }


        public override int GetHashCode()
        {
            return this.Name.ToString().GetHashCode() + this.Value.GetHashCode();
        }

        public override object Unbox()
        {
            return new KeyValuePair<string, string>(this.Name, this.Value);
        }

        public bool Equals(FlexAttribute other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return $"{this.Name}={this.Value}";
        }
    }
}
