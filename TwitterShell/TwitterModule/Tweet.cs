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
using TweetSharp;

namespace TwitterModule
{
    /// <summary>
    /// Represents a simple tweet used for writing a <see cref="TwitterStatus"/> to the pipeline.
    /// </summary>
    public class Tweet
    {
        public Tweet()
        {
        }

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public int FavoriteCount { get; set; }

        public long Id { get; set; }

        public TwitterGeoLocation Location { get; set; }

        public string Place { get; set; }

        public string QuotedStatus { get; set; }

        public int RetweetCount { get; set; }

        public string RetweetStatus { get; set; }

        public string FullText { get; set; }

        public string Text { get; set; }

        internal static Tweet Create(TwitterStatus status)
        {
            var tweet = new Tweet();
            if (null != status.Author)
            {
                tweet.Author = status.Author.ScreenName;
            }
            tweet.CreatedDate = status.CreatedDate;
            tweet.FavoriteCount = status.FavoriteCount;
            tweet.Id = status.Id;
            tweet.Location = status.Location;
            if (null != status.Place)
            {
                tweet.Place = status.Place.Name;
            }
            if (null != status.QuotedStatus)
            {
                tweet.QuotedStatus = status.QuotedStatus.Text;
            }
            tweet.RetweetCount = status.RetweetCount;
            if (null != status.RetweetedStatus)
            {
                tweet.RetweetStatus = status.RetweetedStatus.Text;
            }
            tweet.FullText = status.FullText;
            tweet.Text = status.Text;
            return tweet;
        }
    }
}
