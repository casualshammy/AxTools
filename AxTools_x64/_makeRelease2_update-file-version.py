from win32api import GetFileVersionInfo, LOWORD, HIWORD
import os

def get_version_number(filename):
    try:
        info = GetFileVersionInfo (filename, "\\")
        ms = info['FileVersionMS']
        ls = info['FileVersionLS']
        return HIWORD (ms), LOWORD (ms), HIWORD (ls), LOWORD (ls)
    except:
        return 0,0,0,0

filename = "D:\\sync\\private\\Projects\\AxTools\\AxTools_x64\\bin\\x64\\Release\\distr2\\AxTools.exe"
major, minor, build, revision = get_version_number(filename)
with open("D:\\sync\\private\\Projects\\AxTools\\_webservice\\upd\\update2.json", "w") as file:
    file.write("{{\"Version\":{{\"Major\":{0},\"Minor\":{1},\"Build\":{2}}}}}".format(major, minor, build))

