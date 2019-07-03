cd /d "F:\sync\private\Projects\AxTools\AxTools_x64"
rem use only absolute paths in next line, it's bugged
"C:\apps\ConfuserEx\Confuser.CLI.exe" "F:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\AxTools.exe" -o "F:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\distr2"
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\signtool.exe" sign /f "C:\Users\Axioma\Documents\certificates\Alex Sommen.pfx" /p 125521 /t http://timestamp.comodoca.com/authenticode "bin\x64\Release\distr2\AxTools.exe"
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\signtool.exe" sign /f "C:\Users\Axioma\Documents\certificates\Alex Sommen.pfx" /as /fd sha256 /p 125521 /tr http://timestamp.comodoca.com/rfc3161 /td sha256 "bin\x64\Release\distr2\AxTools.exe"
del "bin\x64\Release\distr2\*.dll"
copy /b "bin\x64\Release\*.dll" "bin\x64\Release\distr2\"
del "..\_webservice\upd\update-package2.zip"
rem use only absolute paths in next line in source directory path
"C:\Program Files\7-Zip\7z.exe" a -tzip "..\_webservice\upd\update-package2.zip" -p3aTaTre6agA$-E+e "F:\sync\private\Projects\AxTools\AxTools_x64\bin\x64\Release\distr2\*" -r -mx9
_makeRelease2_update-file-version.py
py "F:\sync\private\Projects\AxTools\AxTools_x64\_makeRelease2_plugins.py"
"C:\apps\WinSCP\WinSCP.com" /command ^
	"option batch abort" ^
	"option confirm off" ^
	"open sftp://nginx@axio.name:52852 -hostkey=""ssh-rsa 2048 ca:35:fb:e6:d0:c8:6b:e4:ed:2f:42:2f:20:5c:4e:5b"" -privatekey=""C:\\apps\\Proxifier PE\\nginx@axio.name.ppk""" ^
	"synchronize remote -nopermissions -transfer=binary ""..\_webservice"" ""/home/nginx/html/axio.name/axtools""" ^
	"exit"
pause
