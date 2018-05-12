﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ThScoreFileConverter.Models.Tests
{
    public static class TestUtils
    {
        public static string Unreachable => nameof(Unreachable);

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static byte[] MakeByteArray(params object[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            byte[] array = null;

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                using (var writer = new BinaryWriter(stream, Encoding.Default))
                {
                    stream = null;

                    foreach (var arg in args)
                    {
                        switch (arg)
                        {
                            case byte[] bytesArg:
                                writer.Write(bytesArg);
                                break;
                            case char[] charsArg:
                                writer.Write(charsArg);
                                break;
                            case sbyte sbyteArg:
                                writer.Write(sbyteArg);
                                break;
                            case byte byteArg:
                                writer.Write(byteArg);
                                break;
                            case char charArg:
                                writer.Write(charArg);
                                break;
                            case short shortArg:
                                writer.Write(shortArg);
                                break;
                            case ushort ushortArg:
                                writer.Write(ushortArg);
                                break;
                            case int intArg:
                                writer.Write(intArg);
                                break;
                            case uint uintArg:
                                writer.Write(uintArg);
                                break;
                            case long longArg:
                                writer.Write(longArg);
                                break;
                            case ulong ulongArg:
                                writer.Write(ulongArg);
                                break;
                            case ushort[] ushortsArg:
                                foreach (var val in ushortsArg)
                                    writer.Write(val);
                                break;
                            case int[] intsArg:
                                foreach (var val in intsArg)
                                    writer.Write(val);
                                break;
                            case bool boolArg:
                                writer.Write(boolArg);
                                break;
                            case double doubleArg:
                                writer.Write(doubleArg);
                                break;
                            case float floatArg:
                                writer.Write(floatArg);
                                break;
                            case string stringArg:
                                writer.Write(stringArg);
                                break;
                            case decimal decimalArg:
                                writer.Write(decimalArg);
                                break;
                            default:
                                break;
                        }
                    }

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

        public static TResult[] MakeRandomArray<TResult>(int length)
            where TResult : struct
        {
            var random = new Random();
            var defaultValue = default(TResult);
            var maxValue = 0;
            Func<object> getNextValue;

            switch (defaultValue)
            {
                case byte _:
                    getNextValue = () => random.Next(byte.MaxValue + 1);
                    break;
                case short _:
                    getNextValue = () => random.Next(short.MaxValue + 1);
                    break;
                case ushort _:
                    getNextValue = () => random.Next(ushort.MaxValue + 1);
                    break;
                case int _:
                    maxValue = ushort.MaxValue + 1;
                    getNextValue = () => ((random.Next(maxValue) << 16) | random.Next(maxValue));
                    break;
                case uint _:
                    maxValue = ushort.MaxValue + 1;
                    getNextValue = () => (((uint)random.Next(maxValue) << 16) | (uint)random.Next(maxValue));
                    break;
                default:
                    throw new NotImplementedException();
            }

            return Enumerable
                .Repeat(defaultValue, length)
                .Select(i => (TResult)Convert.ChangeType(getNextValue(), typeof(TResult), CultureInfo.InvariantCulture))
                .ToArray();
        }
    }
}