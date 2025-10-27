@echo off
REM This batch file cleans and rebuilds the solution
REM Close IIS Express in Visual Studio first!

cd /d "%~dp0"

echo Cleaning solution...
timeout /t 2 /nobreak

REM Delete bin and obj folders
if exist "FwkWebAppForms1\bin" (
    echo Deleting bin folder...
    rmdir /s /q "FwkWebAppForms1\bin"
)

if exist "FwkWebAppForms1\obj" (
    echo Deleting obj folder...
    rmdir /s /q "FwkWebAppForms1\obj"
)

echo.
echo Done! Now:
echo 1. Close the IIS Express server in Visual Studio
echo 2. Close Visual Studio completely
echo 3. Reopen the solution in Visual Studio
echo 4. Clean Solution (Build menu)
echo 5. Rebuild Solution (Build menu)
echo 6. Run the application (F5)
pause
