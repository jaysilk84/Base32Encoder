using System;
using System.Linq;
using System.Text;

namespace Base32Encoder {
    public class ThreeByteBlock {
        private const byte FiveBitMask = 0x1F;
        private const int Bits = 32;
        private readonly static byte[] Symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ012345=".Select(x => Encoding.ASCII.GetBytes(new char[] { x }) [0]).ToArray();
        private readonly byte[] _value;
        private readonly int _count;
        private readonly int _paddingCount;
        private readonly int _expectedCharacters;
        private readonly uint _bitsToEncode;

        static ThreeByteBlock() { }

        public ThreeByteBlock(string input) {
            if (input.Length > 3)
                throw new ArgumentOutOfRangeException("input");

            _value = IntegerExtentions.MakeUInt(Encoding.ASCII.GetBytes(input));
            _bitsToEncode = BitConverter.ToUInt32(_value, 0);

            _count = _value.Count(x => x != 0x00);
            _expectedCharacters = (_count * 8).NextMultipleOf(5) / 5;
            _paddingCount = 5 - _expectedCharacters;
        }

        public byte[] Encode() {
            var buffer = new byte[_expectedCharacters];

            for (int x = Bits - 5, i = 0; x >= Bits - (_count * 8).NextMultipleOf(5); x -= 5, i++)
                buffer[i] = Symbols[(_bitsToEncode >> x) & FiveBitMask];

            return buffer;
        }

        public static ThreeByteBlock Parse(string input, out string next) {
            next = new string(input.Skip(3).ToArray());
            return new ThreeByteBlock(new string(input.Take(3).ToArray()));
        }

        public int PaddingCount { get { return _paddingCount; } }
        public int ExpectedOutputCount { get { return _expectedCharacters; } }
        public int InputCount { get { return _count; } }
    }
}