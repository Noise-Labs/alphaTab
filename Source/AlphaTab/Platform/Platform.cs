﻿/*
 * This file is part of alphaTab.
 * Copyright © 2018, Daniel Kuschny and Contributors, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or at your option any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using System;
using AlphaTab.Collections;
using AlphaTab.Xml;

namespace AlphaTab.Platform
{
    static partial class Platform
    {
        public static bool IsStringNumber(string s, bool allowSign = true)
        {
            if (s.Length == 0) return false;
            var c = s[0];
            return IsCharNumber(c, allowSign);
        }

        public static bool IsCharNumber(int c, bool allowSign = true)
        {
            return (allowSign && c == 0x2D) || (c >= 0x30 && c <= 0x39);
        }

        public static bool IsWhiteSpace(int c)
        {
            return c == 0x20 || c == 0x0B || c == 0x0D || c == 0x0A || c == 0x09;
        }

        public static bool IsAlmostEqualTo(this float a, float b)
        {
            return Math.Abs(a - b) < 0.00001f;
        }

        public static string ToHexString(int n, int digits = 0)
        {
            var s = "";
            const string hexChars = "0123456789ABCDEF";
            do
            {
                s = StringFromCharCode((int)hexChars[(n & 15)]) + s;
                n >>= 4;
            } while (n > 0);

            while (s.Length < digits)
            {
                s = "0" + s;
            }

            return s;
        }

        public static uint ToUInt32(int i)
        {
            return (uint)i;
        }

        public static short ToInt16(int i)
        {
            return (short)i;
        }

        public static ushort ToUInt16(int i)
        {
            return (ushort)i;
        }

        public static byte ToUInt8(int i)
        {
            return (byte)i;
        }


        private static string DetectEncoding(byte[] data)
        {
            if (data.Length > 2 && data[0] == 0xFE && data[1] == 0xFF)
            {
                return "utf-16be";
            }
            if (data.Length > 2 && data[0] == 0xFF && data[1] == 0xFE)
            {
                return "utf-16le";
            }
            if (data.Length > 4 && data[0] == 0x00 && data[1] == 0x00 && data[2] == 0xFE && data[3] == 0xFF)
            {
                return "utf-32be";
            }
            if (data.Length > 4 && data[0] == 0xFF && data[1] == 0xFE && data[2] == 0x00 && data[3] == 0x00)
            {
                return "utf-32le";
            }

            return null;
        }
    }
}
