using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartJukeBox
{
    public class Settings
    {
        private static IsolatedStorageSettings isolatedStorageSettings = IsolatedStorageSettings.ApplicationSettings;

        private static string userID = null;

        public static string UserID
        {
            get
            {
                if (userID == null)
                {
                    if (isolatedStorageSettings.Contains("UserID"))
                    {
                        userID = (string)isolatedStorageSettings["UserID"];
                    }
                    else
                    {
                        isolatedStorageSettings.Add("UserID", userID);
                    }
                }
                return userID;
            }
            set
            {
                if (UserID != value)
                {
                    userID = value;
                    isolatedStorageSettings["UserID"] = value;
                    isolatedStorageSettings.Save();
                }
            }
        }

    }
}
