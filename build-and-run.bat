@echo off
echo Building project...

:: Set paths
set DOTNET_PATH=dotnet
set PROJECT_FILE=mission.csproj
set OUTPUT_DIR=bin\Debug\net8.0-windows

:: Create output directory if it doesn't exist
if not exist %OUTPUT_DIR% mkdir %OUTPUT_DIR%

:: Build the project
%DOTNET_PATH% build %PROJECT_FILE% -c Debug

if %ERRORLEVEL% NEQ 0 (
    echo Build failed with error code %ERRORLEVEL%
    pause
    exit /b %ERRORLEVEL%
)

echo Build successful!
echo.
echo Running application...

:: Run the application
%DOTNET_PATH% run --project %PROJECT_FILE%

pause