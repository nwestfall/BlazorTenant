// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Components/src/Routing/RouteTemplate.cs

using System.Diagnostics;
using System.Linq;

namespace BlazorTenant
{
    [DebuggerDisplay("{TemplateText}")]
    internal class MultiTenantRouteTemplate
    {
        public MultiTenantRouteTemplate(string templateText, MultiTenantTemplateSegment[] segments)
        {
            TemplateText = templateText;
            Segments = segments;
            OptionalSegmentsCount = segments.Count(template => template.IsOptional);
            ContainsCatchAllSegment = segments.Any(template => template.IsCatchAll);
        }

        public string TemplateText { get; }

        public MultiTenantTemplateSegment[] Segments { get; }

        public int OptionalSegmentsCount { get; }

        public bool ContainsCatchAllSegment { get; }
    }
}