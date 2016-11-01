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
using System.Management.Automation;
using System.Net;
using System.Threading.Tasks;
using TweetSharp;

namespace TwitterModule
{
    /// <summary>
    /// Represents the twitter search commandlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, @"TwitterSearch")]
    [Alias(@"tws")]
    public class TwitterSearch : PSCmdlet
    {
        public TwitterSearch()
        {
            ConnectionFile = @"twitter.con";
            Query = @"#osint";
            Limit = 5;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        [Parameter(Mandatory = false, 
            HelpMessage = @"The path to the connection file.",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 0)]
        [Alias(@"cf")]
        [ValidateNotNullOrEmpty]
        public string ConnectionFile { get; set; }

        [Parameter(Mandatory = false, HelpMessage = @"The query tags like #osint.",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 1)]
        [Alias(@"q")]
        [ValidateNotNullOrEmpty]
        public string Query { get; set; }

        [Alias(@"l")]
        [Parameter(Mandatory = false, HelpMessage = @"The maximum number of tweets.",
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            Position = 2)]
        [ValidateNotNullOrEmpty]
        public int Limit { get; set; }

        /// <summary>
        /// Parses the connection file for getting access to twitter.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                // Connect and authenticate
                var connection = TwitterConnection.Create(ConnectionFile);
                var service = connection.Service;

                // Search twitter using await and IAsyncResult from the twitter API
                var searchOptions = new SearchOptions();
                searchOptions.Q = Query;
                searchOptions.Count = Limit;
                var statuses = new BlockingCollection<Tweet>(searchOptions.Count.Value);
                Task.Factory.FromAsync(service.Search(searchOptions, (tweets, response) =>
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
                            throw new WebException(@"Querying Twitter failed!");
                    }

                    // Add the tweets
                    foreach (var status in tweets.Statuses)
                    {
                        statuses.Add(Tweet.Create(status));
                    }
                }), (result) =>
                {
                    // TODO: End action
                }).Wait();

                // Write the tweets to the pipeline
                WriteObject(statuses, true);
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
