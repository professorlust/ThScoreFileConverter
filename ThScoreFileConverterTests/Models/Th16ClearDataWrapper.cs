﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models
{
    // NOTE: Setting the accessibility as public causes CS0050, CS0051 and CS0053.
    internal sealed class Th16ClearDataWrapper
    {
        private static Type ParentType = typeof(Th16Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+ClearData";
        private static readonly PrivateType PrivateType = new PrivateType(AssemblyNameToTest, TypeNameToTest);

        private readonly PrivateObject pobj = null;

        public Th16ClearDataWrapper(Th10ChapterWrapper<Th16Converter> chapter)
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest, new object[] { chapter?.Target });

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public object Target
            => this.pobj.Target;
        public string Signature
            => this.pobj.GetProperty(nameof(Signature)) as string;
        public ushort? Version
            => this.pobj.GetProperty(nameof(Version)) as ushort?;
        public uint? Checksum
            => this.pobj.GetProperty(nameof(Checksum)) as uint?;
        public int? Size
            => this.pobj.GetProperty(nameof(Size)) as int?;
        public bool? IsValid
            => this.pobj.GetProperty(nameof(IsValid)) as bool?;
        public IReadOnlyCollection<byte> Data
            => this.pobj.GetProperty(nameof(Data)) as byte[];
        public Th16Converter.CharaWithTotal? Chara
            => this.pobj.GetProperty(nameof(Chara)) as Th16Converter.CharaWithTotal?;
        // NOTE: Th16Converter.ScoreData is a private class.
        // public IReadOnlyDictionary<LevelWithTotal, ScoreData[]> Rankings
        //     => this.pobj.GetProperty(nameof(Rankings)) as Dictionary<LevelWithTotal, ScoreData[]>;
        public object Rankings
            => this.pobj.GetProperty(nameof(Rankings));
        public object[] Ranking(ThConverter.LevelWithTotal level)
            => this.Rankings.GetType().GetProperty("Item").GetValue(this.Rankings, new object[] { level }) as object[];
        public Th16ScoreDataWrapper RankingItem(ThConverter.LevelWithTotal level, int index)
            => new Th16ScoreDataWrapper(this.Ranking(level)[index]);
        public int? TotalPlayCount
            => this.pobj.GetProperty(nameof(TotalPlayCount)) as int?;
        public int? PlayTime
            => this.pobj.GetProperty(nameof(PlayTime)) as int?;
        public IReadOnlyDictionary<ThConverter.LevelWithTotal, int> ClearCounts
            => this.pobj.GetProperty(nameof(ClearCounts)) as Dictionary<ThConverter.LevelWithTotal, int>;
        public IReadOnlyDictionary<ThConverter.LevelWithTotal, int> ClearFlags
            => this.pobj.GetProperty(nameof(ClearFlags)) as Dictionary<ThConverter.LevelWithTotal, int>;
        // NOTE: Th16Converter.{LevelStagePair,Practice} are private classes.
        // public IReadOnlyDictionary<LevelStagePair, Practice> Practices
        //     => this.pobj.GetProperty(nameof(Practices)) as Dictionary<LevelStagePair, Practice>;
        public object Practices
            => this.pobj.GetProperty(nameof(Practices));
        public Th13PracticeWrapper<Th16Converter> PracticesItem(
            Th10LevelStagePairWrapper<Th16Converter, ThConverter.Level, Th16Converter.StagePractice> levelStagePair)
            => new Th13PracticeWrapper<Th16Converter>(
                this.Practices.GetType().GetProperty("Item").GetValue(
                    this.Practices, new object[] { levelStagePair.Target }));
        // NOTE: Th16Converter.SpellCard is a private class.
        // public IReadOnlyDictionary<int, SpellCard> Cards
        //     => this.pobj.GetProperty(nameof(Cards)) as Dictionary<int, SpellCard>;
        public object Cards
            => this.pobj.GetProperty(nameof(Cards));
        public Th13SpellCardWrapper<Th16Converter, ThConverter.Level> CardsItem(int id)
            => new Th13SpellCardWrapper<Th16Converter, ThConverter.Level>(
                this.Cards.GetType().GetProperty("Item").GetValue(this.Cards, new object[] { id }));

        public static bool CanInitialize(Th10ChapterWrapper<Th16Converter> chapter)
            => (bool)PrivateType.InvokeStatic(
                nameof(CanInitialize), new object[] { chapter.Target }, CultureInfo.InvariantCulture);
    }
}