using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace ArchiveAPI.Attributes
{
    public class SwaggerIgnoreFilter : ISchemaFilter
    {      
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
                return;
           
            var excludedProperties = context.Type.GetMembers().Where(t => t.GetCustomAttribute<SwaggerIgnoreAttribute>() != null);
            //context.MemberInfo.GetCustomAttribute<SwaggerIgnoreAttribute>();
            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name.ToLower()))
                    schema.Properties.Remove(excludedProperty.Name);
            }  
        }
    }
}
