﻿//-----------------------------------------------------------------------
// <copyright file="Th155Converter.cs" company="None">
//     (c) 2018 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable 1591

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ThScoreFileConverter.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    internal class Th155Converter : ThConverter
    {
        private static readonly new EnumShortNameParser<Level> LevelParser =
            new EnumShortNameParser<Level>();

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "For future use.")]
        private static readonly new EnumShortNameParser<LevelWithTotal> LevelWithTotalParser =
            new EnumShortNameParser<LevelWithTotal>();

        private static readonly EnumShortNameParser<StoryChara> StoryCharaParser =
            new EnumShortNameParser<StoryChara>();

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "For future use.")]
        private static readonly EnumShortNameParser<StoryCharaWithTotal> StoryCharaWithTotalParser =
            new EnumShortNameParser<StoryCharaWithTotal>();

        private AllScoreData allScoreData = null;

        public new enum Level
        {
            [EnumAltName("E")] Easy,
            [EnumAltName("N")] Normal,
            [EnumAltName("H")] Hard,
            [EnumAltName("L")] Lunatic,
            [EnumAltName("D")] OverDrive
        }

        public new enum LevelWithTotal
        {
            [EnumAltName("E")] Easy,
            [EnumAltName("N")] Normal,
            [EnumAltName("H")] Hard,
            [EnumAltName("L")] Lunatic,
            [EnumAltName("D")] OverDrive,
            [EnumAltName("T")] Total
        }

        [Flags]
        public enum LevelFlag
        {
            None = 0,
            Easy = 1,
            Normal = 2,
            Hard = 4,
            Lunatic = 8,
            OverDrive = 16
        }

        public enum StoryChara
        {
            [EnumAltName("RK")] ReimuKasen,
            [EnumAltName("MK")] MarisaKoishi,
            [EnumAltName("NK")] NitoriKokoro,
            [EnumAltName("MM")] MamizouMokou,
            [EnumAltName("MB")] MikoByakuren,
            [EnumAltName("FI")] FutoIchirin,
            [EnumAltName("RD")] ReisenDoremy,
            [EnumAltName("SD")] SumirekoDoremy,
            [EnumAltName("TS")] TenshiShinmyoumaru,
            [EnumAltName("YR")] YukariReimu,
            [EnumAltName("JS")] JoonShion
        }

        public enum StoryCharaWithTotal
        {
            [EnumAltName("RK")] ReimuKasen,
            [EnumAltName("MK")] MarisaKoishi,
            [EnumAltName("NK")] NitoriKokoro,
            [EnumAltName("MM")] MamizouMokou,
            [EnumAltName("MB")] MikoByakuren,
            [EnumAltName("FI")] FutoIchirin,
            [EnumAltName("RD")] ReisenDoremy,
            [EnumAltName("SD")] SumirekoDoremy,
            [EnumAltName("TS")] TenshiShinmyoumaru,
            [EnumAltName("YR")] YukariReimu,
            [EnumAltName("JS")] JoonShion,
            [EnumAltName("TL")] Total
        }

        public override string SupportedVersions
        {
            get { return "1.10c"; }
        }

        public override bool HasCardReplacer
        {
            get { return false; }
        }

        protected override bool ReadScoreFile(Stream input)
        {
#if DEBUG
            using (var decoded = new FileStream("th155decoded.dat", FileMode.Create, FileAccess.ReadWrite))
#else
            using (var decoded = new MemoryStream())
#endif
            {
                if (!Extract(input, decoded))
                    return false;

                decoded.Seek(0, SeekOrigin.Begin);
                this.allScoreData = Read(decoded);

                return this.allScoreData != null;
            }
        }

        protected override IEnumerable<IStringReplaceable> CreateReplacers(
            bool hideUntriedCards, string outputFilePath)
        {
            return new List<IStringReplaceable>
            {
                new ClearRankReplacer(this)
            };
        }

        private static bool Extract(Stream input, Stream output)
        {
            var succeeded = false;

            // See section 2.2 of RFC 1950
            var validHeader = new byte[] { 0x78, 0x9C };

            if (input.Length >= sizeof(int) + validHeader.Length)
            {
                var size = new byte[sizeof(int)];
                var header = new byte[validHeader.Length];

                input.Seek(0, SeekOrigin.Begin);
                input.Read(size, 0, size.Length);
                input.Read(header, 0, header.Length);

                if (Enumerable.SequenceEqual(header, validHeader))
                {
                    var extracted = new byte[0x80000];
                    var extractedSize = 0;

                    using (var deflate = new DeflateStream(input, CompressionMode.Decompress, true))
                        extractedSize = deflate.Read(extracted, 0, extracted.Length);

                    output.Seek(0, SeekOrigin.Begin);
                    output.Write(extracted, 0, extractedSize);

                    succeeded = true;
                }
                else
                {
                    // Invalid header
                }
            }
            else
            {
                // The input stream is too short
            }

            return succeeded;
        }

        private static AllScoreData Read(Stream input)
        {
            var reader = new BinaryReader(input);
            var allScoreData = new AllScoreData();

            try
            {
                allScoreData.ReadFrom(reader);
            }
            catch (EndOfStreamException)
            {
            }

            return allScoreData;
        }

        // %T155CLEAR[x][yy]
        private class ClearRankReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T155CLEAR({0})({1})", LevelParser.Pattern, StoryCharaParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ClearRankReplacer(Th155Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelParser.Parse(match.Groups[1].Value);
                    var chara = StoryCharaParser.Parse(match.Groups[2].Value);

                    LevelFlag toLevelFlag(Level lv)
                    {
                        switch (lv)
                        {
                            case Level.Easy:
                                return LevelFlag.Easy;
                            case Level.Normal:
                                return LevelFlag.Normal;
                            case Level.Hard:
                                return LevelFlag.Hard;
                            case Level.Lunatic:
                                return LevelFlag.Lunatic;
                            case Level.OverDrive:
                                return LevelFlag.OverDrive;
                            default:
                                return LevelFlag.None;
                        }
                    }

                    if (parent.allScoreData.StoryDictionary.TryGetValue(chara, out AllScoreData.Story story)
                        && story.available
                        && ((story.ed & toLevelFlag(level)) != LevelFlag.None))
                        return "Clear";
                    else
                        return "Not Clear";
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        private class AllScoreData : IBinaryReadable
        {
            private static readonly Func<BinaryReader, object> StringReader =
                reader =>
                {
                    var size = reader.ReadInt32();
                    return (size > 0) ? Encoding.Default.GetString(reader.ReadBytes(size)) : string.Empty;
                };

            private static readonly Func<BinaryReader, object> ArrayReader =
                reader =>
                {
                    var num = reader.ReadInt32();
                    if (num > 0)
                    {
                        var array = new object[num];
                        for (var count = 0; count < num; count++)
                        {
                            if (ReadObject(reader, out object index))
                            {
                                if (ReadObject(reader, out object value))
                                {
                                    if ((index is int) && ((int)index < num))
                                        array[(int)index] = value;
                                }
                            }
                        }

                        if (ReadObject(reader, out object endmark) && (endmark is EndMark))
                            return array;
                    }

                    return new object[] { };
                };

            private static readonly Func<BinaryReader, object> DictionaryReader =
                reader =>
                {
                    var dictionary = new Dictionary<object, object>();
                    while (true)
                    {
                        if (ReadObject(reader, out object key))
                        {
                            if (key is EndMark)
                                break;

                            if (ReadObject(reader, out object value))
                                dictionary.Add(key, value);
                        }
                        else
                            break;
                    }

                    return dictionary;
                };

            private static readonly Dictionary<uint, Func<BinaryReader, object>> ObjectReaders =
                new Dictionary<uint, Func<BinaryReader, object>>
                {
                    { 0x01000001, reader => new EndMark() },
                    { 0x01000008, reader => reader.ReadByte() == 0x01 },
                    { 0x05000002, reader => reader.ReadInt32() },
                    { 0x05000004, reader => reader.ReadSingle() },
                    { 0x08000010, StringReader },
                    { 0x08000040, ArrayReader },
                    { 0x08000100, reader => new OptionalMark() },
                    { 0x0A000020, DictionaryReader },
                    { 0x0A008000, reader => new object() }  // unknown (appears in gauge_1p/2p...)
                };

            private Dictionary<string, object> allData;

            public AllScoreData()
            {
                this.allData = null;
                this.StoryDictionary = null;
                this.BgmDictionary = null;
                this.EndingDictionary = null;
                this.StageDictionary = null;
            }

            public Dictionary<StoryChara, Story> StoryDictionary { get; private set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public Dictionary<string, int> CharacterDictionary { get; private set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public Dictionary<int, bool> BgmDictionary { get; private set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public Dictionary<string, int> EndingDictionary { get; private set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public Dictionary<int, int> StageDictionary { get; private set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public int Version { get; private set; }

            public static bool ReadObject(BinaryReader reader, out object obj)
            {
                var type = reader.ReadUInt32();

                obj = ObjectReaders.TryGetValue(type, out Func<BinaryReader, object> objectReader)
                    ? objectReader(reader) : null;

                return obj != null;
            }

            public void ReadFrom(BinaryReader reader)
            {
                if (DictionaryReader(reader) is Dictionary<object, object> dictionary)
                {
                    this.allData = dictionary
                        .Where(pair => pair.Key is string)
                        .ToDictionary(pair => pair.Key as string, pair => pair.Value);

                    this.ParseStoryDictionary();
                    this.ParseCharacterDictionary();
                    this.ParseBgmDictionary();
                    this.ParseEndingDictionary();
                    this.ParseStageDictionary();
                    this.ParseVersion();
                }
            }

            private void ParseStoryDictionary()
            {
                if (this.allData.TryGetValue("story", out object story))
                {
                    if (story is Dictionary<object, object> dict)
                    {
                        this.StoryDictionary = dict
                            .Where(pair => ParseStoryChara(pair.Key) != null)
                            .ToDictionary(
                                pair => ParseStoryChara(pair.Key).Value,
                                pair => ParseStory(pair.Value));
                    }
                }
            }

            private static StoryChara? ParseStoryChara(object obj)
            {
                if (obj is string str)
                {
                    switch (str)
                    {
                        case "reimu":
                            return StoryChara.ReimuKasen;
                        case "marisa":
                            return StoryChara.MarisaKoishi;
                        case "nitori":
                            return StoryChara.NitoriKokoro;
                        case "usami":
                            return StoryChara.SumirekoDoremy;
                        case "tenshi":
                            return StoryChara.TenshiShinmyoumaru;
                        case "miko":
                            return StoryChara.MikoByakuren;
                        case "yukari":
                            return StoryChara.YukariReimu;
                        case "mamizou":
                            return StoryChara.MamizouMokou;
                        case "udonge":
                            return StoryChara.ReisenDoremy;
                        case "futo":
                            return StoryChara.FutoIchirin;
                        case "jyoon":
                            return StoryChara.JoonShion;
                        default:
                            return null;
                    }
                }
                else
                    return null;
            }

            private static Story ParseStory(object obj)
            {
                var story = new Story();
                if (obj is Dictionary<object, object> dict)
                {
                    foreach (var pair in dict)
                    {
                        if (pair.Key is string key)
                        {
                            if ((key == "stage") && (pair.Value is int stage))
                                story.stage = stage;
                            if ((key == "ed") && (pair.Value is int ed))
                                story.ed = (LevelFlag)ed;
                            if ((key == "available") && (pair.Value is bool available))
                                story.available = available;
                            if ((key == "overdrive") && (pair.Value is int overDrive))
                                story.overDrive = overDrive;
                            if ((key == "stage_overdrive") && (pair.Value is int stageOverDrive))
                                story.stageOverDrive = stageOverDrive;
                        }
                    }
                }
                return story;
            }

            private void ParseCharacterDictionary()
            {
                if (this.allData.TryGetValue("character", out object character))
                {
                    if (character is Dictionary<object, object> dict)
                    {
                        this.CharacterDictionary = dict
                            .Where(pair => (pair.Key is string) && (pair.Value is int))
                            .ToDictionary(pair => (string)pair.Key, pair => (int)pair.Value);
                    }
                }
            }

            private void ParseBgmDictionary()
            {
                if (this.allData.TryGetValue("bgm", out object bgm))
                {
                    if (bgm is Dictionary<object, object> dict)
                    {
                        this.BgmDictionary = dict
                            .Where(pair => (pair.Key is int) && (pair.Value is bool))
                            .ToDictionary(pair => (int)pair.Key, pair => (bool)pair.Value);
                    }
                }
            }

            private void ParseEndingDictionary()
            {
                if (this.allData.TryGetValue("ed", out object ed))
                {
                    if (ed is Dictionary<object, object> dict)
                    {
                        this.EndingDictionary = dict
                            .Where(pair => (pair.Key is string) && (pair.Value is int))
                            .ToDictionary(pair => (string)pair.Key, pair => (int)pair.Value);
                    }
                }
            }

            private void ParseStageDictionary()
            {
                if (this.allData.TryGetValue("stage", out object stage))
                {
                    if (stage is Dictionary<object, object> dict)
                    {
                        this.StageDictionary = dict
                            .Where(pair => (pair.Key is int) && (pair.Value is int))
                            .ToDictionary(pair => (int)pair.Key, pair => (int)pair.Value);
                    }
                }
            }

            private void ParseVersion()
            {
                this.Version = this.GetValue<int>("version");
            }

            private T GetValue<T>(string key)
                where T : struct
                => (this.allData.TryGetValue(key, out object value) && (value is T)) ? (T)value : default(T);

            private class OptionalMark
            {
            }

            private class EndMark
            {
            }

            public struct Story
            {
                public int stage;
                public LevelFlag ed;
                public bool available;
                public int overDrive;
                public int stageOverDrive;
            }
        }
    }
}
