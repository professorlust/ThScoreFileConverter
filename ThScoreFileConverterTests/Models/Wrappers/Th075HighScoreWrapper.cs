﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models.Wrappers
{
    public sealed class Th075HighScoreWrapper
    {
        private static Type ParentType = typeof(Th075Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+HighScore";

        private readonly PrivateObject pobj = null;

        public static Th075HighScoreWrapper Create(byte[] array)
        {
            var highScore = new Th075HighScoreWrapper();

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(array);
                using (var reader = new BinaryReader(stream))
                {
                    stream = null;
                    highScore.ReadFrom(reader);
                }
            }
            finally
            {
                stream?.Dispose();
            }

            return highScore;
        }

        public Th075HighScoreWrapper()
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest);
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj")]
        public Th075HighScoreWrapper(object obj)
            => this.pobj = new PrivateObject(obj);

        public object Target
            => this.pobj.Target;
        public string Name
            => this.pobj.GetProperty(nameof(Name)) as string;
        public byte? Month
            => this.pobj.GetProperty(nameof(Month)) as byte?;
        public byte? Day
            => this.pobj.GetProperty(nameof(Day)) as byte?;
        public int? Score
            => this.pobj.GetProperty(nameof(Score)) as int?;

        public void ReadFrom(BinaryReader reader)
            => this.pobj.Invoke(nameof(ReadFrom), new object[] { reader }, CultureInfo.InvariantCulture);
    }
}