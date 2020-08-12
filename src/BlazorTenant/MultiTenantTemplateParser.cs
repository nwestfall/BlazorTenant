// https://github.com/dotnet/aspnetcore/blob/master/src/Components/Components/src/Routing/TemplateParser.cs

using System;

namespace BlazorTenant
{
    // This implementation is temporary, in the future we'll want to have
    // a more performant/properly designed routing set of abstractions.
    // To be more precise these are some things we are scoping out:
    // * We are not doing link generation.
    // * We are not supporting all the route constraint formats supported by ASP.NET server-side routing.
    // The class in here just takes care of parsing a route and extracting
    // simple parameters from it.
    // Some differences with ASP.NET Core routes are:
    // * We don't support catch all parameter segments.
    // * We don't support complex segments.
    // The things that we support are:
    // * Literal path segments. (Like /Path/To/Some/Page)
    // * Parameter path segments (Like /Customer/{Id}/Orders/{OrderId})
    internal class MultiTenantTemplateParser
    {
        public static readonly char[] InvalidParameterNameCharacters =
            new char[] { '*', '{', '}', '=', '.' };

        internal static MultiTenantRouteTemplate ParseTemplate(string template)
        {
            var originalTemplate = template;
            template = template.Trim('/');
            if (template == string.Empty)
            {
                // Special case "/";
                return new MultiTenantRouteTemplate("/", Array.Empty<MultiTenantTemplateSegment>());
            }

            var segments = template.Split('/');
            var templateSegments = new MultiTenantTemplateSegment[segments.Length];
            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (string.IsNullOrEmpty(segment))
                {
                    throw new InvalidOperationException(
                        $"Invalid template '{template}'. Empty segments are not allowed.");
                }

                if (segment[0] != '{')
                {
                    if (segment[segment.Length - 1] == '}')
                    {
                        throw new InvalidOperationException(
                            $"Invalid template '{template}'. Missing '{{' in parameter segment '{segment}'.");
                    }
                    templateSegments[i] = new MultiTenantTemplateSegment(originalTemplate, segment, isParameter: false);
                }
                else
                {
                    if (segment[segment.Length - 1] != '}')
                    {
                        throw new InvalidOperationException(
                            $"Invalid template '{template}'. Missing '}}' in parameter segment '{segment}'.");
                    }

                    if (segment.Length < 3)
                    {
                        throw new InvalidOperationException(
                            $"Invalid template '{template}'. Empty parameter name in segment '{segment}' is not allowed.");
                    }

                    var invalidCharacter = segment.IndexOfAny(InvalidParameterNameCharacters, 1, segment.Length - 2);
                    if (invalidCharacter != -1)
                    {
                        throw new InvalidOperationException(
                            $"Invalid template '{template}'. The character '{segment[invalidCharacter]}' in parameter segment '{segment}' is not allowed.");
                    }

                    templateSegments[i] = new MultiTenantTemplateSegment(originalTemplate, segment.Substring(1, segment.Length - 2), isParameter: true);
                }
            }

            for (int i = 0; i < templateSegments.Length; i++)
            {
                var currentSegment = templateSegments[i];
                if (!currentSegment.IsParameter)
                {
                    continue;
                }

                for (int j = i + 1; j < templateSegments.Length; j++)
                {
                    var nextSegment = templateSegments[j];

                    if (currentSegment.IsOptional && !nextSegment.IsOptional)
                    {
                        throw new InvalidOperationException($"Invalid template '{template}'. Non-optional parameters or literal routes cannot appear after optional parameters.");
                    }

                    if (string.Equals(currentSegment.Value, nextSegment.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            $"Invalid template '{template}'. The parameter '{currentSegment}' appears multiple times.");
                    }
                }
            }

            return new MultiTenantRouteTemplate(template, templateSegments);
        }
    }
}