using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AES
{
    private static string AESHead = "AESEncrypt";
    
    // 文件加密，传入文件路径
    public static void EncryptFile(string filePath, string key)
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fs != null)
                {
                    //读取字节头，判断是否已经加密过了
                    byte[] headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    string headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AESHead)
                    {
#if UNITY_EDITOR
                        Debug.Log(filePath + "已经加密过了！");
#endif
                        return;
                    }
                    //加密并且写入字节头
                    fs.Seek(0, SeekOrigin.Begin);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    byte[] headBuffer = Encoding.UTF8.GetBytes(AESHead);
                    fs.Write(headBuffer, 0, headBuffer.Length);
                    byte[] EncBuffer = EncryptBytes(buffer, key);
                    fs.Write(EncBuffer, 0, EncBuffer.Length);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    // 文件解密，传入文件路径（会改动加密文件）
    public static void DecryptFile(string filePath, string key)
    {
        if (!File.Exists(filePath))
        {
            return;
        }
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fs != null)
                {
                    byte[] headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    string headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AESHead)
                    {
                        byte[] buffer = new byte[fs.Length - headBuff.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.SetLength(0);
                        byte[] DecBuffer = DecryptBytes(buffer, key);
                        fs.Write(DecBuffer, 0, DecBuffer.Length);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    // 文件解密，传入文件路径，返回字节
    public static byte[] DecryptFileToBytes(string filePath, string key)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }
        byte[] DecBuffer = null;
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs != null)
                {
                    byte[] headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    string headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AESHead)
                    {
                        byte[] buffer = new byte[fs.Length - headBuff.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                        DecBuffer = DecryptBytes(buffer, key);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return DecBuffer;
    }
    
    // 加密string
    public static string EncryptString(string originalString, string key)
    {
        byte[] strBytes = StringToBytes(originalString);
        byte[] encryptBytes = EncryptBytes(strBytes, key);
        string encryptString = BytesToString(encryptBytes);
        return encryptString;
    }
    
    // string 解密
    public static string DecryptString(string encryptedString, string key)
    {
        byte[] encryptBytes = StringToBytes(encryptedString);
        byte[] decryptBytes = DecryptBytes(encryptBytes, key);
        string decryString = BytesToString(decryptBytes);
        return decryString;
    }
     public static byte[] StringToBytes(string strs)
    {
        byte[] bytes = new byte[strs.Length * 2];
        for (int i = 0; i < strs.Length; i++)
        {
            char c = strs[i];
            bytes[i*2] = (byte) ((c & 0xFF00) >> 8); 
            bytes[i*2 + 1] = (byte) (c & 0xFF);
        }
        return bytes;
    }

     public static string BytesToString(Byte[] bytes)
     {
         char[] strs = new char[bytes.Length / 2];
         for (int i = 0; i < bytes.Length / 2; i++)
         {
             byte b0 = bytes[i * 2];
             byte b1 = bytes[i * 2 + 1];
             strs[i] = (char) (((b0 & 0xFF) << 8) | (b1 & 0xFF)); 
         }
         return new string(strs);
     }
     
     // 加密Byte[]
    public static byte[] EncryptBytes(byte[] originalBytes, string key)
    {
        if (originalBytes.Length == 0) { throw (new Exception("明文不得为空")); }
        if (string.IsNullOrEmpty(key)) { throw (new Exception("密钥不得为空")); }
        byte[] m_strEncrypt;
        byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael m_AESProvider = Rijndael.Create();
        try
        {
            MemoryStream m_stream = new MemoryStream();
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, m_salt);
            ICryptoTransform transform = m_AESProvider.CreateEncryptor(pdb.GetBytes(32), m_btIV);
            CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
            m_csstream.Write(originalBytes, 0, originalBytes.Length);
            m_csstream.FlushFinalBlock();
            m_strEncrypt = m_stream.ToArray();
            m_stream.Close(); m_stream.Dispose();
            m_csstream.Close(); m_csstream.Dispose();
        }
        catch (IOException ex) { throw ex; }
        catch (CryptographicException ex) { throw ex; }
        catch (ArgumentException ex) { throw ex; }
        catch (Exception ex) { throw ex; }
        finally { m_AESProvider.Clear(); }
        return m_strEncrypt;
    }

    // 解密byte[]
    public static byte[] DecryptBytes(byte[] encryptedBytes, string key)
    {
        if (encryptedBytes.Length == 0) { throw (new Exception("密文不得为空")); }
        if (string.IsNullOrEmpty(key)) { throw (new Exception("密钥不得为空")); }
        byte[] m_strDecrypt;
        byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael m_AESProvider = Rijndael.Create();
        try
        {
            MemoryStream m_stream = new MemoryStream();
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, m_salt);
            ICryptoTransform transform = m_AESProvider.CreateDecryptor(pdb.GetBytes(32), m_btIV);
            CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
            m_csstream.Write(encryptedBytes, 0, encryptedBytes.Length);
            m_csstream.FlushFinalBlock();
            m_strDecrypt = m_stream.ToArray();
            m_stream.Close(); m_stream.Dispose();
            m_csstream.Close(); m_csstream.Dispose();
        }
        catch (IOException ex) { throw ex; }
        catch (CryptographicException ex) { throw ex; }
        catch (ArgumentException ex) { throw ex; }
        catch (Exception ex) { throw ex; }
        finally { m_AESProvider.Clear(); }
        return m_strDecrypt;
    }

}

