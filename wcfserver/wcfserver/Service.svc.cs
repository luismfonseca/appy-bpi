using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;

namespace wcfserver
{
    public class Service : IService
    {
        public string GetData(string value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public bool UserRegister(User user)
        {
            if (User.Register(user))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
                return true;
            }
            else
            {
                //WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Conflict;
                return false;
            }
        }
    }
}
