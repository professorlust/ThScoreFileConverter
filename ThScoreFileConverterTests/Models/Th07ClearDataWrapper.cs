﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ThScoreFileConverter.Models.Tests
{
    // NOTE: Setting the accessibility as public causes CS0051 and CS0053.
    internal sealed class Th07ClearDataWrapper
    {
        private static Type ParentType = typeof(Th07Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+ClearData";

        private readonly PrivateObject pobj = null;

        public Th07ClearDataWrapper(Th06ChapterWrapper<Th07Converter> chapter)
        {
            if (chapter == null)
            {
                var ch = new Th06ChapterWrapper<Th07Converter>();
                this.pobj = new PrivateObject(
                    AssemblyNameToTest,
                    TypeNameToTest,
                    new Type[] { ch.Target.GetType() },
                    new object[] { null });
            }
            else
            {
                this.pobj = new PrivateObject(
                    AssemblyNameToTest,
                    TypeNameToTest,
                    new Type[] { chapter.Target.GetType() },
                    new object[] { chapter.Target });
            }
        }

        // NOTE: Enabling the following causes CA1811.
        // public object Target => this.pobj.Target;

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
        public IReadOnlyDictionary<Th07Converter.Level, byte> StoryFlags
            => this.pobj.GetProperty(nameof(StoryFlags)) as Dictionary<Th07Converter.Level, byte>;
        public IReadOnlyDictionary<Th07Converter.Level, byte> PracticeFlags
            => this.pobj.GetProperty(nameof(PracticeFlags)) as Dictionary<Th07Converter.Level, byte>;
        public Th07Converter.Chara? Chara
            => this.pobj.GetProperty(nameof(Chara)) as Th07Converter.Chara?;
    }
}