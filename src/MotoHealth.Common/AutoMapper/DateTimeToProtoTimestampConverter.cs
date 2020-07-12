using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace MotoHealth.Common.AutoMapper
{
    public sealed class DateTimeToProtoTimestampConverter : ITypeConverter<DateTime, Timestamp>
    {
        public Timestamp Convert(DateTime source, Timestamp destination, ResolutionContext context)
            => Timestamp.FromDateTime(source.ToUniversalTime());
    }
}