using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Base32Encoder {
    public class FiveByteBlock {
        private const byte FiveBitMask = 0x1F;
        private const int Bits = 64;
        private readonly static byte[] Symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ012345=".Select(x => Encoding.ASCII.GetBytes(new char[] { x }) [0]).ToArray();
        private readonly byte[] _value;
        private readonly int _count;
        private readonly int _paddingCount;
        private readonly int _expectedCharacters;
        private readonly ulong _bitsToEncode;

        static FiveByteBlock() { }

        public FiveByteBlock(string input) {
            if (input.Length > 5)
                throw new ArgumentOutOfRangeException("input");

            _value = IntegerExtentions.MakeULong(Encoding.ASCII.GetBytes(input));
            _bitsToEncode = BitConverter.ToUInt64(_value, 0);

            _count = _value.Count(x => x != 0x00);
            _expectedCharacters = (_count * 8).NextMultipleOf(5) / 5;
            _paddingCount = 8 - _expectedCharacters;
        }

        public byte[] Encode() {
            var buffer = new byte[_expectedCharacters];

            for (int x = Bits - 5, i = 0; x >= Bits - (_count * 8).NextMultipleOf(5); x -= 5, i++)
                buffer[i] = Symbols[(_bitsToEncode >> x) & FiveBitMask];

            return buffer;
        }

        public static byte[] Decode(string encoded) {
            var buffer = new List<byte>();

            for (var x = 0; x < encoded.Length; x+=8) {
                ulong bitStore = 0;
                int padding = 8 - encoded.Length;

                foreach (var c in encoded.Substring(x, Math.Min(encoded.Length, 8))) {
                    var base32Val = (byte)Array.IndexOf(Symbols, (byte)c);
                    bitStore = (bitStore | (byte)(base32Val)) << 5;
                }

                bitStore <<= 19 + (5 * padding);
                buffer.AddRange(BitConverter.GetBytes(bitStore));
            }
            
            return buffer.ToArray();
        }

        public static FiveByteBlock Parse(string input, out string next) {
            next = new string(input.Skip(5).ToArray());
            return new FiveByteBlock(new string(input.Take(5).ToArray()));
        }

        public int PaddingCount => _paddingCount;
        public int ExpectedOutputCount => _expectedCharacters;
        public int InputCount => _count;
        public ulong Value => _bitsToEncode;
    }
}