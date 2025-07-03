using Microsoft.AspNetCore.Diagnostics;
using SQLisHard.General.ErrorLogging;

public class EmailExceptionHandler : IExceptionHandler
{
    private readonly IErrorReporter _exceptionLogger;
    private readonly ILogger<EmailExceptionHandler> _logger;

    public EmailExceptionHandler(IErrorReporter exceptionLogger, ILogger<EmailExceptionHandler> logger)
    {
        _exceptionLogger = exceptionLogger;
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string,string>();
        foreach(var header in httpContext.Request.Headers) {
            headers.Add(header.Key, header.Value);
        }
        var args = new LogArguments(httpContext.Request.Path, headers, 0, "(Not Auth'd)");
        if(httpContext.User.FindFirst("id") != null) {
            args.UserId = int.Parse(httpContext.User.FindFirst("id")!.Value);
            args.Username = "not supported at the moment";
        }

        _exceptionLogger.LogException(exception, args);

        // continue with default behavior, don't interrupt
        return ValueTask.FromResult(false);
    }
}