using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoHealth.Bot.HealthChecks
{
    public sealed class ApplicationHealthReportViewModel
    {
        public string OverallStatus { get; set; } = default!;

        public IReadOnlyList<DependencyHealthViewModel> Dependencies { get; set; } = Array.Empty<DependencyHealthViewModel>();

        public sealed class DependencyHealthViewModel
        {
            public string Name { get; set; } = default!;

            public string Status { get; set; } = default!;

            public string? Description { get; set; }
        }

        public sealed class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<HealthReport, ApplicationHealthReportViewModel>()
                    .ForMember(x => x.OverallStatus, opts => opts.MapFrom(x => x.Status))
                    .ForMember(x => x.Dependencies, opts => opts.MapFrom(x => x.Entries));

                CreateMap<KeyValuePair<string, HealthReportEntry>, DependencyHealthViewModel>()
                    .ForMember(x => x.Name, opts => opts.MapFrom(x => x.Key))
                    .ForMember(x => x.Status, opts => opts.MapFrom(x => x.Value.Status))
                    .ForMember(x => x.Description, opts => opts.MapFrom(x => x.Value.Description));
            }
        }
    }
}