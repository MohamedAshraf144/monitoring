# Quick Alert Test Script
Write-Host "🚀 Testing Alertmanager..." -ForegroundColor Green

$alertBody = @'
[{
  "labels": {
    "alertname": "TestAlert",
    "severity": "critical",
    "instance": "test"
  },
  "annotations": {
    "summary": "Test alert from PowerShell",
    "description": "This is a test alert to verify the system works"
  },
  "startsAt": "2025-01-30T12:35:24.000Z"
}]
'@

try {
    $response = Invoke-RestMethod -Uri "http://localhost:9093/api/v1/alerts" -Method POST -ContentType "application/json" -Body $alertBody
    Write-Host "✅ Alert sent successfully!" -ForegroundColor Green
    Write-Host "Response: $response" -ForegroundColor Gray
} catch {
    Write-Host "❌ Error sending alert: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n📧 Check these channels for alerts:" -ForegroundColor Yellow
Write-Host "  • Slack #general channel" -ForegroundColor Gray
Write-Host "  • Email inbox (medoata44@gmail.com)" -ForegroundColor Gray
Write-Host "  • Webhook.site" -ForegroundColor Gray
Write-Host "  • Microsoft Teams" -ForegroundColor Gray

# Open Alertmanager UI
Write-Host "`n🌐 Opening Alertmanager UI..." -ForegroundColor Cyan
Start-Process "http://localhost:9093"
