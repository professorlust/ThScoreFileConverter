﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models.Wrappers
{
    public sealed class Th06FileHeaderWrapper
    {
        private static Type ParentType = typeof(Th06Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+FileHeader";

        private readonly PrivateObject pobj = null;

        public static Th06FileHeaderWrapper Create(byte[] array)
        {
            var chapter = new Th06FileHeaderWrapper();

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

        public Th06FileHeaderWrapper()
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest);

        public object Target
            => this.pobj.Target;
        public ushort? Checksum
            => this.pobj.GetProperty(nameof(Checksum)) as ushort?;
        public short? Version
            => this.pobj.GetProperty(nameof(Version)) as short?;
        public int? Size
            => this.pobj.GetProperty(nameof(Size)) as int?;
        public int? DecodedAllSize
            => this.pobj.GetProperty(nameof(DecodedAllSize)) as int?;
        public bool? IsValid
            => this.pobj.GetProperty(nameof(IsValid)) as bool?;

        public void ReadFrom(BinaryReader reader)
            => this.pobj.Invoke(nameof(ReadFrom), new object[] { reader }, CultureInfo.InvariantCulture);
        public void WriteTo(BinaryWriter writer)
            => this.pobj.Invoke(nameof(WriteTo), new object[] { writer }, CultureInfo.InvariantCulture);
    }
}