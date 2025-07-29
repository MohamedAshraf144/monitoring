namespace dotnet_app.Services
{
    public interface IMetricsService
    {
        void IncrementExceptionCounter(string exceptionType, string method = "", string endpoint = "");
        void IncrementRequestCounter(string method, string endpoint, int statusCode);
        void RecordRequestDuration(string method, string endpoint, double duration);
        void RecordDatabaseOperation(string operation, string table, string status);
        void RecordExternalApiCall(string apiName, string endpoint, int statusCode);
        void IncrementBusinessMetric(string operationType, string status);
        void UpdateMemoryUsage();
        void SetActiveConnections(int count);
    }
}
