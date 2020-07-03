using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MotoHealth.Common.AutoMapper;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Events.Dto;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    public sealed class AccidentReportingMappingProfile : Profile
    {
        public AccidentReportingMappingProfile()
        {
            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(new DateTimeToProtoTimestampConverter());

            CreateMap<IMapLocation, MapLocationDto>();
            CreateMap<AccidentReporter, AccidentReporterDto>();
            CreateMap<AccidentDetails, AccidentDetailsDto>();
            CreateMap<AccidentReport, AccidentReportDto>();
        }
    }
}