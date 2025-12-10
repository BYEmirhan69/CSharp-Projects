# Haar Cascade Dosyasını İndirme Scripti
# PowerShell ile çalıştırın: .\download_cascade.ps1

$cascadeUrl = "https://raw.githubusercontent.com/opencv/opencv/4.x/data/haarcascades/haarcascade_frontalface_default.xml"
$targetDir = Join-Path $PSScriptRoot "Assets\haarcascades"
$targetFile = Join-Path $targetDir "haarcascade_frontalface_default.xml"

# Klasörü oluştur
if (-not (Test-Path $targetDir)) {
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    Write-Host "Klasör oluşturuldu: $targetDir" -ForegroundColor Green
}

# Dosyayı indir
Write-Host "Haar cascade dosyası indiriliyor..." -ForegroundColor Yellow
try {
    Invoke-WebRequest -Uri $cascadeUrl -OutFile $targetFile -UseBasicParsing
    Write-Host "Dosya başarıyla indirildi: $targetFile" -ForegroundColor Green
    Write-Host "Dosya boyutu: $((Get-Item $targetFile).Length) byte" -ForegroundColor Cyan
}
catch {
    Write-Host "HATA: Dosya indirilemedi: $_" -ForegroundColor Red
    Write-Host "Manuel indirme için: $cascadeUrl" -ForegroundColor Yellow
    exit 1
}

Write-Host "`nTamamlandı! Artık uygulamayı çalıştırabilirsiniz." -ForegroundColor Green

