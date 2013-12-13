/***
 * 
 * 
 * **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Dynamic;
using System.Reflection;

namespace Navigation
{
    public static class Extensions
    {
        public static List<Type> KnownTypes = new List<Type>();
        
        public static bool Navigate(this NavigationService navigationService, Uri source, params object[] objects)
        {
            string[] objectsInJson = new string[objects.Length];

            for (int i = 0; i < objects.Length; ++i)
            {
                var objectToSerialize = objects[i];

                // serialize object
                string objectSerialized = string.Empty;
                using (var stream = new MemoryStream())
                {
                    var jsonSerializer = new DataContractJsonSerializer(objectToSerialize.GetType(), KnownTypes);
                    jsonSerializer.WriteObject(stream, objectToSerialize);
                    
                    byte[] streamBytes = stream.ToArray();
                    objectSerialized = Encoding.UTF8.GetString(streamBytes, 0, streamBytes.Length);
                }

                // serialize keyValuePair to encapsulate the known type
                var objectToSerializeWithType =
                    new KeyValuePair<string, string>(objectToSerialize.GetType().AssemblyQualifiedName, objectSerialized);
                using (var stream = new MemoryStream())
                {
                    var jsonSerializer = new DataContractJsonSerializer(typeof(KeyValuePair<string, string>));
                    jsonSerializer.WriteObject(stream, objectToSerializeWithType);

                    byte[] streamBytes = stream.ToArray();
                    objectsInJson[i] = Encoding.UTF8.GetString(streamBytes, 0, streamBytes.Length);
                }

                // make it url safe
                objectsInJson[i] = HttpUtility.UrlEncode(objectsInJson[i]);
            }
            return navigationService.Navigate(new Uri(string.Format(source.OriginalString, objectsInJson), UriKind.Relative));
        }

        public static void Bind(this NavigationContext navigationContext, FrameworkElement view)
        {
            foreach (var keyAndValue in navigationContext.QueryString)
            {
                try
                {
                    var objectToDeserialize = keyAndValue.Value;

                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(KeyValuePair<string, string>));
                    var objectKeyValuePair = (KeyValuePair<string, string>)jsonSerializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(objectToDeserialize)));

                    var objectType = Type.GetType(objectKeyValuePair.Key, true);
                    DataContractJsonSerializer jsonSerializer2 = new DataContractJsonSerializer(objectType, KnownTypes);
                    var objectDeserialized = jsonSerializer2.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(objectKeyValuePair.Value)));

                    if (view.DataContext == null)
                    {
                        // in this case, we are assuming there's only 1 object passed and that it's the view model itself
                        view.DataContext = objectDeserialized;
                    }
                    else
                    {
                        PropertyInfo property = view.DataContext.GetType().GetProperty(keyAndValue.Key, BindingFlags.Public | BindingFlags.Instance);
                        if (property != null && property.CanWrite)
                        {
                            property.SetValue(view.DataContext, objectDeserialized, null);
                        }
                    }
                } catch
                {
                    // probably a normal parameter, real exceptions would have happenned previously anyhow
                }
            }
        }
    }
}
