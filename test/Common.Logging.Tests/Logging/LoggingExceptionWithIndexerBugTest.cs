﻿#region License

/*
 * Copyright © 2002-2007 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Common.Logging
{
    /// <summary>
    /// Exception with indexer property should be logged without <see cref="TargetParameterCountException"/> error
    /// </summary>
    /// <author>artem1</author>
    /// <version>$Id:$</version>
    public class LoggingExceptionWithIndexerBugTest
    {
        /// <summary>
        /// This bug was found by me in Npgsql driver for PostgreSQL (NpgsqlException)
        /// </summary>
        [Test]
        public void ErrorNotThrownWhenLoggedExceptionHasIndexerProperty()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ILog log = LogManager.GetCurrentClassLogger();
#pragma warning restore CS0618 // Type or member is obsolete
            ExceptionWithIndexerException exception = new ExceptionWithIndexerException();
            Assert.That(() => log.Error("error catched", exception), Throws.Nothing);
        }
        
        [Serializable]
        public class ExceptionWithIndexerException : Exception
        {
            public ExceptionWithIndexerException()
            {
            }

            public ExceptionWithIndexerException(string message) : base(message)
            {
            }

            public ExceptionWithIndexerException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected ExceptionWithIndexerException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public string this[string key]
            {
                get { return null; }
            }
        }
    }
}