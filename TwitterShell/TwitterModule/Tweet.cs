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

        public ITweeter Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public int FavoriteCount { get; set; }

        public long Id { get; set; }

        public string IdStr { get; set; }

        public TwitterGeoLocation Location { get; set; }

        public TwitterPlace Place { get; set; }

        public TwitterStatus QuotedStatus { get; set; }

        public int RetweetCount { get; set; }

        public TwitterStatus RetweetStatus { get; set; }

        public string FullText { get; set; }

        public string Text { get; set; }

        internal static Tweet Create(TwitterStatus status)
        {
            var tweet = new Tweet();
            tweet.Author = status.Author;
            tweet.CreatedDate = status.CreatedDate;
            tweet.FavoriteCount = status.FavoriteCount;
            tweet.Id = status.Id;
            tweet.IdStr = status.IdStr;
            tweet.Location = status.Location;
            tweet.Place = status.Place;
            tweet.QuotedStatus = status.QuotedStatus;
            tweet.RetweetCount = status.RetweetCount;
            tweet.RetweetStatus = status.RetweetedStatus;
            tweet.FullText = status.FullText;
            tweet.Text = status.Text;
            return tweet;
        }
    }
}
