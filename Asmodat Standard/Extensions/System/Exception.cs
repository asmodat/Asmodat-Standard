using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace AsmodatStandard.Extensions
{
    public static class ExceptionEx
    {
        /// <summary>
        /// The throw if critical.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public static void ThrowIfCritical(this System.Exception ex)
        {
            if (ex is OutOfMemoryException || ex is ThreadAbortException || ex is StackOverflowException)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Iterates the message path.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The entire collection of messages.</returns>
        public static string ShowAllMessages(this System.Exception ex)
        {
            // Validate
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            if (ex.InnerException != null)
            {
                return string.Format(CultureInfo.CurrentUICulture, "{0} : {1}", ex.Message, ex.InnerException.ShowAllMessages());
            }

            return ex.Message;
        }
    }
}
