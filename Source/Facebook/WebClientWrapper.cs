
namespace Facebook
{
    using System;
    using System.Collections.Specialized;
    using System.Net;

    internal class WebClientWrapper : IWebClient
    {
        private WebClient webClient = new WebClient();

        public WebClientWrapper()
            : this(new WebClient())
        {
        }

        public WebClientWrapper(WebClient webClient)
        {
            this.webClient = webClient;
            this.webClient.DownloadDataCompleted +=
                (o, e) =>
                {
                    if (this.DownloadDataCompleted == null)
                    {
                        return;
                    }

                    if (e == null)
                    {
                        this.DownloadDataCompleted(o, null);
                    }
                    else
                    {
                        byte[] result = null;
                        var error = e.Error;

                        if (error == null)
                        {
                            result = e.Result;
                        }
                        else if (error is WebException)
                        {
                            error = new WebExceptionWrapper((WebException)error);
                        }

                        this.DownloadDataCompleted(o, new DownloadDataCompletedEventArgsWrapper(error, e.Cancelled, e.UserState, result));
                    }
                };
        }

        public WebHeaderCollection Headers
        {
            get { return this.webClient.Headers; }
            set { this.webClient.Headers = value; }
        }

        public NameValueCollection QueryString
        {
            get { return this.webClient.QueryString; }
            set { this.webClient.QueryString = value; }
        }

        public WebHeaderCollection ResponseHeaders
        {
            get { return this.webClient.ResponseHeaders; }
        }

        public IWebProxy Proxy
        {
            get { return this.webClient.Proxy; }
            set { this.webClient.Proxy = value; }
        }

        public byte[] DownloadData(Uri address)
        {
            try
            {
                return this.webClient.DownloadData(address);
            }
            catch (WebException webException)
            {
                throw new WebExceptionWrapper(webException);
            }
        }

        public byte[] UploadData(Uri address, string method, byte[] data)
        {
            try
            {
                return this.webClient.UploadData(address, method, data);
            }
            catch (WebException webException)
            {
                throw new WebExceptionWrapper(webException);
            }
        }

        public void DownloadDataAsync(Uri address, object userToken)
        {
            this.webClient.DownloadDataAsync(address, userToken);
        }

        public void UploadDataAsync(Uri address, string method, byte[] data, object userToken)
        {
            this.webClient.UploadDataAsync(address, method, data, userToken);
        }

        public void CancelAsync()
        {
            webClient.CancelAsync();
        }

        public Action<object, DownloadDataCompletedEventArgsWrapper> DownloadDataCompleted { get; set; }

        //private List<DownloadDataCompletedEventHandler> downloadDataCompletedDelegates = new List<DownloadDataCompletedEventHandler>();

        //public event DownloadDataCompletedEventHandler DownloadDataCompleted
        //{
        //    add
        //    {
        //        this.webClient.DownloadDataCompleted += value;
        //        this.downloadDataCompletedDelegates.Add(value);
        //    }

        //    remove
        //    {
        //        this.webClient.DownloadDataCompleted -= value;
        //        this.downloadDataCompletedDelegates.Remove(value);
        //    }
        //}

        //private void RemoveAllDownloadDataCompletedEventHandlers()
        //{
        //    foreach (var @delegate in this.downloadDataCompletedDelegates)
        //    {
        //        this.webClient.DownloadDataCompleted -= @delegate;
        //    }

        //    this.downloadDataCompletedDelegates.Clear();
        //}

        //public event UploadDataCompletedEventHandler UploadDataCompleted;

        public void Dispose()
        {
            webClient.Dispose();
        }
    }
}