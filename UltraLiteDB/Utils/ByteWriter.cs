using System;
using System.Text;
using System.Buffers.Binary;

namespace UltraLiteDB
{
    public class ByteWriter
    {
        private byte[] _buffer;
        private int _pos;

        public byte[] Buffer { get { return _buffer; } }

        public int Position { get { return _pos; } set { _pos = value; } }


        public ByteWriter()
        {
            _buffer = null;
            _pos = 0;
        }

        public ByteWriter(int length)
        {
            _buffer = new byte[length];
            _pos = 0;
        }

        public ByteWriter(byte[] buffer, int offset = 0)
        {
            _buffer = buffer;
            _pos = offset;
        }

        public ByteWriter(ArraySegment<byte> buffer)
        {
            _buffer = buffer.Array;
            _pos = buffer.Offset;
        }

        public void Clear()
        {
            _buffer = null;
            _pos = 0;
        }

        public void Reset(byte[] buffer)
        {
            _buffer = buffer;
            _pos = 0;
        }

        public void Reset(ArraySegment<byte> buffer)
        {
            _buffer = buffer.Array;
            _pos = buffer.Offset;
        }

        public void Skip(int length)
        {
            _pos += length;
        }

        public void EnsureCapacity(int additionalBytes)
        {
            if (_buffer == null)
            {
                _buffer = new byte[additionalBytes];
            }
            else if (_pos + additionalBytes > _buffer.Length)
            {
                var newBuffer = new byte[Math.Max(_buffer.Length * 2, _pos + additionalBytes)];
                System.Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _pos);
                _buffer = newBuffer;
            }
        }

        #region Native data types

        public void Write(Byte value)
        {
            _buffer[_pos] = value;

            _pos++;
        }

        public void Write(Boolean value)
        {
            _buffer[_pos] = value ? (byte)1 : (byte)0;

            _pos++;
        }

        public void Write(UInt16 value)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(new Span<byte>(_buffer, _pos, 2), value);

            _pos += 2;
        }

        public void Write(UInt32 value)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(new Span<byte>(_buffer, _pos, 4), value);

            _pos += 4;
        }

        public void Write(UInt64 value)
        {
            BinaryPrimitives.WriteUInt64LittleEndian(new Span<byte>(_buffer, _pos, 8), value);

            _pos += 8;
        }

        public void Write(Int16 value)
        {
            BinaryPrimitives.WriteInt16LittleEndian(new Span<byte>(_buffer, _pos, 2), value);

            _pos += 2;
        }

        public void Write(Int32 value)
        {
            BinaryPrimitives.WriteInt32LittleEndian(new Span<byte>(_buffer, _pos, 4), value);

            _pos += 4;
        }

        public void Write(Int64 value)
        {
            BinaryPrimitives.WriteInt64LittleEndian(new Span<byte>(_buffer, _pos, 8), value);

            _pos += 8;
        }

        public void Write(Single value)
        {
            BitConverter.TryWriteBytes(new Span<byte>(_buffer, _pos, 4), value);

            _pos += 4;
        }

        public void Write(Double value)
        {
            BitConverter.TryWriteBytes(new Span<byte>(_buffer, _pos, 8), value);

            _pos += 8;
        }

        public void Write(Decimal value)
        {
            var array = Decimal.GetBits(value);

            this.Write(array[0]);
            this.Write(array[1]);
            this.Write(array[2]);
            this.Write(array[3]);
        }

        public void Write(Byte[] value)
        {
            System.Buffer.BlockCopy(value, 0, _buffer, _pos, value.Length);

            _pos += value.Length;
        }

        public void Write(ArraySegment<byte> value)
        {
            System.Buffer.BlockCopy(value.Array, value.Offset, _buffer, _pos, value.Count);

            _pos += value.Count;
        }

        #endregion

        #region Extended types

        public void Write(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            this.Write(bytes.Length);
            this.Write(bytes);
        }

        public void Write(string value, int length)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length != length) throw new ArgumentException("Invalid string length");
            this.Write(bytes);
        }

        public void Write(DateTime value)
        {
            this.Write(value.ToUniversalTime().Ticks);
        }

        public void Write(Guid value)
        {
            this.Write(value.ToByteArray());
        }

        public void Write(ObjectId value)
        {
            this.Write(value.ToByteArray());
        }

        internal void Write(PageAddress value)
        {
            this.Write(value.PageID);
            this.Write(value.Index);
        }

        public void WriteBsonValue(BsonValue value, ushort length)
        {
            this.Write((byte)value.Type);

            switch (value.Type)
            {
                case BsonType.Null:
                case BsonType.MinValue:
                case BsonType.MaxValue:
                    break;

                case BsonType.Int32: this.Write((Int32)value.RawValue); break;
                case BsonType.Int64: this.Write((Int64)value.RawValue); break;
                case BsonType.Double: this.Write((Double)value.RawValue); break;
                case BsonType.Decimal: this.Write((Decimal)value.RawValue); break;

                case BsonType.String: this.Write((String)value.RawValue, length); break;

                case BsonType.Document: BsonWriter.WriteDocument(this, value.AsDocument); break;
                case BsonType.Array: BsonWriter.WriteArray(this, value.AsArray); break;

                case BsonType.Binary: this.Write((Byte[])value.RawValue); break;
                case BsonType.ObjectId: this.Write((ObjectId)value.RawValue); break;
                case BsonType.Guid: this.Write((Guid)value.RawValue); break;

                case BsonType.Boolean: this.Write((Boolean)value.RawValue); break;
                case BsonType.DateTime: this.Write((DateTime)value.RawValue); break;

                default: throw new NotImplementedException();
            }
        }

        #endregion
    }
}