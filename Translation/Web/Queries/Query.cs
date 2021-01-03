using System;
using System.IO;
using System.Net;

namespace Translation.Web.Queries
{
    public abstract class Query<TInput, TOutput>
    {
        /// <summary>
        ///     Таймер вызова запроса, что бы не слишком загружать сервер.
        /// </summary>
        private readonly QueueTimer queryTimer;

        /// <summary>
        ///     Путь к папке для логгирования
        /// </summary>
        private readonly string logPath;

        protected Query(QueueTimer queryTimer, string logPath = null)
        {
            this.queryTimer = queryTimer;
            this.logPath = logPath;
        }

        public virtual TOutput Execute(TInput input)
        {
            string request = this.CreateRequest(input);
            string response = this.Get(request);
            if (this.logPath != null)
            {
                this.Log(request, response);
            }

            return this.ParseResult(response);
        }

        private void Log(string request, string response)
        {
            string shortFileName = $"{DateTime.Now:yyyyMMddTHHmmss}{this.GetType().Name}.txt";
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

        protected abstract string CreateRequest(TInput input);

        protected abstract TOutput ParseResult(string response);

        private string Get(string uri)
        {
            this.queryTimer.WaitMyTurn();

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using Stream stream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}