@echo off
reg delete "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v HTTP_PROXY /f
reg delete "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v HTTPS_PROXY /f
pause