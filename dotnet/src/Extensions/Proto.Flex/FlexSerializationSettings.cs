

using System.Collections.Generic;
using NerdyMishka.ComponentModel.ValueConversion;

namespace NerdyMishka.Extensions.Flex 
{

    public interface IFlexSerializationSettings
    {
         bool OmitNulls { get;  }

        List<ValueConverter> ValueConverters { get;  }
    }

    public interface IMutableFlexSerializationSettings
    {
        bool OmitNulls { get; set; }

        List<ValueConverter> ValueConverters { get; set; }
    }

    public class FlexSerializationSettings : IFlexSerializationSettings, IMutableFlexSerializationSettings
    {
        public bool OmitNulls { get; set; }

        public List<ValueConverter> ValueConverters { get; set; }
    }
}