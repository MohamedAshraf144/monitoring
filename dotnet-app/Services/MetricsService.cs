using Prometheus;

namespace dotnet_app.Services
{
    public class MetricsService : IMetricsService
    {
        // عداد الاستثناءات
        private static readonly Counter ExceptionsTotal = Metrics
            .CreateCounter("app_exceptions_total", "إجمالي عدد الاستثناءات",
                new[] { "exception_type", "method", "endpoint" });

        // عداد طلبات HTTP
        private static readonly Counter HttpRequestsTotal = Metrics
            .CreateCounter("http_requests_total", "إجمالي طلبات HTTP",
                new[] { "method", "endpoint", "status_code" });

        // مدة الطلبات
        private static readonly Histogram HttpRequestDuration = Metrics
            .CreateHistogram("http_request_duration_seconds", "مدة طلبات HTTP",
                new[] { "method", "endpoint" });

        // عمليات قاعدة البيانات
        private static readonly Counter DatabaseOperationsTotal = Metrics
            .CreateCounter("database_operations_total", "إجمالي عمليات قاعدة البيانات",
                new[] { "operation", "table", "status" });

        // استدعاءات API الخارجية
        private static readonly Counter ExternalApiCallsTotal = Metrics
            .CreateCounter("external_api_calls_total", "إجمالي استدعاءات API الخارجية",
                new[] { "api_name", "endpoint", "status_code" });

        // العمليات التجارية
        private static readonly Counter BusinessOperationsTotal = Metrics
            .CreateCounter("business_operations_total", "إجمالي العمليات التجارية",
                new[] { "operation_type", "status" });

        // مقاييس النظام
        private static readonly Gauge MemoryUsageBytes = Metrics
            .CreateGauge("app_memory_usage_bytes", "استخدام الذاكرة بالبايت");

        private static readonly Gauge ActiveConnections = Metrics
            .CreateGauge("app_active_connections", "عدد الاتصالات النشطة");

        public void IncrementExceptionCounter(string exceptionType, string method = "", string endpoint = "")
        {
            ExceptionsTotal.WithLabels(exceptionType, method, endpoint).Inc();
        }

        public void IncrementRequestCounter(string method, string endpoint, int statusCode)
        {
            HttpRequestsTotal.WithLabels(method, endpoint, statusCode.ToString()).Inc();
        }

        public void RecordRequestDuration(string method, string endpoint, double duration)
        {
            HttpRequestDuration.WithLabels(method, endpoint).Observe(duration);
        }

        public void RecordDatabaseOperation(string operation, string table, string status)
        {
            DatabaseOperationsTotal.WithLabels(operation, table, status).Inc();
        }

        public void RecordExternalApiCall(string apiName, string endpoint, int statusCode)
        {
            ExternalApiCallsTotal.WithLabels(apiName, endpoint, statusCode.ToString()).Inc();
        }

        public void IncrementBusinessMetric(string operationType, string status)
        {
            BusinessOperationsTotal.WithLabels(operationType, status).Inc();
        }

        public void UpdateMemoryUsage()
        {
            var memoryUsage = GC.GetTotalMemory(false);
            MemoryUsageBytes.Set(memoryUsage);
        }

        public void SetActiveConnections(int count)
        {
            ActiveConnections.Set(count);
        }
    }
}
