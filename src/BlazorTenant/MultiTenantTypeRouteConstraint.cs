// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Components/src/Routing/TypeRouteConstraint.cs

using System;
using System.Diagnostics.CodeAnalysis;

namespace BlazorTenant
{
    /// <summary>
    /// A route constraint that requires the value to be parseable as a specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the value must be parseable.</typeparam>
    internal class MultiTenantTypeRouteConstraint<T> : MultiTenantRouteConstraint
    {
        public delegate bool TryParseDelegate(string str, [MaybeNullWhen(false)] out T result);

        private readonly TryParseDelegate _parser;

        public MultiTenantTypeRouteConstraint(TryParseDelegate parser)
        {
            _parser = parser;
        }

        public override bool Match(string pathSegment, out object? convertedValue)
        {
            if (_parser(pathSegment, out var result))
            {
                convertedValue = result;
                return true;
            }
            else
            {
                convertedValue = null;
                return false;
            }
        }

        public override string ToString() => typeof(T) switch
        {
            var x when x == typeof(bool) => "bool",
            var x when x == typeof(DateTime) => "datetime",
            var x when x == typeof(decimal) => "decimal",
            var x when x == typeof(double) => "double",
            var x when x == typeof(float) => "float",
            var x when x == typeof(Guid) => "guid",
            var x when x == typeof(int) => "int",
            var x when x == typeof(long) => "long",
            var x => x.Name.ToLowerInvariant()
        };
    }
}