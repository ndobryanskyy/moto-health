using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MotoHealth.Common.AutoMapper;
using MotoHealth.Events.Dto;
using MotoHealth.Functions.AccidentAlerting;
using MotoHealth.Functions.AccidentAlerting.Workflow;

namespace MotoHealth.Functions
{
    public sealed class FunctionsMappingProfile : Profile
    {
        public FunctionsMappingProfile()
        {
            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(new ProtoTimestampToDatetimeConverter());

            CreateMap<MapLocationDto, AccidentAlertingWorkflowInput.MapLocation>();
            CreateMap<AccidentReportDto, AccidentAlertingWorkflowInput.AccidentReportSummary>();
            CreateMap<AccidentAlertDto, AccidentAlertingWorkflowInput>();

            CreateMap<AccidentAlertingWorkflowInput.AccidentReportSummary, AccidentTableEntity>()
                .ForMember(x => x.RowKey, opts => opts.Ignore())
                .ForMember(x => x.PartitionKey, opts => opts.Ignore())
                .ForMember(x => x.Timestamp, opts => opts.Ignore())
                .ForMember(x => x.ETag, opts => opts.Ignore())
                .ForMember(x => x.AnyChatAlerted, opts => opts.Ignore())
                .ForMember(x => x.ReportHandledAtUtc, opts => opts.Ignore());

            CreateMap<RecordAccidentActivityInput, AccidentTableEntity>()
                .IncludeMembers(x => x.AccidentReport)
                .ForMember(x => x.RowKey, opts => opts.MapFrom(x => AccidentTableEntity.EntityRowKey))
                .ForMember(x => x.PartitionKey, opts => opts.MapFrom(x => x.AccidentReport.Id))
                .ForMember(x => x.Timestamp, opts => opts.Ignore())
                .ForMember(x => x.ETag, opts => opts.Ignore());
        }
    }
}