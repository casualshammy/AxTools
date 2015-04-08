"C:\Program Files\Red Gate\SmartAssembly 6\SmartAssembly.com" /build "F:\Dropbox\Projects\AxTools\AxTools\bin\Release\AxTools_CSharp.saproj"
"C:\Program Files\7-Zip\7z.exe" u "F:\Dropbox\Projects\AxTools\AxTools\bin\Release\distr\__update.zip" "F:\Dropbox\Projects\AxTools\AxTools\bin\Release\distr\AxTools.exe"
"F:\Dropbox\Projects\AxTools\AxTools\bin\Release\distr\__update.json"
pause
"F:\programs\WinSCP\WinSCP.exe" /console /script="F:\Dropbox\Projects\AxTools\AxTools\bin\Release\_axtoolsWinSCP.txt"