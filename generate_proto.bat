@echo off
REM Batch file to generate C# and C++ classes from proto files

echo ========================================
echo Generating Protocol Buffer Classes
echo ========================================
echo.

REM Check if protoc is available
where protoc >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: protoc is not installed or not in PATH
    echo.
    echo Please install Protocol Buffers compiler:
    echo 1. Download from: https://github.com/protocolbuffers/protobuf/releases
    echo 2. Or install via package manager:
    echo    - Windows: choco install protoc
    echo    - Windows: scoop install protobuf
    echo 3. Add protoc to your system PATH
    echo.
    pause
    exit /b 1
)

echo Found protoc:
protoc --version
echo.

REM Set paths
set PROTO_DIR=proto
set CSHARP_OUTPUT_DIR=GUI\Comm
set CPP_OUTPUT_DIR=Simulator\comm

REM Check if proto directory exists
if not exist "%PROTO_DIR%" (
    echo ERROR: Proto directory not found: %PROTO_DIR%
    pause
    exit /b 1
)

REM Create output directories if they don't exist
if not exist "%CSHARP_OUTPUT_DIR%" (
    echo Creating directory: %CSHARP_OUTPUT_DIR%
    mkdir "%CSHARP_OUTPUT_DIR%"
)

if not exist "%CPP_OUTPUT_DIR%" (
    echo Creating directory: %CPP_OUTPUT_DIR%
    mkdir "%CPP_OUTPUT_DIR%"
)

echo.
echo Generating C# classes...
echo Output directory: %CSHARP_OUTPUT_DIR%
protoc --proto_path=%PROTO_DIR% --csharp_out=%CSHARP_OUTPUT_DIR% %PROTO_DIR%\*.proto
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to generate C# classes
    pause
    exit /b 1
)
echo C# classes generated successfully!

echo.
echo Generating C++ classes...
echo Output directory: %CPP_OUTPUT_DIR%
protoc --proto_path=%PROTO_DIR% --cpp_out=%CPP_OUTPUT_DIR% %PROTO_DIR%\*.proto
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to generate C++ classes
    pause
    exit /b 1
)
echo C++ classes generated successfully!

echo.
echo ========================================
echo Generation Complete!
echo ========================================
echo C# classes: %CSHARP_OUTPUT_DIR%
echo C++ classes: %CPP_OUTPUT_DIR%
echo ========================================
echo.

REM List generated files
echo Generated C# files:
dir /b "%CSHARP_OUTPUT_DIR%\*.cs" 2>nul
echo.
echo Generated C++ files:
dir /b "%CPP_OUTPUT_DIR%\*.pb.h" 2>nul
dir /b "%CPP_OUTPUT_DIR%\*.pb.cc" 2>nul
echo.

pause
