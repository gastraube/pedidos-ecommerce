using System.Diagnostics;
using MediatR;
using Serilog;

namespace OrderManagementAPI.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString();

        Log.Information("Starting request {RequestName} {@Request} ({RequestId})", requestName, request, requestId);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next();

            sw.Stop();
            Log.Information("Completed request {RequestName} with response {@Response} in {ElapsedMilliseconds}ms ({RequestId})",
                requestName, response, sw.ElapsedMilliseconds, requestId);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            Log.Error(ex, "Request {RequestName} failed in {ElapsedMilliseconds}ms ({RequestId})",
                requestName, sw.ElapsedMilliseconds, requestId);
            throw;
        }
    }
}