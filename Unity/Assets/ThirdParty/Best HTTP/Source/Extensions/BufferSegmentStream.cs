using System;
using System.Collections.Generic;
using System.IO;

using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.Extensions
{
    public sealed class BufferSegmentStream : Stream
    {
        public override bool CanRead { get { return false; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return false; } }

        public override long Length { get { return this._length; } }
        private long _length;

        public override long Position { get { return 0; } set { } }

        List<BufferSegment> bufferList = new List<BufferSegment>();

        public override int Read(byte[] buffer, int offset, int count)
        {
            int sumReadCount = 0;

            while (count > 0 && bufferList.Count > 0)
            {
                BufferSegment buff = this.bufferList[0];

                int readCount = Math.Min(count, buff.Count);

                Array.Copy(buff.Data, buff.Offset, buffer, offset, readCount);

                sumReadCount += readCount;
                offset += readCount;
                count -= readCount;

                if (readCount >= buff.Count)
                {
                    this.bufferList.RemoveAt(0);
                    BufferPool.Release(buff.Data);
                }
                else
                    this.bufferList[0] = new BufferSegment(buff.Data, buff.Offset + readCount, buff.Count - readCount);
            }

            return sumReadCount;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.Write(new BufferSegment(buffer, offset, count));
        }

        public void Write(BufferSegment bufferSegment)
        {
            this.bufferList.Add(bufferSegment);
            this._length += bufferSegment.Count;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._length = 0;
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
