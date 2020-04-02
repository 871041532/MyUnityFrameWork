using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

public class ConsoleScript : MonoBehaviour
{
    private InputField m_Filed;
    private Queue m_Queue;
    // Start is called before the first frame update
    void Start()
    {
        m_Filed = GetComponent<InputField>();
        m_Queue = Queue.Synchronized(new Queue());
        Console.Init(m_Filed, m_Queue);
    }

    private void Update()
    {
            Console.Update();
    }

    private void OnDestroy()
    {
        Console.OnDestroy();
    }
    
    static class Console
 {
     static bool is_OpenDebug = true;
     static string m_UDPServerURL = "127.0.0.1";
     static string m_UDPServerPort = "8633";
 
     public static UdpClient m_UDPClient = null;
     static IPEndPoint m_EndPoint = null;
     static Process m_ChildProcess;
     static UnityEngine.LogType m_LastLogType;
     static UnityEngine.Application.LogCallback m_LogCallback;
     private static Queue m_Queue;
     private static InputField m_Field;
     private static Thread m_Thread;
 
     #region 外部接口
     public static void Init(InputField field, Queue queue)
     {
         m_Queue = queue;
         m_Field = field;
         if (!is_OpenDebug)
         {
             return;
         }
         SendUDP("Unity已连接服务器。");
//         if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
//         {
//             RunPythonConsole();
//         }
         m_LogCallback = ( condition,  stackTrace,  type) => {
            // 只打印error
            if (type != UnityEngine.LogType.Error)
            {
                return;
            }
            SendUDP("#" + (int)type);
             if (m_LastLogType != type)
             {
                 m_LastLogType = type;
                 SendUDP("#" + (int)type);
             }
             SendUDP(condition);
             if (type != UnityEngine.LogType.Log)
             {
//                 SendUDP(stackTrace);
             }
         };
//         UnityEngine.Application.logMessageReceived += m_LogCallback;
     }
 
     public static void OnDestroy()
     {
         SendUDP("Unity已断开服务器。");
         if (m_UDPClient != null)
         {
             m_UDPClient.Close();
             m_UDPClient = null;
         }
         if (m_ChildProcess != null)
         {
             try
             {
                 m_ChildProcess.Kill();
             }
             catch (Exception)
             {
             }
             m_ChildProcess = null;
         }
//         if (is_OpenDebug)
//         {
//             Application.logMessageReceived -= m_LogCallback;
//         }
     }
     #endregion
 
     #region 内部实现
     static void RunPythonConsole()
     {
         string cmd = string.Format("D:\\Console\\ConsoleRun.cmd");
         m_ChildProcess = new System.Diagnostics.Process();
         m_ChildProcess.StartInfo.FileName = cmd;
         m_ChildProcess.StartInfo.Arguments = m_UDPServerPort;
         m_ChildProcess.Start();
     }
 
     public static void SendUDP(string sendString)
     {
         if (m_UDPClient == null)
         {
             IPAddress remoteIP = IPAddress.Parse(m_UDPServerURL); //假设发送给这个IP
             m_EndPoint = new IPEndPoint(remoteIP, Convert.ToInt32(m_UDPServerPort));//实例化一个远程端点 
             m_UDPClient = new UdpClient(10001);
             ThreadReceive();
         }
         byte[] sendData = Encoding.UTF8.GetBytes(sendString);
         m_UDPClient.Send(sendData, sendData.Length, m_EndPoint);//将数据发送到远程端点 
     }

     public static void Update()
     {
         if (m_Queue.Count > 0)
         {
             for (int i = 0; i < m_Queue.Count; i++)
             {
                 string strs = (string) m_Queue.Dequeue();
                 PraseReceive(strs);
             }
         }
     }

     static IPEndPoint m_ReceiveIp = new IPEndPoint(IPAddress.Parse(m_UDPServerURL), Convert.ToInt32(m_UDPServerPort));
     static void ThreadReceive()
     {
        m_UDPClient.BeginReceive(AsyncReceive, m_ReceiveIp);
     }

     static void AsyncReceive(IAsyncResult ar)
     {
         if (ar.AsyncState != null)
         {
             
             var receiveBytes = m_UDPClient.EndReceive(ar, ref m_ReceiveIp);
             var str = Encoding.UTF8.GetString(receiveBytes);
             m_Queue.Enqueue(str);
             m_UDPClient.BeginReceive(AsyncReceive, m_ReceiveIp);
         }
     }

     static void PraseReceive(string strs)
     {
         m_Field.text = strs;
         m_Field.onEndEdit.Invoke(strs);
     }
     #endregion
 }
}
