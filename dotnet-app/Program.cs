using dotnet_app.Middleware;
using dotnet_app.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// إضافة الخدمات
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// إضافة خدمة المقاييس
builder.Services.AddSingleton<IMetricsService, MetricsService>();

// إضافة فحوصات الصحة
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("التطبيق يعمل بشكل صحيح"))
    .AddCheck("database", () =>
    {
        try
        {
            // هنا يمكن إضافة فحص قاعدة البيانات
            // مثال: using var connection = new SqlConnection(connectionString);
            // connection.Open();
            return HealthCheckResult.Healthy("قاعدة البيانات متاحة");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"خطأ في الاتصال بقاعدة البيانات: {ex.Message}");
        }
    })
    .AddCheck("external_api", () =>
    {
        try
        {
            // فحص الخدمات الخارجية
            return HealthCheckResult.Healthy("الخدمات الخارجية متاحة");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"خطأ في الاتصال بالخدمات الخارجية: {ex.Message}");
        }
    });

// إضافة خدمة الخلفية لجمع المقاييس
builder.Services.AddHostedService<MetricsBackgroundService>();

// إعداد CORS للسماح لـ Prometheus
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPrometheus",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// إعداد pipeline التطبيق
// تفعيل Swagger في جميع البيئات
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Monitoring API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowPrometheus");
app.UseRouting();

// إضافة middleware لجمع المقاييس
app.UseMiddleware<ExceptionMetricsMiddleware>();

// إضافة مقاييس HTTP
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

// نقطة نهاية الصحة
app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// نقطة نهاية Prometheus
app.MapMetrics();

// نقطة نهاية الترحيب
app.MapGet("/", () => "مرحباً! التطبيق يعمل. اذهب إلى /metrics أو /health");

app.Run();