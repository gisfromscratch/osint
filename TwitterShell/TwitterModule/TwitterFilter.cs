/*
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
using System.Management.Automation;
using System.Net;
using System.Threading.Tasks;

namespace TwitterModule
{
    /// <summary>
    /// Represents the twitter filter commandlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, @"TwitterFilter")]
    [Alias(@"twf")]
    public class TwitterFilter : PSCmdlet
    {
        public TwitterFilter()
        {
            ConnectionFile = @"twitter.con";
        }

        [Parameter(Mandatory = false,
            HelpMessage = @"The path to the connection file.",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0)]
        [Alias(@"cf")]
        [ValidateNotNullOrEmpty]
        public string ConnectionFile { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                // Connect and authenticate
                var connection = TwitterConnection.Create(ConnectionFile);
                var service = connection.Service;

                // Search twitter using await and IAsyncResult from the twitter API
                Task.Factory.FromAsync(service.StreamFilter((artifact, response) =>
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            break;

                        default:
                            // TODO: Delegate Exception back to the pipeline thread
                            if (null != response.Error)
                            {
                                throw new WebException(response.Error.Message);
                            }
                            throw new WebException(@"Streaming Twitter failed!");
                    }
                }), (response) =>
                {
                    // TODO: End action
                }).Wait();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, TwitterErrors.TwitterConnectionError.ToString(), ErrorCategory.ConnectionError, ConnectionFile));
            }
        }
    }
}
