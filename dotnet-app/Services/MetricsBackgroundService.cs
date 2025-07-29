using dotnet_app.Services;

namespace dotnet_app.Services
{
    public class MetricsBackgroundService : BackgroundService
    {
        private readonly IMetricsService _metricsService;
        private readonly ILogger<MetricsBackgroundService> _logger;

        public MetricsBackgroundService(IMetricsService metricsService, ILogger<MetricsBackgroundService> logger)
        {
            _metricsService = metricsService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Metrics background service started - خدمة المقاييس الخلفية بدأت");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // تحديث استخدام الذاكرة
                    _metricsService.UpdateMemoryUsage();

                    // يمكنك إضافة مقاييس أخرى هنا
                    // مثال: _metricsService.SetActiveConnections(GetActiveConnectionCount());

                    _logger.LogDebug("System metrics updated - تم تحديث مقاييس النظام");

                    // انتظار 30 ثانية قبل التحديث التالي
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // متوقع عند طلب الإلغاء
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating system metrics - خطأ في تحديث مقاييس النظام");

                    // انتظار أطول في حالة الخطأ
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                }
            }

            _logger.LogInformation("Metrics background service stopped - توقفت خدمة المقاييس الخلفية");
        }

        // يمكنك إضافة هذه الطريقة لحساب الاتصالات النشطة (اختياري)
        private int GetActiveConnectionCount()
        {
            // هنا يمكنك إضافة منطق حساب الاتصالات النشطة
            // مثال بسيط: عدد عشوائي للاختبار
            return Random.Shared.Next(1, 100);
        }
    }
}