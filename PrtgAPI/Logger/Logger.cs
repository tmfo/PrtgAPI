﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace PrtgAPI
{
    [ExcludeFromCodeCoverage]
    internal class Logger
    {
#pragma warning disable 649
        private static bool debugEnabled;

        internal static bool DebugEnabled
        {
            get { return debugEnabled; }
            set
            {
#if DEBUG
                debugEnabled = value;
#else
                throw new NotSupportedException("Logging is not supported in Release builds");
#endif
            }
        }

#pragma warning restore 649

        internal static void Debug(string message, bool newLine = true)
        {
            if (DebugEnabled)
            {
                if (newLine)
                    System.Diagnostics.Debug.WriteLine(message);
                else
                    System.Diagnostics.Debug.Write(message);
            }            
        }
    }
}
