# Simple Alert Test Script
Write-Host "🚀 Testing Alerts..." -ForegroundColor Green

$currentTime = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
$alertmanagerUrl = "http://localhost:9093/api/v2/alerts"

# Test 1: Critical Alert
Write-Host "`n🚨 Sending Critical Alert..." -ForegroundColor Red
$criticalAlert = '[{"labels":{"alertname":"TestCritical","severity":"critical","job":"test"},"annotations":{"summary":"Critical test alert","description":"Testing critical notifications"},"startsAt":"' + $currentTime + '"}]'

try {
    Invoke-RestMethod -Uri $alertmanagerUrl -Method POST -Body $criticalAlert -ContentType "application/json"
    Write-Host "✅ Critical alert sent!" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 2

# Test 2: Warning Alert  
Write-Host "`n⚠️ Sending Warning Alert..." -ForegroundColor Yellow
$warningAlert = '[{"labels":{"alertname":"TestWarning","severity":"warning","job":"web-service"},"annotations":{"summary":"Warning test alert","description":"Testing warning notifications"},"startsAt":"' + $currentTime + '"}]'

try {
    Invoke-RestMethod -Uri $alertmanagerUrl -Method POST -Body $warningAlert -ContentType "application/json"
    Write-Host "✅ Warning alert sent!" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 2

# Test 3: HTTP Monitoring Alert
Write-Host "`n🌐 Sending HTTP Monitoring Alert..." -ForegroundColor Cyan
$httpAlert = '[{"labels":{"alertname":"BlackboxProbeHttpFailure","severity":"warning","job":"blackbox-http","instance":"https://example.com"},"annotations":{"summary":"HTTP probe failed","description":"Endpoint is down"},"startsAt":"' + $currentTime + '"}]'

try {
    Invoke-RestMethod -Uri $alertmanagerUrl -Method POST -Body $httpAlert -ContentType "application/json"
    Write-Host "✅ HTTP alert sent!" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 2

# Test 4: .NET App Alert
Write-Host "`n🔧 Sending .NET App Alert..." -ForegroundColor Magenta
$dotnetAlert = '[{"labels":{"alertname":"DotNetAppDown","severity":"critical","job":"dotnet8-app","instance":"localhost:5000"},"annotations":{"summary":".NET app is down","description":"Application health check failed"},"startsAt":"' + $currentTime + '"}]'

try {
    Invoke-RestMethod -Uri $alertmanagerUrl -Method POST -Body $dotnetAlert -ContentType "application/json"
    Write-Host "✅ .NET alert sent!" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 All alerts sent! Check:" -ForegroundColor Green
Write-Host "  • Slack channels" -ForegroundColor Gray
Write-Host "  • Email inbox" -ForegroundColor Gray  
Write-Host "  • Webhook.site" -ForegroundColor Gray
Write-Host "  • http://localhost:9093 (Alertmanager)" -ForegroundColor Gray

# Open Alertmanager
Start-Process "http://localhost:9093"
