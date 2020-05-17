using AutoMapper;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Infrastructure.AzureEventGrid
{
    public sealed class EventsDataMappingProfile : Profile
    {
        public EventsDataMappingProfile()
        {
            CreateMap<IMapLocation, MapLocation>();

            CreateMap<AccidentReport, AccidentReportedEventData>()
                .ForMember(
                    x => x.ReportId,
                    opts => opts.MapFrom(x => x.Id)
                );
        }
    }
}