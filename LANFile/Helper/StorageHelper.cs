using System;
using System.IO;
using System.Runtime.InteropServices;
using Android.App;

namespace LANFile.Helper;

public class StorageHelper
{
    public static string AndroidFullPathData()
    {
        // return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        //return Android.App.Application.Context.DataDir.Path;
        return Application.Context.GetExternalFilesDir(null).ToString();
        // return Android.OS.Environment.ExternalStorageDirectory.Path;
    }

    public static string WinFullPathData()
    {
        return Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
    }

    public static void WriteToFile(string fileName, string content)
    {
        var _path = string.Empty;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            _path = AndroidFullPathData();
        else
            _path = WinFullPathData();


        if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

        _path = Path.Combine(_path, fileName);
        File.WriteAllText(_path, content);

        Console.WriteLine(_path);
    }

    public static string ReadFile(string fileName)
    {
        var _path = string.Empty;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            _path = AndroidFullPathData();
        else
            _path = WinFullPathData();
        _path = Path.Combine(_path, fileName);
        if (File.Exists(_path)) return File.ReadAllText(_path);
        return string.Empty;
    }
}