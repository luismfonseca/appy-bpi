using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wcfserver
{
    public class Database
    {
        public static readonly String ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|Database.mdf;Integrated Security=True;User Instance=True";
    }

    #region User
    public partial class User
    {
        public static List<User> GetAll()
        {
            using (var db = new DataClassesDataContext(Database.ConnectionString))
            {
                return db.Users.ToList();
            }
        }

        public static bool Exists(string email)
        {
            using (var db = new DataClassesDataContext(Database.ConnectionString))
            {
                return db.Users.Any(passenger => passenger.email.Equals(email));
            }
        }

        public static bool Register(User userToAdd)
        {
            try
            {
                if (User.Exists(userToAdd.email))
                {
                    return false;
                }
                userToAdd.guid = Guid.NewGuid();
                userToAdd.password = Util.Md5String(userToAdd.password);

                using (var db = new DataClassesDataContext(Database.ConnectionString))
                {
                    db.Users.InsertOnSubmit(userToAdd);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
    #endregion
}