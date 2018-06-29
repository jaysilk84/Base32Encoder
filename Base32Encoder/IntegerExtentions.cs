using System;

namespace Base32Encoder {

    public static class IntegerExtentions {

        public static int NextMultipleOf(this int value, int multiple) {
            var modResult = value % multiple;
            return (modResult == 0) ? value : (multiple - modResult) + value;
        }

        public static byte[] MakeULong(byte[] input) {
            var buffer = new byte[8];

            for (var x = 0; x < 8 - input.Length; x++)
                buffer[x] = 0x00;

            Array.Copy(input, 0, buffer, 8 - input.Length, input.Length);

            return buffer;
        }

        public static byte[] MakeUInt(byte[] input) {
            var buffer = new byte[4];

            for (var x = 0; x < 4 - input.Length; x++)
                buffer[x] = 0x00;

            Array.Copy(input, 0, buffer, 4 - input.Length, input.Length);

            return buffer;
        }


    }
}