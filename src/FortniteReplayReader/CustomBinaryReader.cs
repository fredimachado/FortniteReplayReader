using System;
using System.IO;
using System.Text;

namespace FortniteReplayReader
{
    /// <summary>
    /// Custom Binary Reader with methods for Unreal Engine replay files
    /// </summary>
    public class CustomBinaryReader : BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the CustomBinaryReader class based on the specified stream.
        /// </summary>
        /// <param name="input">An stream.</param>
        /// <seealso cref="System.IO.BinaryReader"/> 
        public CustomBinaryReader(Stream input) : base(input)
        {
        }

        /// <summary>
        /// Reads a string from the current stream. The string is prefixed with the length as an 4-byte signed integer.
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Containers/String.cpp#L1390
        /// </summary>
        /// <returns>A string read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual string ReadFString()
        {
            var length = ReadInt32();

            if (length == 0)
            {
                return "";
            }

            var isUnicode = length < 0;
            byte[] data;
            string value;

            if (isUnicode)
            {
                length = -2 * length;
                data = ReadBytes(length);
                value = Encoding.Unicode.GetString(data);
            }
            else
            {
                data = ReadBytes(length);
                value = Encoding.Default.GetString(data);
            }

            return value.Trim(new[] { ' ', '\0' });
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and casts it to an Enum.
        /// Then advances the position of the stream by four bytes.
        /// </summary>
        ///  <typeparam name="T">The element type of the enum.</typeparam>
        /// <returns>A value of enum T.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual T ReadUInt32AsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadUInt32());
        }

        /// <summary>
        /// Reads a byte from the current stream and casts it to an Enum.
        /// Then advances the position of the stream by 1 byte.
        /// </summary>
        ///  <typeparam name="T">The element type of the enum.</typeparam>
        /// <returns>A value of enum T.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual T ReadByteAsEnum<T>()
        {
            return (T)Enum.ToObject(typeof(T), ReadByte());
        }

        /// <summary>
        /// Reads a Boolean value from the current stream and advances the current position of the stream by 4-bytes.
        /// </summary>
        /// <returns>true if the byte is nonzero; otherwise, false.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual bool ReadUInt32AsBoolean()
        {
            return ReadUInt32() == 1;
        }

        /// <summary>
        /// Reads a Boolean value from the current stream and advances the current position of the stream by 4-bytes.
        /// </summary>
        /// <returns>true if the byte is nonzero; otherwise, false.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual bool ReadInt32AsBoolean()
        {
            return ReadInt32() == 1;
        }

        /// <summary>
        /// Reads an array of tuples from the current stream. The array is prefixed with the number of items in it.
        /// see https://github.com/EpicGames/UnrealEngine/blob/7d9919ac7bfd80b7483012eab342cb427d60e8c9/Engine/Source/Runtime/Core/Public/Containers/Array.h#L1069
        /// </summary>
        /// <typeparam name="T">The type of the first value.</typeparam>
        /// <typeparam name="U">The type of the second value.</typeparam>
        /// <param name="func1">The function to parse the fist value.</param>
        /// <param name="func2">The function to parse the second value.</param>
        /// <returns>An array of tuples.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual (T, U)[] ReadTupleArray<T, U>(Func<T> func1, Func<U> func2)
        {
            var count = ReadUInt32();
            var arr = new (T, U)[count];
            for (var i = 0; i < count; i++)
            {
                arr[i] = (func1.Invoke(), func2.Invoke());
            }
            return arr;
        }

        /// <summary>
        /// Reads an array of <typeparamref name="T"/> from the current stream. The array is prefixed with the number of items in it.
        /// see https://github.com/EpicGames/UnrealEngine/blob/7d9919ac7bfd80b7483012eab342cb427d60e8c9/Engine/Source/Runtime/Core/Public/Containers/Array.h#L1069
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="func1">The function to the value.</param>
        /// <returns>An array of tuples.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual T[] ReadArray<T>(Func<T> func1)
        {
            var count = ReadUInt32();
            var arr = new T[count];
            for (var i = 0; i < count; i++)
            {
                arr[i] = (func1.Invoke());
            }
            return arr;
        }

        /// <summary>
        /// Reads <paramref name="count"/> bytes from the current stream and advances the current position of the stream by <paramref name="count"/>-bytes.
        /// </summary>
        /// <param name="count">Numer of bytes to read.</param>
        /// <returns>A string.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual string ReadBytesToString(int count)
        {
            // https://github.com/dotnet/corefx/issues/10013
            return BitConverter.ToString(ReadBytes(count)).Replace("-", "");
        }

        /// <summary>
        /// Reads 16 bytes from the current stream and advances the current position of the stream by 16-bytes.
        /// </summary>
        /// <returns>A GUID in string format read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public virtual string ReadGUID()
        {
            return ReadBytesToString(16);
        }

        /// <summary>
        /// Advances the current position of the stream by <paramref name="byteCount"/> bytes.
        /// </summary>
        /// <param name="byteCount">The amount of bytes to skip. This value must be 0 or a non-negative number.</param>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public void SkipBytes(uint byteCount)
        {
            BaseStream.Seek(byteCount, SeekOrigin.Current);
        }
        
        /// <summary>
        /// Advances the current position of the stream by <paramref name="byteCount"/> bytes.
        /// </summary>
        /// <param name="byteCount">The amount of bytes to skip. This value must be 0 or a non-negative number.</param>
        /// <exception cref="System.IO.EndOfStreamException">Thrown when the end of the stream is reached.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown when the stream is closed.</exception>
        /// <exception cref="System.IO.IOException">Thrown when an I/O error occurs.</exception>
        public void SkipBytes(int byteCount)
        {
            BaseStream.Seek(byteCount, SeekOrigin.Current);
        }

        /// <summary>
        /// see https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Private/Serialization/Archive.cpp#L1026
        /// </summary>
        /// <returns>uint</returns>
        public virtual uint ReadIntPacked()
        {
            uint value = 0;
            byte count = 0;
            var remaining = true;

            while (remaining)
            {
                var nextByte = ReadByte();
                remaining = (nextByte & 1) == 1;            // Check 1 bit to see if theres more after this
                nextByte >>= 1;                             // Shift to get actual 7 bit value
                value += (uint)nextByte << (7 * count++);   // Add to total value
            }
            return value;
        }
    }
}
