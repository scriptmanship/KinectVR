using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Security;



public class Alice{
	private static Alice instance;
    public static Alice Instance
    {
        get
        {
            if (instance == null) instance = new Alice();
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    private Action<string> stringReceive;
    private Action<byte[]> byteReceive;
    private bool connected = false;
    private TcpClient client;

    /// <summary>
    /// Connects to the server
    /// Remember to call Destroy() at the end of application 
    /// <param name="prefetchSocketPolicy">
    /// Use when using web player and socketpolicy server</param>
    /// </summary>
	public bool Connect(string server, Int32 port, bool prefetchSocketPolicy = true) 
	{
        if (connected) return false;
      if(prefetchSocketPolicy)Security.PrefetchSocketPolicy(server, 843);
	  try 
	  {
	    client = new TcpClient(server, port);
        StartReceive();
        connected = true;
        return true;
	  } 
	  catch
	  {
        return false;
	  }
	}
    public void Send(string message)
    {
        // Translate the passed message into ASCII and store it as a Byte array.
        Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
        Send(data);
    }
    public void Send(byte[] data)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }
        catch
        {
            throw new Exception("ALICE:: Client is not connected");
        }
    }
    public void SendEvent(byte eventId, string data)
    {
        Send((char)eventId + data);
    }
    public void SendEvent(byte eventId, byte[] data)
    {
        byte[] dat = new byte[data.Length + 1];
        dat[0] = eventId;
        Buffer.BlockCopy(data, 0, dat, 1, data.Length);
        Send(dat);
    }
    private void StartReceive()
    {
        NetworkStream stream = client.GetStream();
        Byte[] data;
        String responseData;
        Int32 bytes;
        Loom.RunAsync(() =>
        {
            while (true)
            {
                data = new Byte[4096];
                responseData = String.Empty;
                bytes = stream.Read(data, 0, data.Length);
                if (byteReceive != null) byteReceive(data);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                if (stringReceive != null) stringReceive(responseData);
            }
        });
    }
    public void AssignReceive(Action<string> receiveFunction)
    {
        stringReceive = receiveFunction;
    }
    public void AssignReceive(Action<byte[]> receiveFunction)
    {
        byteReceive = receiveFunction;
    }
    public void Disconnect()
    {
        client.GetStream().Close();
        client.Close();
        connected = false;
    }
    public void Destroy()
    {
        Debug.Log("Alice destroyed");
        using (this.client)
        {
            if (client != null)
            {
                Disconnect();
            }
        }
        client = null;
        
    }
}
