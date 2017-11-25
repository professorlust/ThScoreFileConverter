﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ThScoreFileConverter.Models.Tests
{
    [TestClass()]
    public class LzssTests
    {
        // "AbcAbc"
        private readonly byte[] decompressed = new byte[]
        {
            0x41, 0x62, 0x63, 0x41, 0x62, 0x63
        };

        // f <- ch -->
        // 1 0100_0001
        // 1 0110_0010
        // 1 0110_0011
        // f <--- offset ---> len>
        // 0 0_0000_0000_0001 0000
        // 0 0_0000_0000_0000
        private readonly byte[] compressed = new byte[]
        {
            0xA0, 0xD8, 0xAC, 0x60, 0x00, 0x80, 0x00, 0x00
        };

        [TestMethod()]
        [ExpectedException(typeof(NotImplementedException))]
        public void CompressTest()
        {
            using (var input = new MemoryStream(this.decompressed))
            using (var output = new MemoryStream())
            {
                Lzss.Compress(input, output);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        public void ExtractTest()
        {
            using (var input = new MemoryStream(this.compressed))
            using (var output = new MemoryStream())
            {
                // FIXME: Should be renamed
                Lzss.Extract(input, output);

                var actual = new byte[output.Length];
                output.Position = 0;
                output.Read(actual, 0, actual.Length);

                CollectionAssert.AreEqual(this.decompressed, actual);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExtractTestNullInput()
        {
            using (var output = new MemoryStream())
            {
                Lzss.Extract(null, output);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        public void ExtractTestNullStreamInput()
        {
            using (var output = new MemoryStream())
            {
                Lzss.Extract(Stream.Null, output);
                Assert.AreEqual(0, output.Length);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void ExtractTestEmptyInput()
        {
            using (var input = new MemoryStream())
            using (var output = new MemoryStream())
            {
                Lzss.Extract(input, output);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractTestUnreadableInput()
        {
            using (var input = new UnreadableMemoryStream())
            using (var output = new MemoryStream())
            {
                Lzss.Extract(input, output);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ExtractTestClosedInput()
        {
            using (var output = new MemoryStream())
            {
                var input = new MemoryStream(this.compressed);
                input.Close();
                Lzss.Extract(input, output);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void ExtractTestShortenedInput()
        {
            using (var input = new MemoryStream(this.compressed, 0, this.compressed.Length - 1))
            using (var output = new MemoryStream())
            {
                Lzss.Extract(input, output);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        public void ExtractTestInvalidInput()
        {
            var invalid = new byte[this.compressed.Length];
            this.compressed.CopyTo(invalid, 0);
            invalid[invalid.Length - 1] ^= 0x80;

            using (var input = new MemoryStream(invalid))
            using (var output = new MemoryStream())
            {
                Lzss.Extract(input, output);

                var actual = new byte[output.Length];
                output.Position = 0;
                output.Read(actual, 0, actual.Length);

                CollectionAssert.AreNotEqual(this.decompressed, actual);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void ExtractTestNullOutput()
        {
            using (var input = new MemoryStream(this.compressed))
            {
                Lzss.Extract(input, null);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void ExtractTestUnwritableOutput()
        {
            using (var input = new MemoryStream(this.compressed))
            using (var output = new MemoryStream(new byte[] { }, false))
            {
                Lzss.Extract(input, output);
                Assert.Fail("Unreachable");
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ExtractTestClosedOutput()
        {
            using (var input = new MemoryStream(this.compressed))
            {
                var output = new MemoryStream();
                output.Close();
                Lzss.Extract(input, output);
                Assert.Fail("Unreachable");
            }
        }
    }
}