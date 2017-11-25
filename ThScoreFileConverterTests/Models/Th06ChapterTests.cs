﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace ThScoreFileConverter.Models.Tests
{
    public sealed class Th06ChapterWrapper<TParent>
    {
        private static Type ParentType = typeof(TParent);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+Chapter";

        private PrivateObject pobj = null;

        private Th06ChapterWrapper(params object[] args)
        {
            this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest, args);
        }

        public Th06ChapterWrapper()
            : this(new object[] { })
        {
        }

        public Th06ChapterWrapper(Th06ChapterWrapper<TParent> chapter)
            : this(new object[] { chapter?.Target })
        {
        }

        public object Target => this.pobj.Target;

        public string Signature
            => this.pobj.GetProperty(nameof(Signature)) as string;
        public short? Size1
            => this.pobj.GetProperty(nameof(Size1)) as short?;
        public short? Size2
            => this.pobj.GetProperty(nameof(Size2)) as short?;
        public byte? FirstByteOfData
            => this.pobj.GetProperty(nameof(FirstByteOfData)) as byte?;
        public IReadOnlyCollection<byte> Data
            => this.pobj.GetProperty(nameof(Data)) as byte[];

        public void ReadFrom(BinaryReader reader)
            => this.pobj.Invoke(
                nameof(ReadFrom),
                new object[] { reader },
                CultureInfo.InvariantCulture);
    }

    [TestClass()]
    public class Th06ChapterTests
    {
        public static byte[] MakeByteArray(
            string signature,
            short size1,
            short size2,
            byte[] data)
        {
            if (signature == null)
                throw new ArgumentNullException(nameof(signature));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte[] array = null;

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                using (var writer = new BinaryWriter(stream, Encoding.Default))
                {
                    stream = null;

                    writer.Write(signature.ToCharArray());
                    writer.Write(size1);
                    writer.Write(size2);
                    writer.Write(data);
                    writer.Flush();

                    array = new byte[writer.BaseStream.Length];
                    writer.BaseStream.Position = 0;
                    writer.BaseStream.Read(array, 0, array.Length);
                }
            }
            finally
            {
                stream?.Dispose();
            }

            return array;
        }

        public static Th06ChapterWrapper<TParent> Create<TParent>(byte[] array)
        {
            var chapter = new Th06ChapterWrapper<TParent>();

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(array);
                using (var reader = new BinaryReader(stream))
                {
                    stream = null;
                    chapter.ReadFrom(reader);
                }
            }
            finally
            {
                stream?.Dispose();
            }

            return chapter;
        }

        public static void Validate<TParent>(
            Th06ChapterWrapper<TParent> chapter,
            string signature,
            short size1,
            short size2,
            byte[] data)
        {
            if (chapter == null)
                throw new ArgumentNullException(nameof(chapter));

            Assert.AreEqual(signature, chapter.Signature);
            Assert.AreEqual(size1, chapter.Size1);
            Assert.AreEqual(size2, chapter.Size2);
            CollectionAssert.AreEqual(data, chapter.Data.ToArray());
            Assert.AreEqual((data?.Length > 0 ? data[0] : (byte)0), chapter.FirstByteOfData);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ChapterTestHelper<TParent>()
        {
            try
            {
                var chapter = new Th06ChapterWrapper<TParent>();
                Validate(chapter, string.Empty, 0, 0, new byte[] { });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ChapterTestCopyHelper<TParent>()
        {
            try
            {
                var chapter1 = new Th06ChapterWrapper<TParent>();
                var chapter2 = new Th06ChapterWrapper<TParent>(chapter1);
                Validate(chapter2, string.Empty, 0, 0, new byte[] { });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "chapter")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ChapterTestNullHelper<TParent>()
        {
            try
            {
                var chapter = new Th06ChapterWrapper<TParent>(null);
                Assert.Fail("Unreachable");
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestHelper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 12;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                var chapter = new Th06ChapterWrapper<TParent>();

                MemoryStream stream = null;
                try
                {
                    stream = new MemoryStream(MakeByteArray(signature, size1, size2, data));
                    using (var reader = new BinaryReader(stream))
                    {
                        stream = null;
                        chapter.ReadFrom(reader);
                    }
                }
                finally
                {
                    stream?.Dispose();
                }

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestNullHelper<TParent>()
        {
            try
            {
                var chapter = new Th06ChapterWrapper<TParent>();
                chapter.ReadFrom(null);
                Assert.Fail("Unreachable");
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestEmptySignatureHelper<TParent>()
        {
            try
            {
                var signature = string.Empty;
                short size1 = 12;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                // <-- sig --> size1 size2 <- data -->
                // __ __ __ __ 0c 00 22 00 56 78 9a bc
                //             <-- sig --> size1 size2 <dat>

                // The actual value of the Size1 property becomes too large and
                // the Data property becomes empty,
                // so EndOfStreamException will be thrown.
                Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Assert.Fail("Unreachable");
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestShortenedSignatureHelper<TParent>()
        {
            try
            {
                var signature = "ABC";
                short size1 = 12;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                // <-- sig --> size1 size2 <- data -->
                // __ 41 42 43 0c 00 22 00 56 78 9a bc
                //    <-- sig --> size1 size2 < dat ->

                // The actual value of the Size property becomes too large,
                // so EndOfStreamException will be thrown.
                Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Assert.Fail("Unreachable");
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestExceededSignatureHelper<TParent>()
        {
            try
            {
                var signature = "ABCDE";
                short size1 = 12;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                // <--- sig ----> size1 size2 <- data -->
                // 41 42 43 44 45 0c 00 22 00 56 78 9a bc
                // <-- sig --> size1 size2 <---- data ---->

                // The actual value of the Size property becomes too large,
                // so EndOfStreamException will be thrown.
                Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Assert.Fail("Unreached");
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestNegativeSize1Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = -1;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Assert.Fail("Unreached");
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestZeroSize1Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 0;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Assert.Fail("Unreached");
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestShortenedSize1Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 11;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Assert.AreEqual(signature, chapter.Signature);
                Assert.AreEqual(size1, chapter.Size1);
                Assert.AreEqual(size2, chapter.Size2);
                CollectionAssert.AreNotEqual(data, chapter.Data.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestExceededSize1Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 13;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestNegativeSize2Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 12;
                short size2 = -1;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestZeroSize2Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 12;
                short size2 = 0;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestShortenedSize2Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 12;
                short size2 = 33;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestExceededSize2Helper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 12;
                short size2 = 35;
                var data = new byte[] { 0x56, 0x78, 0x9A, 0xBC };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestEmptyDataHelper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 12;
                short size2 = 34;
                var data = new byte[] { };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void ReadFromTestMisalignedDataHelper<TParent>()
        {
            try
            {
                var signature = "ABCD";
                short size1 = 11;
                short size2 = 34;
                var data = new byte[] { 0x56, 0x78, 0x9A };

                var chapter = Create<TParent>(MakeByteArray(signature, size1, size2, data));

                Validate(chapter, signature, size1, size2, data);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [TestMethod()]
        public void Th06ChapterTest()
        {
            ChapterTestHelper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterTestCopy()
        {
            ChapterTestCopyHelper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th06ChapterTestNull()
        {
            ChapterTestNullHelper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterReadFromTest()
        {
            ReadFromTestHelper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th06ChapterReadFromTestNull()
        {
            ReadFromTestNullHelper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th06ChapterReadFromTestEmptySignature()
        {
            ReadFromTestEmptySignatureHelper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th06ChapterReadFromTestShortenedSignature()
        {
            ReadFromTestShortenedSignatureHelper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th06ChapterReadFromTestExceededSignature()
        {
            ReadFromTestExceededSignatureHelper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th06ChapterReadFromTestNegativeSize1()
        {
            ReadFromTestNegativeSize1Helper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th06ChapterReadFromTestZeroSize1()
        {
            ReadFromTestZeroSize1Helper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterReadFromTestShortenedSize1()
        {
            ReadFromTestShortenedSize1Helper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th06ChapterReadFromTestExceededSize1()
        {
            ReadFromTestExceededSize1Helper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterReadFromTestNegativeSize2()
        {
            ReadFromTestNegativeSize2Helper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterReadFromTestZeroSize2()
        {
            ReadFromTestZeroSize2Helper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterReadFromTestShortenedSize2()
        {
            ReadFromTestShortenedSize2Helper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterReadFromTestExceededSize2()
        {
            ReadFromTestExceededSize2Helper<Th06Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th06ChapterReadFromTestEmptyData()
        {
            ReadFromTestEmptyDataHelper<Th06Converter>();
        }

        [TestMethod()]
        public void Th06ChapterReadFromTestMisalignedData()
        {
            ReadFromTestMisalignedDataHelper<Th06Converter>();
        }

        [TestMethod()]
        public void Th07ChapterTest()
        {
            ChapterTestHelper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterTestCopy()
        {
            ChapterTestCopyHelper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th07ChapterTestNull()
        {
            ChapterTestNullHelper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterReadFromTest()
        {
            ReadFromTestHelper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th07ChapterReadFromTestNull()
        {
            ReadFromTestNullHelper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th07ChapterReadFromTestEmptySignature()
        {
            ReadFromTestEmptySignatureHelper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th07ChapterReadFromTestShortenedSignature()
        {
            ReadFromTestShortenedSignatureHelper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th07ChapterReadFromTestExceededSignature()
        {
            ReadFromTestExceededSignatureHelper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th07ChapterReadFromTestNegativeSize1()
        {
            ReadFromTestNegativeSize1Helper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th07ChapterReadFromTestZeroSize1()
        {
            ReadFromTestZeroSize1Helper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterReadFromTestShortenedSize1()
        {
            ReadFromTestShortenedSize1Helper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th07ChapterReadFromTestExceededSize1()
        {
            ReadFromTestExceededSize1Helper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterReadFromTestNegativeSize2()
        {
            ReadFromTestNegativeSize2Helper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterReadFromTestZeroSize2()
        {
            ReadFromTestZeroSize2Helper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterReadFromTestShortenedSize2()
        {
            ReadFromTestShortenedSize2Helper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterReadFromTestExceededSize2()
        {
            ReadFromTestExceededSize2Helper<Th07Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th07ChapterReadFromTestEmptyData()
        {
            ReadFromTestEmptyDataHelper<Th07Converter>();
        }

        [TestMethod()]
        public void Th07ChapterReadFromTestMisalignedData()
        {
            ReadFromTestMisalignedDataHelper<Th07Converter>();
        }

        [TestMethod()]
        public void Th08ChapterTest()
        {
            ChapterTestHelper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterTestCopy()
        {
            ChapterTestCopyHelper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th08ChapterTestNull()
        {
            ChapterTestNullHelper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterReadFromTest()
        {
            ReadFromTestHelper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th08ChapterReadFromTestNull()
        {
            ReadFromTestNullHelper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08ChapterReadFromTestEmptySignature()
        {
            ReadFromTestEmptySignatureHelper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08ChapterReadFromTestShortenedSignature()
        {
            ReadFromTestShortenedSignatureHelper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08ChapterReadFromTestExceededSignature()
        {
            ReadFromTestExceededSignatureHelper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th08ChapterReadFromTestNegativeSize1()
        {
            ReadFromTestNegativeSize1Helper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th08ChapterReadFromTestZeroSize1()
        {
            ReadFromTestZeroSize1Helper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterReadFromTestShortenedSize1()
        {
            ReadFromTestShortenedSize1Helper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08ChapterReadFromTestExceededSize1()
        {
            ReadFromTestExceededSize1Helper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterReadFromTestNegativeSize2()
        {
            ReadFromTestNegativeSize2Helper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterReadFromTestZeroSize2()
        {
            ReadFromTestZeroSize2Helper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterReadFromTestShortenedSize2()
        {
            ReadFromTestShortenedSize2Helper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterReadFromTestExceededSize2()
        {
            ReadFromTestExceededSize2Helper<Th08Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08ChapterReadFromTestEmptyData()
        {
            ReadFromTestEmptyDataHelper<Th08Converter>();
        }

        [TestMethod()]
        public void Th08ChapterReadFromTestMisalignedData()
        {
            ReadFromTestMisalignedDataHelper<Th08Converter>();
        }

        [TestMethod()]
        public void Th09ChapterTest()
        {
            ChapterTestHelper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterTestCopy()
        {
            ChapterTestCopyHelper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th09ChapterTestNull()
        {
            ChapterTestNullHelper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterReadFromTest()
        {
            ReadFromTestHelper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th09ChapterReadFromTestNull()
        {
            ReadFromTestNullHelper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th09ChapterReadFromTestEmptySignature()
        {
            ReadFromTestEmptySignatureHelper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th09ChapterReadFromTestShortenedSignature()
        {
            ReadFromTestShortenedSignatureHelper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th09ChapterReadFromTestExceededSignature()
        {
            ReadFromTestExceededSignatureHelper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th09ChapterReadFromTestNegativeSize1()
        {
            ReadFromTestNegativeSize1Helper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Th09ChapterReadFromTestZeroSize1()
        {
            ReadFromTestZeroSize1Helper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterReadFromTestShortenedSize1()
        {
            ReadFromTestShortenedSize1Helper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th09ChapterReadFromTestExceededSize1()
        {
            ReadFromTestExceededSize1Helper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterReadFromTestNegativeSize2()
        {
            ReadFromTestNegativeSize2Helper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterReadFromTestZeroSize2()
        {
            ReadFromTestZeroSize2Helper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterReadFromTestShortenedSize2()
        {
            ReadFromTestShortenedSize2Helper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterReadFromTestExceededSize2()
        {
            ReadFromTestExceededSize2Helper<Th09Converter>();
        }

        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th09ChapterReadFromTestEmptyData()
        {
            ReadFromTestEmptyDataHelper<Th09Converter>();
        }

        [TestMethod()]
        public void Th09ChapterReadFromTestMisalignedData()
        {
            ReadFromTestMisalignedDataHelper<Th09Converter>();
        }
    }
}