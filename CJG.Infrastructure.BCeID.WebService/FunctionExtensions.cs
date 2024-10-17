using System;
using System.Threading;

namespace CJG.Infrastructure.BCeID.WebService
{
    /// <summary>
    /// <typeparamref name="FunctionExtensions"/> static class, provides extension methods for actions and functions.
    /// </summary>
    public static class FunctionExtensions
    {
        /// <summary>
        /// Executes the function with a separate thread and aborts if exceeds the specified timeout.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="timeout">In seconds</param>
        /// <returns></returns>
        public static T Execute<T>(this Func<T> func, int timeout)
        {
            T result;
            func.TryExecute(timeout, out result);
            return result;
        }

        /// <summary>
        /// Executes the function with a separate thread and aborts if exceeds the specified timeout.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="timeout">In seconds</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryExecute<T>(this Func<T> func, int timeout, out T result)
        {
            var t = default(T);
            var thread = new Thread(() => t = func());
            thread.Start();
            var completed = thread.Join(timeout * 1000);
            if (!completed) thread.Abort();
            result = t;
            return completed;
        }

        /// <summary>
        /// Executes the function with a separate thread and aborts if exceeds the specified timeout.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeout">In seconds</param>
        public static void Execute(this Action action, int timeout)
        {
            action.TryExecute(timeout);
        }

        /// <summary>
        /// Executes the function with a separate thread and aborts if exceeds the specified timeout.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="timeout">In seconds</param>
        /// <returns></returns>
        public static bool TryExecute(this Action action, int timeout)
        {
            var thread = new Thread(() => action());
            thread.Start();
            var completed = thread.Join(timeout * 1000);
            if (!completed) thread.Abort();
            return completed;
        }
    }
}
