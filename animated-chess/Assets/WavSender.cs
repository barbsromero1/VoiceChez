using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class WavSender : MonoBehaviour
{
    public string host = "localhost";
    public int port = 12345;
    public string keyword = "ajedrez";
    public int padding = 25;

    public UnityEvent<string> onResponse;

    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;

    private void OnEnable() {
        client = new TcpClient();
        client.Connect(host, port);
        stream = client.GetStream();
        reader = new StreamReader(stream, Encoding.UTF8);
        SendVerification();
    }

    private void SendVerification(){
        byte[] usernameBytes = Encoding.UTF8.GetBytes(keyword);
        stream.Write(usernameBytes, 0, usernameBytes.Length);
    }

    private void SendMetadata(char user, int size){
        string data = $"{user},{size}";
        data = data.PadRight(padding);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        stream.Write(dataBytes, 0, dataBytes.Length);
    }

    public void SendAudio(string filename){
        byte[] wav = ReadWav(filename);
        int wavSize = wav.Length;

        SendMetadata(ChessMgr.instance.GetCurrentPlayer(), wavSize); //TODO: leer jugador actual

        // Send wav data
        stream.Write(wav, 0, wav.Length);
        File.Delete(filename);

        //Read response
        string response = reader.ReadLine();
        Debug.Log("Server response: " + response);
        onResponse.Invoke(response);

    }

    private byte[] ReadWav(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    private void OnDisable(){
        stream.Close();
        client.Close();
    }
}