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
using System.Collections.Concurrent;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TweetSharp;

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
            Track = @"osint";
            WaitTime = Timeout.Infinite;
        }

        [Parameter(Mandatory = false,
            HelpMessage = @"The path to the connection file.",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0)]
        [Alias(@"cf")]
        [ValidateNotNullOrEmpty]
        public string ConnectionFile { get; set; }

        [Parameter(Mandatory = false, HelpMessage = @"The track filter like aleppo,crimea,donbas.",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1)]
        [Alias(@"tag")]
        [ValidateNotNullOrEmpty]
        public string Track { get; set; }

        [Parameter(Mandatory = false, HelpMessage = @"The time to wait in [msec].",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 2)]
        [Alias(@"t")]
        [ValidateNotNullOrEmpty]
        public int WaitTime { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                // Connect and authenticate
                var connection = TwitterConnection.Create(ConnectionFile);
                var service = connection.Service;

                // Search twitter using await and IAsyncResult from the twitter API
                var errors = new BlockingCollection<TwitterError>();
                var statuses = new BlockingCollection<Tweet>();
                var resetEvent = new ManualResetEvent(false);
                Task.Factory.FromAsync(service.StreamFilterAndTrack(Track, (artifact, response) =>
                {
                    // TODO: There is always HTTP 0 returned!
                    if (null == response.Error)
                    {
                        response.StatusCode = HttpStatusCode.OK;
                    }

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            break;

                        default:
                            if (null != response.Error)
                            {
                                errors.Add(response.Error);
                            }
                            else
                            {
                                errors.Add(new TwitterError { Message = @"Streaming Twitter failed!" });
                            }

                            // Activate the event
                            resetEvent.Set();
                            return;
                    }

                    // Add the tweet
                    var streamStatus = artifact as TwitterUserStreamStatus;
                    if (null != streamStatus)
                    {
                        var status = streamStatus.Status;
                        statuses.Add(Tweet.Create(status));
                    }
                }), (response) =>
                {
                    // TODO: End action
                });

                // Wait for the event
                resetEvent.WaitOne(WaitTime);
                service.CancelStreaming();

                // Write the tweets to the pipeline
                WriteObject(statuses, true);

                // Check the errors
                if (errors.Any())
                {
                    WriteObject(errors, true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, TwitterErrors.TwitterConnectionError.ToString(), ErrorCategory.ConnectionError, ConnectionFile));
            }
        }
    }
}
