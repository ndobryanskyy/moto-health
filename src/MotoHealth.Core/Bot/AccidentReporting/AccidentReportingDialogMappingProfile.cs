using AutoMapper;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public class AccidentReportingDialogMappingProfile : Profile
    {
        public AccidentReportingDialogMappingProfile()
        {
            CreateMap<IAccidentReportingDialogState, AccidentDetails>();
        }
    }
}