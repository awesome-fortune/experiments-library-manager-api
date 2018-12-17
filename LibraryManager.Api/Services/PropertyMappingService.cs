using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManager.Api.Entities;
using LibraryManager.Api.Models;

namespace LibraryManager.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping = 
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new PropertyMappingValue( new List<string>() { "Id" }) },
            { "Genre", new PropertyMappingValue( new List<string>() { "Genre" }) },
            { "Age", new PropertyMappingValue( new List<string>() { "DateOfBirth" }, true) },
            { "Name", new PropertyMappingValue( new List<string>() { "FirstName", "LastName" }) },
        };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // Get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)}, {typeof(TDestination)}>");
        }
    }
}