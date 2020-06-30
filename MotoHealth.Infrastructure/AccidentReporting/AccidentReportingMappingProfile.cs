using AutoMapper;
using MotoHealth.Common.Dto;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    public sealed class AccidentReportingMappingProfile : Profile
    {
        public AccidentReportingMappingProfile()
        {
            CreateMap<IMapLocation, MapLocationDto>();
            CreateMap<AccidentReport, AccidentReportDto>();
        }
    }
}