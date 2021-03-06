// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace PSDocs.Azure.Pipeline
{
    /// <summary>
    /// A base class for all pipeline exceptions.
    /// </summary>
    public abstract class PipelineException : Exception
    {
        /// <summary>
        /// Creates a pipeline exception.
        /// </summary>
        protected PipelineException()
        {
        }

        /// <summary>
        /// Creates a pipeline exception.
        /// </summary>
        /// <param name="message">The detail of the exception.</param>
        protected PipelineException(string message)
            : base(message) { }

        /// <summary>
        /// Creates a pipeline exception.
        /// </summary>
        /// <param name="message">The detail of the exception.</param>
        /// <param name="innerException">A nested exception that caused the issue.</param>
        protected PipelineException(string message, Exception innerException)
            : base(message, innerException) { }

        protected PipelineException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// An exception related to reading templates.
    /// </summary>
    [Serializable]
    public sealed class TemplateReadException : PipelineException
    {
        /// <summary>
        /// Creates a template read exception.
        /// </summary>
        public TemplateReadException()
        {
        }

        /// <summary>
        /// Creates a template read exception.
        /// </summary>
        /// <param name="message">The detail of the exception.</param>
        public TemplateReadException(string message)
            : base(message) { }

        /// <summary>
        /// Creates a template read exception.
        /// </summary>
        /// <param name="message">The detail of the exception.</param>
        /// <param name="innerException">A nested exception that caused the issue.</param>
        public TemplateReadException(string message, Exception innerException)
            : base(message, innerException) { }

        public TemplateReadException(string message, Exception innerException, string templateFile, string parameterFile)
            : this(message, innerException)
        {
            TemplateFile = templateFile;
            ParameterFile = parameterFile;
        }

        private TemplateReadException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public string TemplateFile { get; }

        public string ParameterFile { get; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
        }
    }
}
