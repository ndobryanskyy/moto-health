using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MotoHealth.AccidentReporting;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    public sealed class AccidentReportingMappingProfile : Profile
    {
        public AccidentReportingMappingProfile()
        {
            CreateMap<AccidentReport, AccidentReportDto>()
                .ForMember(
                    x => x.ReportedAtUtc, 
                    x => x.MapFrom(x => Timestamp.FromDateTime(x.ReportedAtUtc)));
        }
    }
}