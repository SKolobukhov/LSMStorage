using System;

namespace LSMStorage.Core
{
    internal static class Preconditions
    {
        public static void EnsureCondition(bool argumentCondition, string argumentName, string message)
        {
            if (!argumentCondition)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        public static void EnsureCondition(bool argumentCondition, string argumentName, string format = null, params object[] args)
        {
            if (!argumentCondition)
            {
                throw new ArgumentException(string.Format(format ?? string.Empty, args), argumentName);
            }
        }

        public static void EnsureNotNull<T>(T argument, string argumentName, string message)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }

        public static void EnsureNotNull<T>(T argument, string argumentName, string format = null, params object[] args)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName, string.Format(format ?? string.Empty, args));
            }
        }

        public static void EnsureArgumentRange(bool argumentRangeCondition, string argumentName, string message)
        {
            if (!argumentRangeCondition)
            {
                throw new ArgumentOutOfRangeException(argumentName, message);
            }
        }

        public static void EnsureArgumentRange(bool argumentRangeCondition, string argumentName, string format = null, params object[] args)
        {
            if (!argumentRangeCondition)
            {
                throw new ArgumentOutOfRangeException(argumentName, string.Format(format ?? string.Empty, args));
            }
        }
    }
}