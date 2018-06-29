using System;

namespace Base32Encoder {
    public static class ByteExtensions {
        public static byte[] Prepend(this byte[] array, byte valueToPrepend) {
            var buffer = new byte[array.Length + 1];
            buffer[0] = valueToPrepend;
            Array.Copy(array, 0, buffer, 1, array.Length);

            return buffer;
        }
        public static byte[] Append(this byte[] array, byte valueToPrepend) {
            var buffer = new byte[array.Length + 1];
            Array.Copy(array, 0, buffer, 0, array.Length);
            buffer[buffer.Length - 1] = valueToPrepend;

            return buffer;
        }
    }
}