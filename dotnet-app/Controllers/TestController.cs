using dotnet_app.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMetricsService _metricsService;
        private readonly ILogger<TestController> _logger;

        public TestController(IMetricsService metricsService, ILogger<TestController> logger)
        {
            _metricsService = metricsService;
            _logger = logger;
        }

        [HttpGet("exception")]
        public IActionResult TestException()
        {
            _logger.LogWarning("رمي استثناء تجريبي عن قصد");
            throw new InvalidOperationException("هذا استثناء تجريبي لاختبار المقاييس");
        }

        [HttpGet("slow")]
        public async Task<IActionResult> SlowEndpoint()
        {
            _logger.LogInformation("محاكاة عملية بطيئة");
            await Task.Delay(3000); // تأخير 3 ثوان
            return Ok(new { message = "هذا كان رد بطيء", duration = "3 ثوان" });
        }

        [HttpGet("error")]
        public IActionResult TestError()
        {
            _logger.LogWarning("إرجاع HTTP 500 للاختبار");
            return StatusCode(500, "خطأ داخلي محاكي في الخادم");
        }

        [HttpGet("metrics-test")]
        public IActionResult TestMetrics()
        {
            // توليد مقاييس تجريبية
            _metricsService.IncrementBusinessMetric("test_operation", "success");
            _metricsService.RecordDatabaseOperation("SELECT", "test_table", "success");
            _metricsService.RecordExternalApiCall("test_api", "/test", 200);

            _logger.LogInformation("تم توليد مقاييس تجريبية بنجاح");

            return Ok(new
            {
                message = "تم توليد المقاييس التجريبية",
                tip = "تحقق من /metrics لرؤية المقاييس المولدة"
            });
        }

        [HttpGet("arabic-test")]
        public IActionResult TestArabicContent()
        {
            _metricsService.IncrementBusinessMetric("arabic_content", "success");

            return Ok(new
            {
                message = "مرحباً! هذا اختبار للمحتوى العربي",
                data = new
                {
                    users = new[] { "أحمد", "فاطمة", "محمد", "عائشة" },
                    status = "نجح الاختبار"
                }
            });
        }

        [HttpGet("load-test")]
        public async Task<IActionResult> LoadTest([FromQuery] int iterations = 10)
        {
            var random = new Random();

            for (int i = 0; i < iterations; i++)
            {
                // توليد مقاييس عشوائية
                if (random.NextDouble() < 0.1) // 10% احتمال استثناء
                {
                    _metricsService.IncrementExceptionCounter("TestException", "GET", "/api/test/load-test");
                }

                _metricsService.IncrementBusinessMetric("load_test", random.NextDouble() < 0.9 ? "success" : "error");
                _metricsService.RecordDatabaseOperation("SELECT", "load_test", random.NextDouble() < 0.95 ? "success" : "error");

                await Task.Delay(10); // تأخير صغير بين العمليات
            }

            return Ok(new { message = $"تم توليد {iterations} عملية اختبار" });
        }
    }
}
