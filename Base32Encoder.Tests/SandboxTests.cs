using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Base32Encoder;
using Xunit;

namespace Base32Encoder.Tests {

    public class SandboxTests {
        [Fact]
        public void TestFiveByteBlockDecode() {
            var test = "kjfWERewr23432rfewfsd3434rewqr34few23423erweqr@#$REwrrr";
            var encoded = new List<byte>();

            while (test.Length > 0) { 
                var b = FiveByteBlock.Parse(test, out test);
                encoded.AddRange(b.Encode());
            }

            Console.WriteLine($"Encoded String: {Encoding.ASCII.GetString(encoded.ToArray())}");

            var decoded = FiveByteBlock.Decode(Encoding.ASCII.GetString(encoded.ToArray()));

            Console.WriteLine($"Decoded: {Encoding.ASCII.GetString(decoded)}");
        }

        [Fact]
        public void TestThreeByteBlock() {
            string test = "kjfWERewr23432rfewfsd3434rewqr34few23423erweqr@#$REwrrr";
            var originalLength = test.Length;
            var i = 0;
            while (test.Length > 0) {
                var b = ThreeByteBlock.Parse(test, out test);
                var e = b.Encode();
                i += e.Length;
                foreach (var x in e)
                    Console.Write($"{Encoding.ASCII.GetChars(new byte [] { x })[0]}");
                
                if (test.Length == 0)
                    Console.WriteLine($"\nPadding: {b.PaddingCount}, Last block count {b.InputCount}, Expected output: {b.ExpectedOutputCount}");
            }

            Console.WriteLine("");
            Console.WriteLine($"Original length: {originalLength} Encoded Length: {i}");
        }

        [Fact]
        public void TestFiveByteBlock() {
            var block = new FiveByteBlock("L0l.,");
            var encoded = block.Encode();

            foreach (var b in encoded)
                Console.WriteLine($"value: {b}, char: {Encoding.ASCII.GetChars(new byte [] { b })[0]}");

            string test = "kjfWERewr23432rfewfsd3434rewqr34few23423erweqr@#$REwrrr";
            var originalLength = test.Length;
            var i = 0;
            while (test.Length > 0) {
                var b = FiveByteBlock.Parse(test, out test);
                var e = b.Encode();
                i += e.Length;
                foreach (var x in e)
                    Console.Write($"{Encoding.ASCII.GetChars(new byte [] { x })[0]}");

                if (test.Length == 0)
                    Console.WriteLine($"\nPadding: {b.PaddingCount}, Last block count {b.InputCount}, Expected output: {b.ExpectedOutputCount}");
            }

            Console.WriteLine("");
            Console.WriteLine($"Original length: {originalLength} Encoded Length: {i}");

        }

        public static byte[] MakeULong(byte[] input) {
            var buffer = new byte[8];

            for (var x = 0; x < 8 - input.Length; x++)
                buffer[x] = 0x00;

            Array.Copy(input, 0, buffer, 8 - input.Length, input.Length);

            return buffer;
        }

        [Fact]
        public void BitTwiddlingTests() {
            byte[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ012345=".Select(x => Encoding.ASCII.GetBytes(new char[] { x }) [0]).ToArray();
            byte fiveBitMask = 0x1F;
            int bits = 64;

            var value = MakeULong(Encoding.ASCII.GetBytes("L0l.,"));
            int count = value.Count(x => x != 0x00);
            int expectedCharacters = (count * 8).NextMultipleOf(5) / 5;
            int paddingCount = 8 - expectedCharacters;

            ulong total = BitConverter.ToUInt64(value, 0);

            Console.WriteLine($"Total: {total}");
            for (var x = bits - 5; x >= bits - (count * 8).NextMultipleOf(5); x -= 5) {
                var fiveBlock = (total >> x) & fiveBitMask; //(x - (x == 0 ? 0 : 1))) & fiveBitMask;
                Console.WriteLine($"Segment: {x}, value: {fiveBlock}, char: {Encoding.ASCII.GetChars(new byte [] { alphabet[fiveBlock] })[0]}");
            }

            Console.WriteLine($"Expected encoded characters: {expectedCharacters} Padding: {paddingCount}");

            // Console.WriteLine((total & first) >> 18);
            // Console.WriteLine((total & second) >> 12);
            // Console.WriteLine((total & third) >> 6);
            // Console.WriteLine((total & fourth));

            // char encoded1 = Encoding.ASCII.GetChars(new byte[] { alphabet[(total & first)] })[0];
            // char encoded2 = Encoding.ASCII.GetChars(new byte[] { alphabet[(total & second)] })[0];
            // char encoded3 = Encoding.ASCII.GetChars(new byte[] { alphabet[(total & third)] })[0];
            // char encoded4 = Encoding.ASCII.GetChars(new byte[] { alphabet[(total & fourth)] })[0];

            // Console.WriteLine($"Result: {encoded1} {encoded2} {encoded3} {encoded4}");

            // string base64Test = Convert.ToBase64String(new byte[] { 64 });
            // Console.WriteLine(base64Test);
            // Console.WriteLine($"Value: {value} Six: {sixBits} Two: {twoBits}");
        }
    }
}