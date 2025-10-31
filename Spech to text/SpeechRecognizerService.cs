using System;
using System.Globalization;
using System.Linq;
using NAudio.Wave;
using SpeechToText.Streaming;
using System.Speech.Recognition;

namespace SpeechToText
{
    /// <summary>
    /// SpeechRecognizerService, System.Speech.Recognition API'sini kullanarak
    /// mikrofon girişinden Türkçe (tr-TR) dikte tanıma yapan, başlat/durdur
    /// işlemlerini yöneten ve olaylar yayınlayan servis sınıfıdır.
    /// </summary>
    public sealed class SpeechRecognizerService : IDisposable
    {
        private SpeechRecognitionEngine _engine;
        private bool _isInitialized;
        private bool _isDisposed;
        private string _preferredRecognizerId; // Kullanıcı seçimiyle öncelikli tanıyıcı
        private int? _preferredMicrophoneIndex; // NAudio WaveIn cihaz indexi
        private WaveInEvent _waveIn;
        private BufferedAudioStream _audioStream;

        /// <summary>
        /// Tanınan metin her güncellendiğinde tetiklenir.
        /// </summary>
        public event EventHandler<string> TextRecognized;

        /// <summary>
        /// Hata oluştuğunda tetiklenir.
        /// </summary>
        public event EventHandler<Exception> RecognitionError;

        /// <summary>
        /// Servisin durum değişimlerini (Başladı/Durdu) bildirmek için tetiklenir.
        /// </summary>
        public event EventHandler<bool> ListeningStateChanged;

        /// <summary>
        /// Servisin dinleme durumunu belirtir.
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        /// Belirli bir tanıyıcıyı (RecognizerInfo.Id) tercih olarak ayarla. Bir sonraki başlatmada kullanılacaktır.
        /// </summary>
        public void SetPreferredRecognizer(string recognizerId)
        {
            _preferredRecognizerId = recognizerId;
        }

        /// <summary>
        /// Tercih edilen mikrofonu WaveIn indexine göre ayarlar (NAudio). Null ise varsayılan ses girişi kullanılır.
        /// </summary>
        public void SetPreferredMicrophone(int? waveInDeviceIndex)
        {
            _preferredMicrophoneIndex = waveInDeviceIndex;
        }

        /// <summary>
        /// Türkçe (tr-TR) için tanıyıcıyı hazırlar.
        /// </summary>
        private void EnsureInitialized()
        {
            if (_isInitialized) return;

            try
            {
                // Yüklü tanıyıcılar arasından önce tr-TR'yi ara, yoksa ilk uygun olanı kullan.
                var installed = SpeechRecognitionEngine.InstalledRecognizers();
                if (installed == null || installed.Count == 0)
                {
                    throw new InvalidOperationException("Sistemde herhangi bir konuşma tanıyıcısı (SAPI) yüklü değil. Lütfen Windows Konuşma dil paketini kurun.");
                }

                // Öncelik: Kullanıcı seçimi → tr-TR → listedeki ilk tanıyıcı
                RecognizerInfo selected = null;

                if (!string.IsNullOrWhiteSpace(_preferredRecognizerId))
                {
                    selected = installed.Cast<RecognizerInfo>()
                        .FirstOrDefault(r => string.Equals(r.Id, _preferredRecognizerId, StringComparison.OrdinalIgnoreCase));
                }

                var turkish = installed
                    .Cast<RecognizerInfo>()
                    .FirstOrDefault(r => string.Equals(r.Culture.Name, "tr-TR", StringComparison.OrdinalIgnoreCase));

                selected = selected ?? turkish ?? installed.Cast<RecognizerInfo>().First();

                _engine = new SpeechRecognitionEngine(selected);

                // Dikte için genel gramer yükle.
                _engine.LoadGrammar(new DictationGrammar());

                // Ses girişini ayarla: cihaz seçildiyse NAudio ile özel akış kullan, yoksa varsayılan cihaz.
                if (_preferredMicrophoneIndex.HasValue)
                {
                    _audioStream = new BufferedAudioStream();
                    var format = new WaveFormat(16000, 16, 1);
                    _waveIn = new WaveInEvent
                    {
                        DeviceNumber = _preferredMicrophoneIndex.Value,
                        WaveFormat = format,
                        BufferMilliseconds = 50
                    };
                    _waveIn.DataAvailable += (s, a) => _audioStream.Write(a.Buffer, 0, a.BytesRecorded);
                    _waveIn.RecordingStopped += (s, a) => _audioStream.CompleteWriting();

                    var speechFormat = new System.Speech.AudioFormat.SpeechAudioFormatInfo(
                        16000,
                        System.Speech.AudioFormat.AudioBitsPerSample.Sixteen,
                        System.Speech.AudioFormat.AudioChannel.Mono);

                    _engine.SetInputToAudioStream(_audioStream, speechFormat);
                }
                else
                {
                    _engine.SetInputToDefaultAudioDevice();
                }

                // Olay abonelikleri.
                _engine.SpeechRecognized += OnSpeechRecognized;
                _engine.RecognizeCompleted += OnRecognizeCompleted;

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                RecognitionError?.Invoke(this, ex);
                throw;
            }
        }

        /// <summary>
        /// Dinlemeyi başlatır. Birden fazla cümle için sürekli tanıma modunu kullanır.
        /// </summary>
        public void Start()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(SpeechRecognizerService));
            EnsureInitialized();
            if (IsListening) return;

            try
            {
                // Kaydı başlat (NAudio kullanılıyorsa)
                if (_waveIn != null)
                {
                    _waveIn.StartRecording();
                }

                _engine.RecognizeAsync(RecognizeMode.Multiple);
                IsListening = true;
                ListeningStateChanged?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                RecognitionError?.Invoke(this, ex);
                throw;
            }
        }

        /// <summary>
        /// Dinlemeyi durdurur.
        /// </summary>
        public void Stop()
        {
            if (_isDisposed) return;
            if (!_isInitialized) return;
            if (!IsListening) return;

            try
            {
                _engine.RecognizeAsyncStop();
                if (_waveIn != null)
                {
                    try { _waveIn.StopRecording(); } catch { }
                }
            }
            catch (Exception ex)
            {
                RecognitionError?.Invoke(this, ex);
                throw;
            }
        }

        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Güven puanına basit bir eşik uygula (isteğe bağlı).
            if (e.Result != null && e.Result.Confidence >= 0.5)
            {
                TextRecognized?.Invoke(this, e.Result.Text);
            }
        }

        private void OnRecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            IsListening = false;
            ListeningStateChanged?.Invoke(this, false);

            if (e.Error != null)
            {
                RecognitionError?.Invoke(this, e.Error);
            }
        }

        /// <summary>
        /// Kaynakları serbest bırakır.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            try
            {
                if (_engine != null)
                {
                    if (IsListening)
                    {
                        // Durdurmayı dene; hata olsa bile dispose edilmeli.
                        try { _engine.RecognizeAsyncCancel(); } catch { }
                    }

                    _engine.SpeechRecognized -= OnSpeechRecognized;
                    _engine.RecognizeCompleted -= OnRecognizeCompleted;
                    _engine.Dispose();
                }

                if (_waveIn != null)
                {
                    _waveIn.Dispose();
                    _waveIn = null;
                }

                if (_audioStream != null)
                {
                    _audioStream.Dispose();
                    _audioStream = null;
                }
            }
            finally
            {
                _isDisposed = true;
                _isInitialized = false;
                IsListening = false;
            }
        }
    }
}


