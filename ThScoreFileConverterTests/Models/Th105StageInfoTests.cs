﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ThScoreFileConverter.Models;
using ThScoreFileConverterTests.Models.Wrappers;

namespace ThScoreFileConverterTests.Models
{
    [TestClass]
    public class Th105StageInfoTests
    {
        internal struct Properties<TStage, TChara>
            where TStage : struct, Enum
            where TChara : struct, Enum
        {
            public TStage stage;
            public TChara enemy;
            public List<int> cardIds;
        };

        internal static Properties<TStage, TChara> GetValidProperties<TStage, TChara>()
            where TStage : struct, Enum
            where TChara : struct, Enum
            => new Properties<TStage, TChara>()
            {
                stage = TestUtils.Cast<TStage>(1),
                enemy = TestUtils.Cast<TChara>(2),
                cardIds = new List<int>() { 3, 4, 5 }
            };

        internal static void Validate<TParent, TStage, TChara>(
            in Th105StageInfoWrapper<TParent, TStage, TChara> spellCardInfo, in Properties<TStage, TChara> properties)
            where TParent : ThConverter
            where TStage : struct, Enum
            where TChara : struct, Enum
        {
            Assert.AreEqual(properties.stage, spellCardInfo.Stage);
            Assert.AreEqual(properties.enemy, spellCardInfo.Enemy);
            CollectionAssert.AreEqual(properties.cardIds, spellCardInfo.CardIds.ToArray());
        }

        internal static void StageInfoTestHelper<TParent, TStage, TChara>()
            where TParent : ThConverter
            where TStage : struct, Enum
            where TChara : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties = GetValidProperties<TStage, TChara>();

                var spellCardInfo = new Th105StageInfoWrapper<TParent, TStage, TChara>(
                    properties.stage, properties.enemy, properties.cardIds);

                Validate(spellCardInfo, properties);
            });

        [TestMethod]
        public void Th105StageInfoTest()
            => StageInfoTestHelper<Th105Converter, Th105Converter.Stage, Th105Converter.Chara>();

        [TestMethod]
        public void Th123StageInfoTest()
            => StageInfoTestHelper<Th123Converter, Th123Converter.Stage, Th123Converter.Chara>();
    }
}
