using System.IO;
using TweetSharp;

namespace TwitterModule
{
    /// <summary>
    /// Represents a twitter connection.
    /// </summary>
    internal class TwitterConnection
    {
        private readonly TwitterService _service;

        private TwitterConnection(TwitterService service)
        {
            _service = service;
        }

        internal TwitterService Service
        {
            get { return _service; }
        }

        /// <summary>
        /// Create a new connection using the specified connection file.
        /// </summary>
        /// <param name="filePath">Path to the connection file.</param>
        /// <returns>A new connection.</returns>
        /// <exception cref="FileFormatException"></exception>
        internal static TwitterConnection Create(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var line = reader.ReadLine();
                var tokens = line.Split('|');
                if (4 != tokens.Length)
                {
                    throw new FileFormatException($@"'{new FileInfo(filePath).FullName}' is not a valid connection file!");
                }

                string apiKey = tokens[0];
                string apiKeySecret = tokens[1];
                string accessToken = tokens[2];
                string accessTokenSecret = tokens[3];

                // Connect to twitter
                var service = new TwitterService(apiKey, apiKeySecret);
                service.AuthenticateWith(accessToken, accessTokenSecret);
                return new TwitterConnection(service);
            }
        }
    }
}
