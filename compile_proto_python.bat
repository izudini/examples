@echo off
REM Batch file to generate Python classes from proto files

echo ========================================
echo Generating Python Protocol Buffer Classes
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
set PYTHON_OUTPUT_DIR=pyProto

REM Check if proto directory exists
if not exist "%PROTO_DIR%" (
    echo ERROR: Proto directory not found: %PROTO_DIR%
    pause
    exit /b 1
)

REM Create output directory if it doesn't exist
if not exist "%PYTHON_OUTPUT_DIR%" (
    echo Creating directory: %PYTHON_OUTPUT_DIR%
    mkdir "%PYTHON_OUTPUT_DIR%"
)

echo.
echo Generating Python classes...
echo Output directory: %PYTHON_OUTPUT_DIR%
protoc --proto_path=%PROTO_DIR% --python_out=%PYTHON_OUTPUT_DIR% %PROTO_DIR%\*.proto
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to generate Python classes
    pause
    exit /b 1
)
echo Python classes generated successfully!

echo.
echo ========================================
echo Generation Complete!
echo ========================================
echo Python classes: %PYTHON_OUTPUT_DIR%
echo ========================================
echo.

REM List generated files
echo Generated Python files:
dir /b "%PYTHON_OUTPUT_DIR%\*.py" 2>nul

echo.
pause
