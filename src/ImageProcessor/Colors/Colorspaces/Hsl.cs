﻿// <copyright file="Hsl.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageProcessor
{
    using System;
    using System.ComponentModel;
    using System.Numerics;

    /// <summary>
    /// Represents a Hsl (hue, saturation, lightness) color.
    /// </summary>
    public struct Hsl : IEquatable<Hsl>
    {
        /// <summary>
        /// Represents a <see cref="Hsl"/> that has H, S, and L values set to zero.
        /// </summary>
        public static readonly Hsl Empty = default(Hsl);

        /// <summary>
        /// The epsilon for comparing floating point numbers.
        /// </summary>
        private const float Epsilon = 0.0001f;

        /// <summary>
        /// The backing vector for SIMD support.
        /// </summary>
        private Vector3 backingVector;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hsl"/> struct.
        /// </summary>
        /// <param name="h">The h hue component.</param>
        /// <param name="s">The s saturation component.</param>
        /// <param name="l">The l value (lightness) component.</param>
        public Hsl(float h, float s, float l)
        {
            this.backingVector.X = h.Clamp(0, 360);
            this.backingVector.Y = s.Clamp(0, 1);
            this.backingVector.Z = l.Clamp(0, 1);
        }

        /// <summary>
        /// Gets the hue component.
        /// <remarks>A value ranging between 0 and 360.</remarks>
        /// </summary>
        public float H => this.backingVector.X;

        /// <summary>
        /// Gets the saturation component.
        /// <remarks>A value ranging between 0 and 1.</remarks>
        /// </summary>
        public float S => this.backingVector.Y;

        /// <summary>
        /// Gets the lightness component.
        /// <remarks>A value ranging between 0 and 1.</remarks>
        /// </summary>
        public float L => this.backingVector.Z;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Hsl"/> is empty.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEmpty => this.backingVector.Equals(default(Vector3));

        /// <summary>
        /// Allows the implicit conversion of an instance of <see cref="Color"/> to a
        /// <see cref="Hsl"/>.
        /// </summary>
        /// <param name="color">The instance of <see cref="Color"/> to convert.</param>
        /// <returns>
        /// An instance of <see cref="Hsl"/>.
        /// </returns>
        public static implicit operator Hsl(Color color)
        {
            color = Color.ToNonPremultiplied(color.Limited);
            float r = color.R;
            float g = color.G;
            float b = color.B;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float chroma = max - min;
            float h = 0;
            float s = 0;
            float l = (max + min) / 2;

            if (Math.Abs(chroma) < Epsilon)
            {
                return new Hsl(0, s, l);
            }

            if (Math.Abs(r - max) < Epsilon)
            {
                h = (g - b) / chroma;
            }
            else if (Math.Abs(g - max) < Epsilon)
            {
                h = 2 + ((b - r) / chroma);
            }
            else if (Math.Abs(b - max) < Epsilon)
            {
                h = 4 + ((r - g) / chroma);
            }

            h *= 60;
            if (h < 0.0)
            {
                h += 360;
            }

            if (l <= .5f)
            {
                s = chroma / (max + min);
            }
            else {
                s = chroma / (2 - chroma);
            }

            return new Hsl(h, s, l);
        }

        /// <summary>
        /// Compares two <see cref="Hsl"/> objects for equality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Hsl"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Hsl"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator ==(Hsl left, Hsl right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Hsl"/> objects for inequality.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Hsl"/> on the left side of the operand.
        /// </param>
        /// <param name="right">
        /// The <see cref="Hsl"/> on the right side of the operand.
        /// </param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        public static bool operator !=(Hsl left, Hsl right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Hsl)
            {
                Hsl color = (Hsl)obj;

                return this.backingVector == color.backingVector;
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.IsEmpty)
            {
                return "Hsl [ Empty ]";
            }

            return $"Hsl [ H={this.H:#0.##}, S={this.S:#0.##}, L={this.L:#0.##} ]";
        }

        /// <inheritdoc/>
        public bool Equals(Hsl other)
        {
            return this.backingVector.Equals(other.backingVector);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <param name="color">
        /// The instance of <see cref="Hsl"/> to return the hash code for.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        private static int GetHashCode(Hsl color) => color.backingVector.GetHashCode();
    }
}
