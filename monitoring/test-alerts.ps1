# 🧪 Alert Testing Script
# اختبار جميع أنواع التنبيهات في نظام المراقبة

Write-Host "🚀 Starting Alert Testing..." -ForegroundColor Green

# Get current time in UTC
$currentTime = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
$alertmanagerUrl = "http://localhost:9093/api/v2/alerts"

# Function to send alert
function Send-Alert {
    param(
        [string]$AlertData,
        [string]$Description
    )
    
    Write-Host "`n📤 Sending: $Description" -ForegroundColor Yellow
    try {
        $response = Invoke-RestMethod -Uri $alertmanagerUrl -Method POST -Body $AlertData -ContentType "application/json"
        Write-Host "✅ Alert sent successfully!" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ Failed to send alert: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# 🚨 Test 1: Critical Alert
Write-Host "`n=== 🚨 Testing Critical Alert ===" -ForegroundColor Red
$criticalAlert = @'
[
  {
    "labels": {
      "alertname": "TestCriticalAlert",
      "severity": "critical",
      "job": "test-service",
      "instance": "test-server:8080"
    },
    "annotations": {
      "summary": "Critical system failure detected",
      "description": "This is a test critical alert that should trigger all notification channels"
    },
    "startsAt": "TIMESTAMP_PLACEHOLDER"
  }
]
'@

$criticalAlert = $criticalAlert.Replace("TIMESTAMP_PLACEHOLDER", $currentTime)
Send-Alert -AlertData $criticalAlert -Description "Critical Alert Test"

# ⚠️ Test 2: Warning Alert
Write-Host "`n=== ⚠️ Testing Warning Alert ===" -ForegroundColor Yellow
$warningAlert = @'
[
  {
    "labels": {
      "alertname": "TestWarningAlert",
      "severity": "warning",
      "job": "web-service",
      "instance": "web-server:80"
    },
    "annotations": {
      "summary": "High memory usage detected",
      "description": "Memory usage is above 80% threshold"
    },
    "startsAt": "TIMESTAMP_PLACEHOLDER"
  }
]
'@

$warningAlert = $warningAlert.Replace("TIMESTAMP_PLACEHOLDER", $currentTime)
Send-Alert -AlertData $warningAlert -Description "Warning Alert Test"

# 🌐 Test 3: HTTP Monitoring Alert
Write-Host "`n=== 🌐 Testing HTTP Monitoring Alert ===" -ForegroundColor Cyan
$httpAlert = @'
[
  {
    "labels": {
      "alertname": "BlackboxProbeHttpFailure",
      "severity": "warning",
      "job": "blackbox-http",
      "instance": "https://example.com/api"
    },
    "annotations": {
      "summary": "HTTP probe failed",
      "description": "HTTP endpoint is returning 500 status code"
    },
    "startsAt": "TIMESTAMP_PLACEHOLDER"
  }
]
'@

$httpAlert = $httpAlert.Replace("TIMESTAMP_PLACEHOLDER", $currentTime)
Send-Alert -AlertData $httpAlert -Description "HTTP Monitoring Alert Test"

# 🔧 Test 4: .NET App Alert
Write-Host "`n=== 🔧 Testing .NET App Alert ===" -ForegroundColor Magenta
$dotnetAlert = @'
[
  {
    "labels": {
      "alertname": "DotNetAppDown",
      "severity": "critical",
      "job": "dotnet8-app",
      "instance": "host.docker.internal:5000"
    },
    "annotations": {
      "summary": ".NET application is down",
      "description": "The .NET application health check is failing"
    },
    "startsAt": "TIMESTAMP_PLACEHOLDER"
  }
]
'@

$dotnetAlert = $dotnetAlert.Replace("TIMESTAMP_PLACEHOLDER", $currentTime)
Send-Alert -AlertData $dotnetAlert -Description ".NET App Alert Test"

# 📊 Test 5: Multiple Alerts (Group Test)
Write-Host "`n=== 📊 Testing Multiple Alerts (Grouping) ===" -ForegroundColor Blue
$multipleAlerts = @'
[
  {
    "labels": {
      "alertname": "HighCPU",
      "severity": "warning",
      "job": "node-exporter",
      "instance": "server1:9100"
    },
    "annotations": {
      "summary": "High CPU usage on server1",
      "description": "CPU usage is above 90%"
    },
    "startsAt": "TIMESTAMP_PLACEHOLDER"
  },
  {
    "labels": {
      "alertname": "HighCPU",
      "severity": "warning",
      "job": "node-exporter", 
      "instance": "server2:9100"
    },
    "annotations": {
      "summary": "High CPU usage on server2",
      "description": "CPU usage is above 90%"
    },
    "startsAt": "TIMESTAMP_PLACEHOLDER"
  }
]
'@

$multipleAlerts = $multipleAlerts.Replace("TIMESTAMP_PLACEHOLDER", $currentTime)
Send-Alert -AlertData $multipleAlerts -Description "Multiple Alerts (Grouping Test)"

# 🔄 Test 6: Resolved Alert
Write-Host "`n=== 🔄 Testing Resolved Alert ===" -ForegroundColor Green
Start-Sleep -Seconds 2  # Wait a bit before sending resolved alert

$resolvedTime = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
$resolvedAlert = @'
[
  {
    "labels": {
      "alertname": "TestResolvedAlert",
      "severity": "warning",
      "job": "test-service",
      "instance": "test-server:8080"
    },
    "annotations": {
      "summary": "Test alert that will be resolved",
      "description": "This alert tests the resolved notification feature"
    },
    "startsAt": "START_TIME_PLACEHOLDER",
    "endsAt": "END_TIME_PLACEHOLDER"
  }
]
'@

$resolvedAlert = $resolvedAlert.Replace("START_TIME_PLACEHOLDER", $currentTime)
$resolvedAlert = $resolvedAlert.Replace("END_TIME_PLACEHOLDER", $resolvedTime)
Send-Alert -AlertData $resolvedAlert -Description "Resolved Alert Test"

# 📋 Summary
Write-Host "`n" + "="*50 -ForegroundColor White
Write-Host "🎉 Alert Testing Complete!" -ForegroundColor Green
Write-Host "📋 Check the following:" -ForegroundColor White
Write-Host "   • Slack channels for notifications" -ForegroundColor Gray
Write-Host "   • Email inbox for alert emails" -ForegroundColor Gray
Write-Host "   • Webhook.site for webhook deliveries" -ForegroundColor Gray
Write-Host "   • Alertmanager UI: http://localhost:9093" -ForegroundColor Gray
Write-Host "   • Prometheus UI: http://localhost:9090" -ForegroundColor Gray
Write-Host "="*50 -ForegroundColor White

# 🌐 Open monitoring dashboards
Write-Host "`n🌐 Opening monitoring dashboards..." -ForegroundColor Cyan
Start-Process "http://localhost:9093"  # Alertmanager
Start-Process "http://localhost:9090"  # Prometheus
Start-Process "http://localhost:3000"  # Grafana
Start-Process "https://webhook.site/1194a021-ff84-42cf-95a8-bb048188cd3c"  # Webhook site

Write-Host "`n🎯 Test completed! Check your notifications." -ForegroundColor Green
