﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace ThScoreFileConverter
{
    public static class Utils
    {
        /// <summary>
        /// 列挙型 T の全要素名を連結した文字列を生成する
        /// </summary>
        /// <typeparam Name="T">列挙型</typeparam>
        /// <param Name="separator">各要素名の間に挿入される区切り記号</param>
        /// <returns>連結後の文字列</returns>
        public static string JoinEnumNames<T>(string separator)
        {
            return string.Join(separator, Enum.GetNames(typeof(T)));
        }

        /// <summary>
        /// Enum.Parse() の Wrapper 関数
        /// </summary>
        /// <typeparam Name="T">列挙型</typeparam>
        /// <param Name="value">変換する名前または値が含まれている文字列</param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value, bool ignoreCase = false)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        /// T 型インスタンスの集合から合計値を算出する
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="set">T 型インスタンスの列挙可能な集合</param>
        /// <param name="converter">T 型インスタンスから int 型数値に変換するデリゲート</param>
        /// <returns>各 T 型インスタンスから変換した数値の合計値</returns>
        public static long Accumulate<T>(IEnumerable<T> set, Converter<T, int> converter)
        {
            long total = 0L;
            foreach (var element in set)
                total += converter(element);
            return total;
        }

        /// <summary>
        /// T 型インスタンスの集合から合計値を算出する
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="set">T 型インスタンスの列挙可能な集合</param>
        /// <param name="converter">T 型インスタンスから uint 型数値に変換するデリゲート</param>
        /// <returns>各 T 型インスタンスから変換した数値の合計値</returns>
        public static long Accumulate<T>(IEnumerable<T> set, Converter<T, uint> converter)
        {
            long total = 0L;
            foreach (var element in set)
                total += converter(element);
            return total;
        }

        /// <summary>
        /// T 型インスタンスの集合のうち条件を満たす要素数を返す
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="set">T 型インスタンスの列挙可能な集合</param>
        /// <param name="predicator">条件を表すデリゲート</param>
        /// <returns>条件を満たす T 型インスタンスの数</returns>
        public static int CountIf<T>(IEnumerable<T> set, Predicate<T> predicator)
        {
            int count = 0;
            foreach (var element in set)
                if (predicator(element))
                    count++;
            return count;
        }

        /// <summary>
        /// ItemCollection インスタンスから条件を満たす要素を返す
        /// </summary>
        /// <typeparam name="T">items の各要素をこの型と見なす</typeparam>
        /// <param name="items">ItemCollection インスタンス</param>
        /// <param name="predicator">条件を表すデリゲート</param>
        /// <returns>条件を満たす T 型インスタンス</returns>
        public static T Find<T>(ItemCollection items, Predicate<T> predicator)
        {
            foreach (T item in items)
                if (predicator(item))
                    return item;
            return default(T);
        }

        /// <summary>
        /// バイナリーからの読み込みが可能なクラス向けのインターフェース
        /// </summary>
        public interface IBinaryReadable
        {
            void ReadFrom(BinaryReader reader);
        }
    }
}
