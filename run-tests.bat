@echo off
echo Building project for tests...

:: Set paths
set DOTNET_PATH=dotnet
set PROJECT_FILE=mission.csproj

:: Build the project
%DOTNET_PATH% build %PROJECT_FILE% -c Debug

if %ERRORLEVEL% NEQ 0 (
    echo Build failed with error code %ERRORLEVEL%
    pause
    exit /b %ERRORLEVEL%
)

echo Build successful!
echo.
echo Running tests...

:: Run the application with test flag
%DOTNET_PATH% run --project %PROJECT_FILE% -- --test

pause