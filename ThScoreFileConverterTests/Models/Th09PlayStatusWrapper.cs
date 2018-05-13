﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ThScoreFileConverter.Models.Tests
{
    // NOTE: Setting the accessibility as public causes CS0051 and CS0053.
    internal sealed class Th09PlayStatusWrapper
    {
        private static Type ParentType = typeof(Th09Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+PlayStatus";

        private readonly PrivateObject pobj = null;

        public Th09PlayStatusWrapper(Th06ChapterWrapper<Th09Converter> chapter)
        {
            if (chapter == null)
            {
                var ch = new Th06ChapterWrapper<Th09Converter>();
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
        public Time TotalRunningTime
            => this.pobj.GetProperty(nameof(TotalRunningTime)) as Time;
        public Time TotalPlayTime
            => this.pobj.GetProperty(nameof(TotalPlayTime)) as Time;
        public IReadOnlyCollection<byte> BgmFlags
            => this.pobj.GetProperty(nameof(BgmFlags)) as byte[];
        public IReadOnlyDictionary<Th09Converter.Chara, byte> MatchFlags
            => this.pobj.GetProperty(nameof(MatchFlags)) as Dictionary<Th09Converter.Chara, byte>;
        public IReadOnlyDictionary<Th09Converter.Chara, byte> StoryFlags
            => this.pobj.GetProperty(nameof(StoryFlags)) as Dictionary<Th09Converter.Chara, byte>;
        public IReadOnlyDictionary<Th09Converter.Chara, byte> ExtraFlags
            => this.pobj.GetProperty(nameof(ExtraFlags)) as Dictionary<Th09Converter.Chara, byte>;
        // NOTE: Th09Converter.ClearCount is a private class.
        // public IReadOnlyDictionary<Th09Converter.Chara, ClearCount> ClearCounts
        //     => this.pobj.GetProperty(nameof(ClearCounts)) as Dictionary<Th09Converter.Chara, ClearCount>;
        public object ClearCounts
            => this.pobj.GetProperty(nameof(ClearCounts));
        public Th09ClearCountWrapper ClearCountsItem(Th09Converter.Chara chara)
            => new Th09ClearCountWrapper(
                this.ClearCounts.GetType().GetProperty("Item").GetValue(this.ClearCounts, new object[] { chara }));
    }
}