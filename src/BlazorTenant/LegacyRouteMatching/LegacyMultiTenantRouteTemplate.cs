// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Components/src/Routing/LegacyRouteMatching/LegacyRouteTemplate.cs

using System.Diagnostics;
using System.Linq;

namespace BlazorTenant
{
    [DebuggerDisplay("{TemplateText}")]
    internal class LegacyMultiTenantRouteTemplate
    {
        public LegacyMultiTenantRouteTemplate(string templateText, LegacyMultiTenantTemplateSegment[] segments)
        {
            TemplateText = templateText;
            Segments = segments;
            OptionalSegmentsCount = segments.Count(template => template.IsOptional);
        }

        public string TemplateText { get; }

        public LegacyMultiTenantTemplateSegment[] Segments { get; }

        public int OptionalSegmentsCount { get; }
    }
}