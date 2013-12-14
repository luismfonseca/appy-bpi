using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", @"C:\Users\shuffle\Documents\APPY\appy-bpi\wcfserver\wcfserver\App_Data\");
            Thread.Sleep(2300);
            ServiceHost host = new ServiceHost(typeof(wcfserver.Service));

            host.Open();
            Console.WriteLine("Server is online");
            Console.ReadLine();
            host.Close();
        }
    }
}
