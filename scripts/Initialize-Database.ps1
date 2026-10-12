param([string]$Server = '(localdb)\MSSQLLocalDB', [string]$DatabaseName = 'HastaneProjeDemo', [switch]$SeedDemo)
$ErrorActionPreference='Stop'
if ($DatabaseName -notmatch '^HastaneProje[A-Za-z0-9_]*$') { throw 'Kurulum yalnızca HastaneProje ile başlayan veritabanları içindir.' }
$projectRoot=[System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$vswhere='C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe'
$buildTool=& $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $buildTool) { throw 'Visual Studio veya Build Tools ile .NET Framework 4.7.2 geliştirme araçlarını yükleyin.' }
& $buildTool (Join-Path $projectRoot 'tests\HastaneChecks.csproj') /p:Configuration=Release /nologo /verbosity:minimal
if ($LASTEXITCODE -ne 0) { throw 'Derleme başarısız.' }
& sqlcmd -S $Server -E -b -Q "IF DB_ID(N'$DatabaseName') IS NULL CREATE DATABASE [$DatabaseName];"
if ($LASTEXITCODE -ne 0) { throw 'Veritabanı oluşturulamadı.' }
& sqlcmd -S $Server -E -d $DatabaseName -b -f 65001 -i (Join-Path $projectRoot 'database\schema.sql')
if ($LASTEXITCODE -ne 0) { throw 'Şema kurulamadı.' }
if ($SeedDemo) {
 $originalConnection=[Environment]::GetEnvironmentVariable('HASTANE_CONNECTION_STRING','Process')
 try {
 $env:HASTANE_CONNECTION_STRING="Data Source=$Server;Initial Catalog=$DatabaseName;Integrated Security=True;TrustServerCertificate=True;"
 & (Join-Path $projectRoot 'tests\bin\Release\HastaneChecks.exe') --seed-demo
 if ($LASTEXITCODE -ne 0) { throw 'Demo veri kurulamadı.' }
 } finally { [Environment]::SetEnvironmentVariable('HASTANE_CONNECTION_STRING',$originalConnection,'Process') }
}
Write-Host "Hazır: $Server / $DatabaseName"
