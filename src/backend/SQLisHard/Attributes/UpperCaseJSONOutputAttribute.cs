

using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;

/// <summary>
/// Retains upper-case JSON output to match old-style API from original version
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class UpperCaseJSONOutput : ActionFilterAttribute {
     public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context == null || context.Result == null)
        {
            return;
        }

        var options = new JsonSerializerOptions(JsonSerializerOptions.Web);
        options.PropertyNamingPolicy = null;
        
         var formatter = new SystemTextJsonOutputFormatter(options);

        (context.Result as Microsoft.AspNetCore.Mvc.OkObjectResult)?.Formatters.Add(formatter);
    }
}