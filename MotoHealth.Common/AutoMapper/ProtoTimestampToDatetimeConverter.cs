using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace MotoHealth.Common.AutoMapper
{
    public sealed class ProtoTimestampToDatetimeConverter : ITypeConverter<Timestamp, DateTime>
    {
        public DateTime Convert(Timestamp source, DateTime destination, ResolutionContext context)
            => source.ToDateTime();
    }
}