using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using LibraryManager.Api.Services;

namespace LibraryManager.Api.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                if (mappingDictionary == null)
                {
                    throw new ArgumentNullException("mappingDictionary");
                }

                if (string.IsNullOrWhiteSpace(orderBy))
                {
                    return source;
                }

                // The orderBy string is separated by "," so we split it.
                var orderByAfterSplit = orderBy.Split(",");

                // Apply each orderBy clause in reverse order - otherwise, the
                // IQueryable will be ordered in the wrong order
                foreach (var orderByClause in orderByAfterSplit.Reverse())
                {
                    // Trim the orderByClause, as it might contain leading
                    // or trailling spaces. Can't trim the var in foreach,
                    // so use another var.
                    var trimmedOrderByClause = orderByClause.Trim();

                    // If the sort option ends with " desc", we order
                    // descending, otherwise ascending
                    var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                    // Remove " asc" or " desc" from the orderByClause, so we
                    // get the property name to look for in the mapping dictionary
                    var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                    var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                    // Find the matching property
                    if (!mappingDictionary.ContainsKey(propertyName))
                    {
                        throw new ArgumentException($"Key mapping for {propertyName} is missing");
                    }

                    // Get the PropertyMappingValue
                    var propertyMappingValue = mappingDictionary[propertyName];

                    if (propertyMappingValue == null)
                    {
                        throw new ArgumentNullException("propertyMappingValue");
                    }

                    // Run through the property names in reverse
                    // so the orderBy clauses are applied in the correct order
                    foreach (var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                    {
                        // revert sort order if necessary
                        if (propertyMappingValue.Revert)
                        {
                            orderDescending = !orderDescending;
                        }

                        source = source.OrderBy(destinationProperty + (orderDescending ? " descending": " ascending"));
                    }
                }
                
                return source;
            }
    }
}