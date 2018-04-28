using System;

// ReSharper disable once CheckNamespace
namespace LibSvmDotNet
{

    /// <summary>
    /// The exception is general exception for LIBSVM. This class cannot be inherited.
    /// </summary>
    public sealed class LibSvmException : Exception
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LibSvmException"/> class.
        /// </summary>
        public LibSvmException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibSvmException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public LibSvmException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibSvmException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The name of the parameter that caused the current exception.</param>
        public LibSvmException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion

    }

}
