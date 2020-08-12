// https://github.com/dotnet/aspnetcore/blob/master/src/Components/Components/src/Routing/RouteConstraint.cs

using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace BlazorTenant
{
    internal abstract class MultiTenantRouteConstraint
    {
        // note: the things that prevent this cache from growing unbounded is that
        // we're the only caller to this code path, and the fact that there are only
        // 8 possible instances that we create.
        //
        // The values passed in here for parsing are always static text defined in route attributes.
        private static readonly ConcurrentDictionary<string, MultiTenantRouteConstraint> _cachedConstraints
            = new ConcurrentDictionary<string, MultiTenantRouteConstraint>();

        public abstract bool Match(string pathSegment, out object convertedValue);

        public static MultiTenantRouteConstraint Parse(string template, string segment, string constraint)
        {
            if (string.IsNullOrEmpty(constraint))
            {
                throw new ArgumentException($"Malformed segment '{segment}' in route '{template}' contains an empty constraint.");
            }

            if (_cachedConstraints.TryGetValue(constraint, out var cachedInstance))
            {
                return cachedInstance;
            }
            else
            {
                var newInstance = CreateRouteConstraint(constraint);
                if (newInstance != null)
                {
                    // We've done to the work to create the constraint now, but it's possible
                    // we're competing with another thread. GetOrAdd can ensure only a single
                    // instance is returned so that any extra ones can be GC'ed.
                    return _cachedConstraints.GetOrAdd(constraint, newInstance);
                }
                else
                {
                    throw new ArgumentException($"Unsupported constraint '{constraint}' in route '{template}'.");
                }
            }
        }

        /// <summary>
        /// Creates a structured RouteConstraint object given a string that contains
        /// the route constraint. A constraint is the place after the colon in a
        /// parameter definition, for example `{age:int?}`.
        ///
        /// If the constraint denotes an optional, this method will return an
        /// <see cref="OptionalTypeRouteConstraint{T}" /> which handles the appropriate checks.
        /// </summary>
        /// <param name="constraint">String representation of the constraint</param>
        /// <returns>Type-specific RouteConstraint object</returns>
        private static MultiTenantRouteConstraint CreateRouteConstraint(string constraint)
        {
            switch (constraint)
            {
                case "bool":
                    return new MultiTenantTypeRouteConstraint<bool>(bool.TryParse);
                case "bool?":
                    return new MultiTenantOptionalTypeRouteConstraint<bool>(bool.TryParse);
                case "datetime":
                    return new MultiTenantTypeRouteConstraint<DateTime>((string str, out DateTime result)
                        => DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out result));
                case "datetime?":
                    return new MultiTenantOptionalTypeRouteConstraint<DateTime>((string str, out DateTime result)
                        => DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out result));
                case "decimal":
                    return new MultiTenantTypeRouteConstraint<decimal>((string str, out decimal result)
                        => decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result));
                case "decimal?":
                    return new MultiTenantOptionalTypeRouteConstraint<decimal>((string str, out decimal result)
                        => decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result));
                case "double":
                    return new MultiTenantTypeRouteConstraint<double>((string str, out double result)
                        => double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result));
                case "double?":
                    return new MultiTenantOptionalTypeRouteConstraint<double>((string str, out double result)
                        => double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result));
                case "float":
                    return new MultiTenantTypeRouteConstraint<float>((string str, out float result)
                        => float.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result));
                case "float?":
                    return new MultiTenantOptionalTypeRouteConstraint<float>((string str, out float result)
                        => float.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out result));
                case "guid":
                    return new MultiTenantTypeRouteConstraint<Guid>(Guid.TryParse);
                case "guid?":
                    return new MultiTenantOptionalTypeRouteConstraint<Guid>(Guid.TryParse);
                case "int":
                    return new MultiTenantTypeRouteConstraint<int>((string str, out int result)
                        => int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result));
                case "int?":
                    return new MultiTenantOptionalTypeRouteConstraint<int>((string str, out int result)
                        => int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result));
                case "long":
                    return new MultiTenantTypeRouteConstraint<long>((string str, out long result)
                        => long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result));
                case "long?":
                    return new MultiTenantOptionalTypeRouteConstraint<long>((string str, out long result)
                        => long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result));
                default:
                    return null;
            }
        }
    }
}