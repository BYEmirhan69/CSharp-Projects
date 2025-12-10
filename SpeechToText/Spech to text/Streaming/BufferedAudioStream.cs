using System;
using System.IO;
using System.Threading;

namespace SpeechToText.Streaming
{
    /// <summary>
    /// SpeechRecognitionEngine'in SetInputToAudioStream ile okuyabileceği,
    /// üretici-tüketici mantığında çalışan basit bir tamponlu akış.
    /// NAudio tarafından doldurulur, SRE tarafından okunur.
    /// </summary>
    internal sealed class BufferedAudioStream : Stream
    {
        private readonly byte[] _buffer;
        private int _writePos;
        private int _readPos;
        private bool _isCompleted;
        private readonly object _lock = new object();
        private readonly AutoResetEvent _dataAvailable = new AutoResetEvent(false);

        public BufferedAudioStream(int capacityBytes = 32768)
        {
            _buffer = new byte[capacityBytes];
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException();

            int remaining = count;
            while (remaining > 0)
            {
                lock (_lock)
                {
                    int space = FreeSpaceNoLock();
                    while (space == 0)
                    {
                        // Basit bekleme; üretici tüketilmesini bekler
                        Monitor.Exit(_lock);
                        Thread.Sleep(2);
                        Monitor.Enter(_lock);
                        space = FreeSpaceNoLock();
                    }

                    int toCopy = Math.Min(space, remaining);
                    int firstChunk = Math.Min(toCopy, _buffer.Length - _writePos);
                    Buffer.BlockCopy(buffer, offset + (count - remaining), _buffer, _writePos, firstChunk);
                    _writePos = (_writePos + firstChunk) % _buffer.Length;
                    int second = toCopy - firstChunk;
                    if (second > 0)
                    {
                        Buffer.BlockCopy(buffer, offset + (count - remaining) + firstChunk, _buffer, _writePos, second);
                        _writePos = (_writePos + second) % _buffer.Length;
                    }
                    remaining -= toCopy;
                }
                _dataAvailable.Set();
            }
        }

        private int FreeSpaceNoLock()
        {
            if (_readPos <= _writePos)
                return _buffer.Length - (_writePos - _readPos) - 1;
            return _readPos - _writePos - 1;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_isCompleted && AvailableBytes() == 0) return 0;

            int readTotal = 0;
            while (readTotal == 0)
            {
                lock (_lock)
                {
                    int available = AvailableBytes();
                    if (available > 0)
                    {
                        int toRead = Math.Min(available, count);
                        int firstChunk = Math.Min(toRead, _buffer.Length - _readPos);
                        Buffer.BlockCopy(_buffer, _readPos, buffer, offset, firstChunk);
                        _readPos = (_readPos + firstChunk) % _buffer.Length;
                        int second = toRead - firstChunk;
                        if (second > 0)
                        {
                            Buffer.BlockCopy(_buffer, _readPos, buffer, offset + firstChunk, second);
                            _readPos = (_readPos + second) % _buffer.Length;
                        }
                        readTotal = toRead;
                    }
                }
                if (readTotal == 0)
                {
                    _dataAvailable.WaitOne(10);
                    if (_isCompleted && AvailableBytes() == 0) break;
                }
            }
            return readTotal;
        }

        private int AvailableBytes()
        {
            if (_writePos >= _readPos) return _writePos - _readPos;
            return _buffer.Length - (_readPos - _writePos);
        }

        public void CompleteWriting()
        {
            _isCompleted = true;
            _dataAvailable.Set();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _dataAvailable.Dispose();
        }
    }
}


