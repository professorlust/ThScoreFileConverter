﻿//-----------------------------------------------------------------------
// <copyright file="Pair.cs" company="None">
//     (c) 2013-2014 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleCop.CSharp.LayoutRules",
    "SA1503:CurlyBracketsMustNotBeOmitted",
    Justification = "Reviewed.")]

namespace ThScoreFileConverter
{
    /// <summary>
    /// Pair of instances.
    /// </summary>
    /// <typeparam name="TFirst">The type of the <see cref="First"/> property.</typeparam>
    /// <typeparam name="TSecond">The type of the <see cref="Second"/> property.</typeparam>
    public class Pair<TFirst, TSecond>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pair{TFirst,TSecond}"/> class.
        /// </summary>
        /// <param name="first">The value of the first component of the pair.</param>
        /// <param name="second">The value of the second component of the pair.</param>
        protected Pair(TFirst first, TSecond second)
        {
            this.First = first;
            this.Second = second;
        }

        /// <summary>
        /// Gets the value of the first component of the pair.
        /// </summary>
        protected TFirst First { get; private set; }

        /// <summary>
        /// Gets the value of the second component of the pair.
        /// </summary>
        protected TSecond Second { get; private set; }

        /// <summary>
        /// Determines whether the specified instance is equal to the current instance.
        /// </summary>
        /// <param name="obj">The instance to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is equal to <c>this</c>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(this, obj as Pair<TFirst, TSecond>);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            return this.First.GetHashCode() ^ this.Second.GetHashCode();
        }

        /// <summary>
        /// Determines whether two instances are equal.
        /// </summary>
        /// <param name="lhs">The left-hand-side instance.</param>
        /// <param name="rhs">The right-hand-side instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        private static bool Equals(Pair<TFirst, TSecond> lhs, Pair<TFirst, TSecond> rhs)
        {
            if (object.ReferenceEquals(lhs, null))
                return object.ReferenceEquals(rhs, null);
            else
            {
                if (object.ReferenceEquals(lhs, rhs))
                    return true;
                else
                    return lhs.First.Equals(rhs.First) && lhs.Second.Equals(rhs.Second);
            }
        }
    }
}
