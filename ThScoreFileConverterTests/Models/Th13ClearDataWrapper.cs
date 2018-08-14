﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models
{
    // NOTE: Setting the accessibility as public causes CS0050, CS0051 and CS0703.
    internal sealed class Th13ClearDataWrapper<
        TParent, TCharaWithTotal, TLevel, TLevelPractice, TLevelPracticeWithTotal, TStagePractice, TStageProgress>
        where TParent : ThConverter
        where TCharaWithTotal : struct, Enum
        where TLevel : struct, Enum
        where TLevelPractice : struct, Enum
        where TLevelPracticeWithTotal : struct, Enum
        where TStagePractice : struct, Enum
        where TStageProgress : struct, Enum
    {
        private static Type ParentType = typeof(TParent);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+ClearData";
        private static readonly PrivateType PrivateType = new PrivateType(AssemblyNameToTest, TypeNameToTest);

        private readonly PrivateObject pobj = null;

        public Th13ClearDataWrapper(Th10ChapterWrapper<TParent> chapter)
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest, new object[] { chapter?.Target });

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
        public TCharaWithTotal? Chara
            => this.pobj.GetProperty(nameof(Chara)) as TCharaWithTotal?;
        // NOTE: Th13Converter.ScoreData is a private class.
        // public IReadOnlyDictionary<LevelPracticeWithTotal, ScoreData[]> Rankings
        //     => this.pobj.GetProperty(nameof(Rankings)) as Dictionary<LevelPracticeWithTotal, ScoreData[]>;
        public object Rankings
            => this.pobj.GetProperty(nameof(Rankings));
        public object[] Ranking(TLevelPracticeWithTotal level)
            => this.Rankings.GetType().GetProperty("Item").GetValue(this.Rankings, new object[] { level }) as object[];
        public Th10ScoreDataWrapper<TParent, TStageProgress> RankingItem(TLevelPracticeWithTotal level, int index)
            => new Th10ScoreDataWrapper<TParent, TStageProgress>(this.Ranking(level)[index]);
        public int? TotalPlayCount
            => this.pobj.GetProperty(nameof(TotalPlayCount)) as int?;
        public int? PlayTime
            => this.pobj.GetProperty(nameof(PlayTime)) as int?;
        public IReadOnlyDictionary<TLevelPracticeWithTotal, int> ClearCounts
            => this.pobj.GetProperty(nameof(ClearCounts)) as Dictionary<TLevelPracticeWithTotal, int>;
        public IReadOnlyDictionary<TLevelPracticeWithTotal, int> ClearFlags
            => this.pobj.GetProperty(nameof(ClearFlags)) as Dictionary<TLevelPracticeWithTotal, int>;
        // NOTE: Th13Converter.{LevelStagePair,Practice} are private classes.
        // public IReadOnlyDictionary<LevelStagePair, Practice> Practices
        //     => this.pobj.GetProperty(nameof(Practices)) as Dictionary<LevelStagePair, Practice>;
        public object Practices
            => this.pobj.GetProperty(nameof(Practices));
        public Th13PracticeWrapper<TParent> PracticesItem(
            Th10LevelStagePairWrapper<TParent, TLevelPractice, TStagePractice> levelStagePair)
            => new Th13PracticeWrapper<TParent>(
                this.Practices.GetType().GetProperty("Item").GetValue(
                    this.Practices, new object[] { levelStagePair.Target }));
        // NOTE: Th13Converter.SpellCard is a private class.
        // public IReadOnlyDictionary<int, SpellCard> Cards
        //     => this.pobj.GetProperty(nameof(Cards)) as Dictionary<int, SpellCard>;
        public object Cards
            => this.pobj.GetProperty(nameof(Cards));
        public Th13SpellCardWrapper<TParent, TLevel> CardsItem(int id)
            => new Th13SpellCardWrapper<TParent, TLevel>(
                this.Cards.GetType().GetProperty("Item").GetValue(this.Cards, new object[] { id }));

        public static bool CanInitialize(Th10ChapterWrapper<TParent> chapter)
            => (bool)PrivateType.InvokeStatic(
                nameof(CanInitialize), new object[] { chapter.Target }, CultureInfo.InvariantCulture);
    }
}