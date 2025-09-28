
using NAudio.Wave;

namespace AirSend.Services
{
    public interface IAudioSender
    {
        void ConnectToDevice(string deviceId, string ip, int port, string serviceType);
        void Disconnect();
        void SendAudio(byte[] buffer, int offset, int count, WaveFormat format);
        void SetVolume(float vol);
    }
}
