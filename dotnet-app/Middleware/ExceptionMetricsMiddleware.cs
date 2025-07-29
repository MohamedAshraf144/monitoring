using dotnet_app.Services;
using System.Diagnostics;

namespace dotnet_app.Middleware
{
    public class ExceptionMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMetricsService _metricsService;
        private readonly ILogger<ExceptionMetricsMiddleware> _logger;

        public ExceptionMetricsMiddleware(RequestDelegate next, IMetricsService metricsService,
            ILogger<ExceptionMetricsMiddleware> logger)
        {
            _next = next;
            _metricsService = metricsService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var endpoint = context.Request.Path.Value ?? "";

            try
            {
                await _next(context);

                // تسجيل الطلب الناجح
                stopwatch.Stop();
                _metricsService.RecordRequestDuration(method, endpoint, stopwatch.Elapsed.TotalSeconds);
                _metricsService.IncrementRequestCounter(method, endpoint, context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // تسجيل الاستثناء والمقاييس
                _metricsService.IncrementExceptionCounter(ex.GetType().Name, method, endpoint);
                _metricsService.RecordRequestDuration(method, endpoint, stopwatch.Elapsed.TotalSeconds);
                _metricsService.IncrementRequestCounter(method, endpoint, 500);

                _logger.LogError(ex, "حدث استثناء غير معالج في {Method} {Endpoint}", method, endpoint);

                // إعادة رمي الاستثناء للحفاظ على المعالجة الطبيعية
                throw;
            }
        }
    }
}
