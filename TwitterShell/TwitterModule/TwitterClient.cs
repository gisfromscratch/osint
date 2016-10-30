﻿/*
 * Copyright 2016 Jan Tschada
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace TwitterModule
{
    /// <summary>
    /// Represents the twitter client commandlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, @"TwitterClient")]
    public class TwitterClient : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        /// <summary>
        /// Parses the connection file for getting access to twitter.
        /// </summary>
        protected override void ProcessRecord()
        {
            const string ConnectionFile = @"twitter.con";
            try
            {
                using (var reader = new StreamReader(ConnectionFile))
                {

                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, TwitterErrors.TwitterConnectionError.ToString(), ErrorCategory.ConnectionError, ConnectionFile));
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
}
