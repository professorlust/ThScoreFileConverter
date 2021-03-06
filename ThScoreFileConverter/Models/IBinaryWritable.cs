﻿//-----------------------------------------------------------------------
// <copyright file="IBinaryWritable.cs" company="None">
//     (c) 2014-2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ThScoreFileConverter.Models
{
    using System.IO;

    /// <summary>
    /// Defines a method to write to a binary stream.
    /// </summary>
    public interface IBinaryWritable
    {
        /// <summary>
        /// Writes to a stream by using the specified <see cref="BinaryWriter"/> instance.
        /// </summary>
        /// <param name="writer">The instance to use.</param>
        void WriteTo(BinaryWriter writer);
    }
}
