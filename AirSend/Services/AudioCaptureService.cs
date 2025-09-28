
using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace AirSend.Services
{
    public class AudioCaptureService : IDisposable
    {
        private WasapiLoopbackCapture? _capture;
        private BufferedWaveProvider? _buffer;
        private bool _running;
        private readonly IAudioSender _sender;

        public AudioCaptureService(IAudioSender sender)
        {
            _sender = sender;
        }

        public void Start()
        {
            if (_running) return;
            _capture = new WasapiLoopbackCapture();
            _buffer = new BufferedWaveProvider(_capture.WaveFormat) { DiscardOnBufferOverflow = true };
            _capture.DataAvailable += (s, e) =>
            {
                _buffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
                var frame = new byte[4096];
                int read;
                while ((read = _buffer.Read(frame, 0, frame.Length)) > 0)
                {
                    _sender?.SendAudio(frame, 0, read, _capture.WaveFormat);
                }
            };
            _capture.StartRecording();
            _running = true;
        }

        public void Stop()
        {
            if (!_running) return;
            _capture!.StopRecording();
            _capture.Dispose();
            _capture = null;
            _running = false;
        }

        public void Dispose() => Stop();
    }
}
