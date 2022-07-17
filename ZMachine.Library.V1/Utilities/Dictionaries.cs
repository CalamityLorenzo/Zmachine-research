﻿namespace ZMachine.Library.V1
{
    static class ZCharDictionaries
    {
        public static Dictionary<char, int> A2V3 = new()
        {
            { '\r', 7 },
            { '0', 8 },
            { '1', 9 },
            { '2', 10 },
            { '3', 11 },
            { '4', 12 },
            { '5', 13 },
            { '6', 14 },
            { '7', 15 },
            { '8', 16 },
            { '9', 17 },
            { '.', 18 },
            { ',', 19 },
            { '!', 20 },
            { '?', 21 },
            { '_', 22 },
            { '#', 23 },
            { '\'', 24 },
            { '\"', 25 },
            { '/', 26 },
            { '\\', 27 },
            { '-', 28 },
            { ':', 29 },
            { '(', 30 },
            { ')', 31 },
        };

        public static Dictionary<char, int> A2V1 = new()
        {
            { '0', 7 },
            { '1', 8 },
            { '2', 9 },
            { '3', 10 },
            { '4', 11 },
            { '5', 12 },
            { '6', 13 },
            { '7', 14 },
            { '8', 15 },
            { '9', 16 },
            { '.', 17 },
            { ',', 18 },
            { '!', 19 },
            { '?', 20 },
            { '_', 21 },
            { '#', 22 },
            { '\'', 23 },
            { '\"', 24 },
            { '/', 25 },
            { '\\', 26 },
            { '<', 27 },
            { '-', 28 },
            { ':', 29 },
            { '(', 30 },
            { ')', 31 },
        };

        public static Dictionary<char, int> A0Encode = new()
        {
            { 'a', 6 },
            { 'b', 7 },
            { 'c', 8 },
            { 'd', 9 },
            { 'e', 10 },
            { 'f', 11 },
            { 'g', 12 },
            { 'h', 13 },
            { 'i', 14 },
            { 'j', 15 },
            { 'k', 16 },
            { 'l', 17 },
            { 'm', 18 },
            { 'n', 19 },
            { 'o', 20 },
            { 'p', 21 },
            { 'q', 22 },
            { 'r', 23 },
            { 's', 24 },
            { 't', 25 },
            { 'u', 26 },
            { 'v', 27 },
            { 'w', 28 },
            { 'x', 29 },
            { 'y', 30 },
            { 'z', 31 },

        };

        public static Dictionary<byte, char> A0Decode = new()
        {
            { 0, ' ' },
            { 6, 'a' },
            { 7, 'b' },
            { 8, 'c' },
            { 9, 'd' },
            { 10, 'e' },
            { 11, 'f' },
            { 12, 'g' },
            { 13, 'h' },
            { 14, 'i' },
            { 15, 'j' },
            { 16, 'k' },
            { 17, 'l' },
            { 18, 'm' },
            { 19, 'n' },
            { 20, 'o' },
            { 21, 'p' },
            { 22, 'q' },
            { 23, 'r' },
            { 24, 's' },
            { 25, 't' },
            { 26, 'u' },
            { 27, 'v' },
            { 28, 'w' },
            { 29, 'x' },
            { 30, 'y' },
            { 31, 'z' },
        };

        public static Dictionary<char, int> A1Encode = new()
        {
            { 'A', 6 },
            { 'B', 7 },
            { 'C', 8 },
            { 'D', 9 },
            { 'E', 10 },
            { 'F', 11 },
            { 'G', 12 },
            { 'H', 13 },
            { 'I', 14 },
            { 'J', 15 },
            { 'K', 16 },
            { 'L', 17 },
            { 'M', 18 },
            { 'N', 19 },
            { 'O', 20 },
            { 'P', 21 },
            { 'Q', 22 },
            { 'R', 23 },
            { 'S', 24 },
            { 'T', 25 },
            { 'U', 26 },
            { 'V', 27 },
            { 'W', 28 },
            { 'X', 29 },
            { 'Y', 30 },
            { 'Z', 31 },

        };

        public static Dictionary<byte, char> A1Decode = new()
        {
            { 0, ' ' },
            { 6, 'A' },
            { 7, 'B' },
            { 8, 'C' },
            { 9, 'D' },
            { 10, 'E' },
            { 11, 'F' },
            { 12, 'G' },
            { 13, 'H' },
            { 14, 'I' },
            { 15, 'J' },
            { 16, 'K' },
            { 17, 'L' },
            { 18, 'M' },
            { 19, 'N' },
            { 20, 'O' },
            { 21, 'P' },
            { 22, 'Q' },
            { 23, 'R' },
            { 24, 'S' },
            { 25, 'T' },
            { 26, 'U' },
            { 27, 'V' },
            { 28, 'W' },
            { 29, 'X' },
            { 30, 'Y' },
            { 31, 'Z' },

        };

        public static Dictionary<byte, char> A2V3Decode = new()
        {
            { 7, '\r' },
            { 8, '0' },
            { 9, '1' },
            { 10, '2' },
            { 11, '3' },
            { 12, '4' },
            { 13, '5' },
            { 14, '6' },
            { 15, '7' },
            { 16, '8' },
            { 17, '9' },
            { 18, '.' },
            { 19, ',' },
            { 20, '!' },
            { 21, '?' },
            { 22, '_' },
            { 23, '#' },
            { 24, '\'' },
            { 25, '\"' },
            { 26, '/' },
            { 27, '\\' },
            { 28, '-' },
            { 29, ':' },
            { 30, '(' },
            { 31, ')' },
        };

        public static Dictionary<byte, char> A2V1Decode = new()
        {
            { 6, '\r' },
            { 7, '0' },
            { 8, '1' },
            { 9, '2' },
            { 10, '3' },
            { 11, '4' },
            { 12, '5' },
            { 13, '6' },
            { 14, '7' },
            { 15, '8' },
            { 16, '9' },
            { 17, '.' },
            { 18, ',' },
            { 19, '!' },
            { 20, '?' },
            { 21, '_' },
            { 22, '#' },
            { 23, '\'' },
            { 24, '\"' },
            { 25, '/' },
            { 26, '\\' },
            { 27, '<' },
            { 28, '-' },
            { 29, ':' },
            { 30, '(' },
            { 31, ')' },
        };
    }
}
