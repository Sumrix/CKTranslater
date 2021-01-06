using System;
using System.IO;
using System.Net;
using Core.Storages;

namespace Core.Web.Queries
{
    /// <summary>
    ///     Performs HTTP request to the website
    /// </summary>
    /// <typeparam name="TParam">Query parameter type</typeparam>
    /// <typeparam name="TResult">Query result data type</typeparam>
    public abstract class Query<TParam, TResult>
    {
        /// <summary>
        ///     Number of logging file.
        /// </summary>
        private static int logRecordIndex;
        /// <summary>
        ///     The path to the folder for logging.
        /// </summary>
        private readonly string logPath;
        /// <summary>
        ///     Timer for calling the request, so as not to overload the server.
        /// </summary>
        private readonly QueueTimer queryTimer;

        /// <summary>
        ///     Initializes query with <see cref="QueueTimer" /> and path to log.
        /// </summary>
        /// <param name="queryTimer">Timer for calling the request, so as not to overload the server.</param>
        /// <param name="logPath">The path to the folder for logging.</param>
        protected Query(QueueTimer queryTimer, string logPath = null)
        {
            this.queryTimer = queryTimer;
            this.logPath = logPath;
        }

        /// <summary>
        ///     Creates an HTTP request line.
        /// </summary>
        /// <param name="param">Parameters for creating a request.</param>
        /// <returns>HTTP request line.</returns>
        protected abstract string CreateRequest(TParam param);

        /// <summary>
        ///     Executes the request.
        /// </summary>
        /// <param name="param">Request parameters.</param>
        /// <returns>Query result.</returns>
        public virtual TResult Execute(TParam param)
        {
            string request = this.CreateRequest(param);

            // Cache response
            if (!DB.QueryCache.TryGetValue(request, out string response))
            {
                response = this.Get(request);
                DB.QueryCache[request] = response;
            }

            // Log query if it's necessary
            if (this.logPath != null)
            {
                this.Log(request, response);
            }

            return this.ParseResponse(response);
        }

        /// <summary>
        ///     Performs a Get request.
        /// </summary>
        /// <param name="uri">String with URI of request.</param>
        /// <returns>String with request result.</returns>
        private string Get(string uri)
        {
            this.queryTimer?.WaitMyTurn();

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using Stream stream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        ///     Logs the query.
        /// </summary>
        /// <param name="request">Request string.</param>
        /// <param name="response">Query response string.</param>
        private void Log(string request, string response)
        {
            string shortFileName =
                $"{DateTime.Now:yyyyMMddTHHmmss}I{Query<TParam, TResult>.logRecordIndex++:D8}{this.GetType().Name}.txt";
            string fileName = Path.Combine(this.logPath, shortFileName);

            if (!Directory.Exists(this.logPath))
            {
                Directory.CreateDirectory(this.logPath);
            }

            using StreamWriter stream = new StreamWriter(fileName);
            stream.WriteLine("REQUEST:");
            stream.WriteLine(request);
            stream.WriteLine();
            stream.WriteLine("RESPONSE:");
            string prettyResponse = JsonHelper.JsonPrettify(response);
            stream.WriteLine(prettyResponse);
        }

        /// <summary>
        ///     Parses request response string.
        /// </summary>
        /// <param name="response">Request response string.</param>
        /// <returns>Query result.</returns>
        protected abstract TResult ParseResponse(string response);
    }
}