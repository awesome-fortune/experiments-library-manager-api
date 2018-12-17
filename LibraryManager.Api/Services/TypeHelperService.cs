using System;
using System.Reflection;

namespace LibraryManager.Api.Services
{
    public class TypeHelperService : ITypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // The fields are separated by "," so we split them
            var fieldsAfterSplit = fields.Split(",");

            foreach (var field in fieldsAfterSplit)
            {
                /* Trim each field , as is might contain leading or trailing spaces. Can't trim the
                    var in the foreach so use another var */
                var propertyName = field.Trim();

                // Use reflection to get the propert on T
                var propertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // It can't be found, return false
                if (propertyInfo == null)
                {
                    return false;
                }
            }

            // All checks out, return true
            return true;
        }
    }
}