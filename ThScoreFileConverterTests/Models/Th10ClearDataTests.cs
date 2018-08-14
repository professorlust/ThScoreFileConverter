﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models
{
    [TestClass]
    public class Th10ClearDataTests
    {
        internal struct Properties<TCharaWithTotal, TStageProgress>
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
        {
            public string signature;
            public ushort version;
            public uint checksum;
            public int size;
            public TCharaWithTotal chara;
            public Dictionary<ThConverter.Level, Th10ScoreDataTests.Properties<TStageProgress>[]> rankings;
            public int totalPlayCount;
            public int playTime;
            public Dictionary<ThConverter.Level, int> clearCounts;
            public Dictionary<
                Th10LevelStagePairTests.Properties<ThConverter.Level, ThConverter.Stage>,
                Th10PracticeTests.Properties> practices;
            public Dictionary<int, Th10SpellCardTests.Properties> cards;
        };

        internal static Properties<TCharaWithTotal, TStageProgress>
            GetValidProperties<TCharaWithTotal, TStageProgress>(ushort version, int size, int numCards)
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
        {
            var levels = Utils.GetEnumerator<ThConverter.Level>();
            var levelsExceptExtra = levels.Where(level => level != ThConverter.Level.Extra);
            var stages = Utils.GetEnumerator<ThConverter.Stage>();
            var stagesExceptExtra = stages.Where(stage => stage != ThConverter.Stage.Extra);

            return new Properties<TCharaWithTotal, TStageProgress>()
            {
                signature = "CR",
                version = version,
                checksum = 0u,
                size = size,
                chara = TestUtils.Cast<TCharaWithTotal>(1),
                rankings = levels.ToDictionary(
                    level => level,
                    level => Enumerable.Range(0, 10).Select(
                        index => new Th10ScoreDataTests.Properties<TStageProgress>()
                        {
                            score = 12345670u - (uint)index * 1000u,
                            stageProgress = TestUtils.Cast<TStageProgress>(5),
                            continueCount = (byte)index,
                            name = TestUtils.MakeRandomArray<byte>(10),
                            dateTime = 34567890u,
                            slowRate = 1.2f
                        }).ToArray()),
                totalPlayCount = 23,
                playTime = 4567890,
                clearCounts = levels.ToDictionary(level => level, level => 100 - (int)level),
                practices = levelsExceptExtra
                    .SelectMany(level => stagesExceptExtra.Select(stage => new { level, stage }))
                    .ToDictionary(
                        pair => new Th10LevelStagePairTests.Properties<ThConverter.Level, ThConverter.Stage>()
                        {
                            level = pair.level,
                            stage = pair.stage
                        },
                        pair => new Th10PracticeTests.Properties()
                        {
                            score = 123456u - (uint)pair.level * 10u,
                            stageFlag = (uint)pair.stage % 2u
                        }),
                cards = Enumerable.Range(1, numCards).ToDictionary(
                    index => index,
                    index => new Th10SpellCardTests.Properties()
                    {
                        name = TestUtils.MakeRandomArray<byte>(0x80),
                        clearCount = 123 + index,
                        trialCount = 456 + index,
                        id = index,
                        level = ThConverter.Level.Hard
                    })
            };
        }

        internal static byte[] MakeData<TParent, TCharaWithTotal, TStageProgress>(
            in Properties<TCharaWithTotal, TStageProgress> properties)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.MakeByteArray(
                TestUtils.Cast<int>(properties.chara),
                properties.rankings.Values.SelectMany(
                    ranking => ranking.SelectMany(
                        scoreData => Th10ScoreDataTests.MakeByteArray<TParent, TStageProgress>(scoreData))).ToArray(),
                properties.totalPlayCount,
                properties.playTime,
                properties.clearCounts.Values.ToArray(),
                properties.practices.Values.SelectMany(
                    practice => Th10PracticeTests.MakeByteArray(practice)).ToArray(),
                properties.cards.Values.SelectMany(
                    card => Th10SpellCardTests.MakeByteArray(card)).ToArray());

        internal static byte[] MakeByteArray<TParent, TCharaWithTotal, TStageProgress>(
            in Properties<TCharaWithTotal, TStageProgress> properties)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.MakeByteArray(
                properties.signature.ToCharArray(),
                properties.version,
                properties.checksum,
                properties.size,
                MakeData<TParent, TCharaWithTotal, TStageProgress>(properties));

        internal static void Validate<TParent, TCharaWithTotal, TStageProgress>(
            in Th10ClearDataWrapper<TParent, TCharaWithTotal, TStageProgress> clearData,
            in Properties<TCharaWithTotal, TStageProgress> properties)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
        {
            var data = MakeData<TParent, TCharaWithTotal, TStageProgress>(properties);

            Assert.AreEqual(properties.signature, clearData.Signature);
            Assert.AreEqual(properties.version, clearData.Version);
            Assert.AreEqual(properties.checksum, clearData.Checksum);
            Assert.AreEqual(properties.size, clearData.Size);
            CollectionAssert.AreEqual(data, clearData.Data.ToArray());
            Assert.AreEqual(properties.chara, clearData.Chara);

            foreach (var pair in properties.rankings)
            {
                for (var index = 0; index < pair.Value.Length; ++index)
                {
                    Th10ScoreDataTests.Validate(clearData.RankingItem(pair.Key, index), pair.Value[index]);
                }
            }

            Assert.AreEqual(properties.totalPlayCount, clearData.TotalPlayCount);
            Assert.AreEqual(properties.playTime, clearData.PlayTime);
            CollectionAssert.AreEqual(properties.clearCounts.Values, clearData.ClearCounts.Values.ToArray());

            foreach (var pair in properties.practices)
            {
                var levelStagePair = new Th10LevelStagePairWrapper<TParent, ThConverter.Level, ThConverter.Stage>(
                    pair.Key.level, pair.Key.stage);
                Th10PracticeTests.Validate(clearData.PracticesItem(levelStagePair), pair.Value);
            }

            foreach (var pair in properties.cards)
            {
                Th10SpellCardTests.Validate(clearData.CardsItem(pair.Key), pair.Value);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        internal static void ClearDataTestChapterHelper<TParent, TCharaWithTotal, TStageProgress>(
            ushort version, int size, int numCards)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties = GetValidProperties<TCharaWithTotal, TStageProgress>(version, size, numCards);

                var chapter = Th10ChapterWrapper<TParent>.Create(
                    MakeByteArray<TParent, TCharaWithTotal, TStageProgress>(properties));
                var clearData = new Th10ClearDataWrapper<TParent, TCharaWithTotal, TStageProgress>(chapter);

                Validate(clearData, properties);
                Assert.IsFalse(clearData.IsValid.Value);
            });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        internal static void ClearDataTestNullChapterHelper<TParent, TCharaWithTotal, TStageProgress>()
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var clearData = new Th10ClearDataWrapper<TParent, TCharaWithTotal, TStageProgress>(null);

                Assert.Fail(TestUtils.Unreachable);
            });

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        internal static void ClearDataTestInvalidSignatureHelper<TParent, TCharaWithTotal, TStageProgress>(
            ushort version, int size, int numCards)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties = GetValidProperties<TCharaWithTotal, TStageProgress>(version, size, numCards);
                properties.signature = properties.signature.ToLowerInvariant(); 

                var chapter = Th10ChapterWrapper<TParent>.Create(
                    MakeByteArray<TParent, TCharaWithTotal, TStageProgress>(properties));
                var clearData = new Th10ClearDataWrapper<TParent, TCharaWithTotal, TStageProgress>(chapter);

                Assert.Fail(TestUtils.Unreachable);
            });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        internal static void ClearDataTestInvalidVersionHelper<TParent, TCharaWithTotal, TStageProgress>(
            ushort version, int size, int numCards)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties = GetValidProperties<TCharaWithTotal, TStageProgress>(version, size, numCards);
                ++properties.version;

                var chapter = Th10ChapterWrapper<TParent>.Create(
                    MakeByteArray<TParent, TCharaWithTotal, TStageProgress>(properties));
                var clearData = new Th10ClearDataWrapper<TParent, TCharaWithTotal, TStageProgress>(chapter);

                Assert.Fail(TestUtils.Unreachable);
            });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        internal static void ClearDataTestInvalidSizeHelper<TParent, TCharaWithTotal, TStageProgress>(
            ushort version, int size, int numCards)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties = GetValidProperties<TCharaWithTotal, TStageProgress>(version, size, numCards);
                ++properties.size;

                var chapter = Th10ChapterWrapper<TParent>.Create(
                    MakeByteArray<TParent, TCharaWithTotal, TStageProgress>(properties));
                var clearData = new Th10ClearDataWrapper<TParent, TCharaWithTotal, TStageProgress>(chapter);

                Assert.Fail(TestUtils.Unreachable);
            });

        internal static void CanInitializeTestHelper<TParent, TCharaWithTotal, TStageProgress>(
            string signature, ushort version, int size, bool expected)
            where TParent : ThConverter
            where TCharaWithTotal : struct, Enum
            where TStageProgress : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var checksum = 0u;
                var data = new byte[size];

                var chapter = Th10ChapterWrapper<TParent>.Create(
                    TestUtils.MakeByteArray(signature.ToCharArray(), version, checksum, size, data));

                Assert.AreEqual(
                    expected, Th10ClearDataWrapper<TParent, TCharaWithTotal, TStageProgress>.CanInitialize(chapter));
            });

        #region Th10

        [TestMethod]
        public void Th10ClearDataTestChapter()
            => ClearDataTestChapterHelper<
                Th10Converter, Th10Converter.CharaWithTotal, Th10Converter.StageProgress>(0, 0x437C, 110);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th10ClearDataTestNullChapter()
            => ClearDataTestNullChapterHelper<
                Th10Converter, Th10Converter.CharaWithTotal, Th10Converter.StageProgress>();

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th10ClearDataTestInvalidSignature()
            => ClearDataTestInvalidSignatureHelper<
                Th10Converter, Th10Converter.CharaWithTotal, Th10Converter.StageProgress>(0, 0x437C, 110);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th10ClearDataTestInvalidVersion()
            => ClearDataTestInvalidVersionHelper<
                Th10Converter, Th10Converter.CharaWithTotal, Th10Converter.StageProgress>(0, 0x437C, 110);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th10ClearDataTestInvalidSize()
            => ClearDataTestInvalidSizeHelper<
                Th10Converter, Th10Converter.CharaWithTotal, Th10Converter.StageProgress>(0, 0x437C, 110);

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("CR", (ushort)0, 0x437C, true)]
        [DataRow("cr", (ushort)0, 0x437C, false)]
        [DataRow("CR", (ushort)1, 0x437C, false)]
        [DataRow("CR", (ushort)0, 0x437D, false)]
        public void Th10ClearDataCanInitializeTest(string signature, ushort version, int size, bool expected)
            => CanInitializeTestHelper<
                Th10Converter, Th10Converter.CharaWithTotal, Th10Converter.StageProgress>(
                signature, version, size, expected);

        #endregion

        #region Th11

        [TestMethod]
        public void Th11ClearDataTestChapter()
            => ClearDataTestChapterHelper<
                Th11Converter, Th11Converter.CharaWithTotal, Th11Converter.StageProgress>(0, 0x68D4, 175);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th11ClearDataTestNullChapter()
            => ClearDataTestNullChapterHelper<
                Th11Converter, Th11Converter.CharaWithTotal, Th11Converter.StageProgress>();

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th11ClearDataTestInvalidSignature()
            => ClearDataTestInvalidSignatureHelper<
                Th11Converter, Th11Converter.CharaWithTotal, Th11Converter.StageProgress>(0, 0x68D4, 175);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th11ClearDataTestInvalidVersion()
            => ClearDataTestInvalidVersionHelper<
                Th11Converter, Th11Converter.CharaWithTotal, Th11Converter.StageProgress>(0, 0x68D4, 175);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th11ClearDataTestInvalidSize()
            => ClearDataTestInvalidSizeHelper<
                Th11Converter, Th11Converter.CharaWithTotal, Th11Converter.StageProgress>(0, 0x68D4, 175);

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("CR", (ushort)0, 0x68D4, true)]
        [DataRow("cr", (ushort)0, 0x68D4, false)]
        [DataRow("CR", (ushort)1, 0x68D4, false)]
        [DataRow("CR", (ushort)0, 0x68D5, false)]
        public void Th11ClearDataCanInitializeTest(string signature, ushort version, int size, bool expected)
            => CanInitializeTestHelper<
                Th11Converter, Th11Converter.CharaWithTotal, Th11Converter.StageProgress>(
                signature, version, size, expected);

        #endregion

        #region Th12

        [TestMethod]
        public void Th12ClearDataTestChapter()
            => ClearDataTestChapterHelper<
                Th12Converter, Th12Converter.CharaWithTotal, Th12Converter.StageProgress>(2, 0x45F4, 113);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th12ClearDataTestNullChapter()
            => ClearDataTestNullChapterHelper<
                Th12Converter, Th12Converter.CharaWithTotal, Th12Converter.StageProgress>();

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th12ClearDataTestInvalidSignature()
            => ClearDataTestInvalidSignatureHelper<
                Th12Converter, Th12Converter.CharaWithTotal, Th12Converter.StageProgress>(2, 0x45F4, 113);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th12ClearDataTestInvalidVersion()
            => ClearDataTestInvalidVersionHelper<
                Th12Converter, Th12Converter.CharaWithTotal, Th12Converter.StageProgress>(2, 0x45F4, 113);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th12ClearDataTestInvalidSize()
            => ClearDataTestInvalidSizeHelper<
                Th12Converter, Th12Converter.CharaWithTotal, Th12Converter.StageProgress>(2, 0x45F4, 113);

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("CR", (ushort)2, 0x45F4, true)]
        [DataRow("cr", (ushort)2, 0x45F4, false)]
        [DataRow("CR", (ushort)1, 0x45F4, false)]
        [DataRow("CR", (ushort)2, 0x45F5, false)]
        public void Th12ClearDataCanInitializeTest(string signature, ushort version, int size, bool expected)
            => CanInitializeTestHelper<
                Th12Converter, Th12Converter.CharaWithTotal, Th12Converter.StageProgress>(
                signature, version, size, expected);

        #endregion
    }
}