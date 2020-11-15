using System.IO;
using System.Net;
using Translation;

namespace Translation.Web.Queries
{
    public abstract class Query<TInput, TOutput>
    {
        private readonly QueueTimer queryTimer;

        protected Query(QueueTimer queryTimer)
        {
            this.queryTimer = queryTimer;
        }

        public virtual TOutput Execute(TInput input)
        {
            string request = this.CreateRequest(input);
            string response = this.Get(request);
            return this.ParseResult(response);
        }

        protected abstract string CreateRequest(TInput input);

        protected abstract TOutput ParseResult(string response);

        private string Get(string uri)
        {
            this.queryTimer.WaitMyTurn();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using Stream stream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
