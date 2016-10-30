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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TweetSharp;

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
                    var line = reader.ReadLine();
                    var tokens = line.Split('|');
                    if (4 != tokens.Length)
                    {
                        throw new FileFormatException($@"'{new FileInfo(ConnectionFile).FullName}' is not a valid connection file!");
                    }

                    string apiKey = tokens[0];
                    string apiKeySecret = tokens[1];
                    string accessToken = tokens[2];
                    string accessTokenSecret = tokens[3];
                    
                    // Connect to twitter
                    var service = new TwitterService(apiKey, apiKeySecret);
                    service.AuthenticateWith(accessToken, accessTokenSecret);

                    // Search twitter using await and IAsyncResult from the twitter API
                    var searchOptions = new SearchOptions();
                    searchOptions.Q = @"#osint";
                    searchOptions.Count = 100;
                    var messages = new List<string>(searchOptions.Count.Value);
                    Task.Factory.FromAsync(service.Search(searchOptions, (tweets, response) =>
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                break;

                            default:
                                if (null != response.Error)
                                {
                                    throw new WebException(response.Error.Message);
                                }
                                throw new WebException(@"Querying Twitter failed!");
                        }

                        foreach (var status in tweets.Statuses)
                        {
                            messages.Add(status.Text);
                        }
                    }), (result) =>
                    {
                        // TODO: End action
                    }).Wait();

                    foreach (var message in messages)
                    {
                        WriteObject(message);
                    }
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
