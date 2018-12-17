using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace LibraryManager.Api.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source,string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var dataShapedObject = new ExpandoObject();

            
            if (string.IsNullOrWhiteSpace(fields))
            {
                // All public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    // get the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(source);
                }
            }
            else
            {
                // Only the public properties that match the fields shoud be in tht ExpandoObject.
                // The fields are separated by ",", so we split them.
                var fieldsAfterSplit = fields.Split(",");

                foreach (var field in fieldsAfterSplit)
                {
                    /* Trim each field , as is might contain leading or trailing spaces. Can't trim the
                        var in the foreach so use another var */
                    var propertyName = field.Trim();

                    /* Use reflection to get the propert on the source object.
                        We need to include public and instance because specifying a binding flag iverwrutes
                        the already-existing binding flags */
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    }

                    // get the value of the property on the source
                    var propertyValue = propertyInfo.GetValue(source);

                    // Add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
            }

            // return the list
            return dataShapedObject;
        }
    }
}