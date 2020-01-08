

using System.Collections.Generic;
using NerdyMishka.ComponentModel.ValueConversion;
using NerdyMishka.Reflection;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Extensions.Flex 
{

    public interface IFlexSerializationSettings
    {
        bool OmitNulls { get;  }

        bool OmitEncryption { get; }

        IReflectionCache ReflectionCache { get; }

        List<ValueConverter> ValueConverters { get;  }

        ISymmetricEncryptionProvider EncryptionProvider { get; }
    }

    public interface IMutableFlexSerializationSettings
    {
        bool OmitNulls { get; set; }

        List<ValueConverter> ValueConverters { get; set; }

        bool OmitEncryption { get; set; }

        IReflectionCache ReflectionCache { get; set; }

        ISymmetricEncryptionProvider EncryptionProvider { get; set; }
    }

    public class FlexSerializationSettings : IFlexSerializationSettings, IMutableFlexSerializationSettings
    {
        public bool OmitNulls { get; set; }

        public bool OmitEncryption { get; set; }

        public IReflectionCache ReflectionCache { get; set; } = new ReflectionCache();

        public List<ValueConverter> ValueConverters { get; set; }

        public ISymmetricEncryptionProvider EncryptionProvider { get; set; }
    }
}