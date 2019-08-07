using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using Microsoft.Win32.SafeHandles;
public static class Common
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AllocConsole();
    private const int STD_OUTPUT_HANDLE = -11;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FreeConsole();
    [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    static extern IntPtr GetStdHandle(int nStdHandle);
    static TextWriter oldOutput;

    internal static void StartWriteline()
    {
        AllocConsole();
        oldOutput = Console.Out;
        try
        {
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            System.Text.Encoding encoding = System.Text.Encoding.Unicode;
            StreamWriter standardOutput = new StreamWriter(fileStream);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            //Console.OutputEncoding = System.Text.Encoding.Unicode;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Couldn't redirect output: " + ex.Message);
        }
    }

    internal static void CLoseWriteline()
    {
        Console.SetOut(oldOutput);
        FreeConsole();
    }
}