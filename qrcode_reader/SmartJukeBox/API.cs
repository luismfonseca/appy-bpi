using SmartJukeBox.JsonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartJukeBox
{
    public class API
    {
        public const string BASE_URL = "http://172.30.66.49:9999/Service.svc/";

        public enum Actions
        {
            Register,
            Search,
            SetSpot
        }

        private static Uri getUrlFromAction(Actions action, params string[] parameters)
        {
            switch (action)
            {
                case Actions.Register:
                    return new Uri(string.Format(BASE_URL + "User"));
                case Actions.Search:
                    return new Uri(string.Format("http://ws.audioscrobbler.com/2.0/?method=artist.search&artist={0}&api_key=615e15c4504fd183e1d6d6f3ae40f753&format=json", parameters));
                case Actions.SetSpot:
                    return new Uri(string.Format(BASE_URL + "User/{0}/SetSpot/{1}", parameters));
                default:
                    throw new NotSupportedException("Unkown API.Actions.");
            }
        }
        public static async Task<T> GetAsync<T>(Actions action, params string[] parameters)
        {
            return await API.GetAsync<T>(action, null, parameters);
        }

        public static async Task<T> GetAsync<T>(Actions action, Json postObject, params string[] parameters)
        {
            Uri uri = getUrlFromAction(action, parameters);

            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(uri);
            request.AllowReadStreamBuffering = true;

            if (postObject != null)
            {
                request.ContentType = "application/json";
                request.Method = "POST";
                var taskGetRequestStream = Task<Stream>.Factory.FromAsync(
                    new Func<AsyncCallback, object, IAsyncResult>(request.BeginGetRequestStream),
                    new Func<IAsyncResult, Stream>(request.EndGetRequestStream), TaskCreationOptions.HideScheduler);

                using (var postStream = await taskGetRequestStream)
                {
                    string jsonObject = postObject.ToJsonString();
                    byte[] byteArray = Encoding.UTF8.GetBytes(jsonObject);

                    // Write to the request stream.
                    postStream.Write(byteArray, 0, jsonObject.Length);
                    postStream.Close();
                }
            }

            var task = Task<WebResponse>.Factory.FromAsync(
                new Func<AsyncCallback, object, IAsyncResult>(request.BeginGetResponse),
                new Func<IAsyncResult, WebResponse>(request.EndGetResponse), TaskCreationOptions.HideScheduler);
            var response = await task;

            using (Stream streamResponse = response.GetResponseStream())
            using (StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8))
            {
                var responseString = streamRead.ReadToEnd();
                responseString = responseString.Replace("#text", "text");

                DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(T));
                return (T)dcjs.ReadObject(getMemoryStream(responseString));
            }
        }

        private static MemoryStream getMemoryStream(string json)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
            return new MemoryStream(bodyBytes);
        }
    }
}
