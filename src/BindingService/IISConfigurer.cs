using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BindingServiceLib
{
    public class IISConfigurer:IIISConfigurer
    {
        public bool AddBinding(string domainName)
        {
            var siteName = System.Configuration.ConfigurationManager.AppSettings["webSiteName"];
            using (ServerManager serverManager = new ServerManager())
            {
                Microsoft.Web.Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
                Microsoft.Web.Administration.ConfigurationSection sitesSection = config.GetSection("system.applicationHost/sites");
                Microsoft.Web.Administration.ConfigurationElementCollection sitesCollection = sitesSection.GetCollection();
                Microsoft.Web.Administration.ConfigurationElement siteElement = FindElement(sitesCollection, "site", "name", siteName);

                if (siteElement == null) throw new InvalidOperationException("Element not found!");

                Microsoft.Web.Administration.ConfigurationElementCollection bindingsCollection = siteElement.GetCollection("bindings");

                Microsoft.Web.Administration.ConfigurationElement bindingElement = bindingsCollection.CreateElement("binding");
                bindingElement["protocol"] = @"http";
                bindingElement["bindingInformation"] = @"*:80:" + domainName;
                bindingsCollection.Add(bindingElement);

                Microsoft.Web.Administration.ConfigurationElement bindingElement1 = bindingsCollection.CreateElement("binding");
                bindingElement1["protocol"] = @"http";
                bindingElement1["bindingInformation"] = @"*:80:www." + domainName;
                bindingsCollection.Add(bindingElement1);

                serverManager.CommitChanges();
            }

            return true;
        }

        public bool DeleteBinding(string domainName)
        {
            var siteName = System.Configuration.ConfigurationManager.AppSettings["webSiteName"];
            using (ServerManager serverManager = new ServerManager())
            {
                Microsoft.Web.Administration.Configuration config = serverManager.GetApplicationHostConfiguration();
                Microsoft.Web.Administration.ConfigurationSection sitesSection = config.GetSection("system.applicationHost/sites");
                Microsoft.Web.Administration.ConfigurationElementCollection sitesCollection = sitesSection.GetCollection();
                Microsoft.Web.Administration.ConfigurationElement siteElement = FindElement(sitesCollection, "site", "name", siteName);

                if (siteElement == null) throw new InvalidOperationException("Element not found!");

                Microsoft.Web.Administration.ConfigurationElementCollection bindingsCollection = siteElement.GetCollection("bindings");

                var binding = FindElement(bindingsCollection, "binding", "bindingInformation", "*:80:" + domainName);
                if (binding == null) throw new InvalidOperationException("Binding not found!");
                bindingsCollection.Remove(binding);

                var binding1 = FindElement(bindingsCollection, "binding", "bindingInformation", "*:80:www." + domainName);
                if (binding1 == null) throw new InvalidOperationException("Binding not found!");
                bindingsCollection.Remove(binding1);

                serverManager.CommitChanges();
            }

            return true;
        }

        private Microsoft.Web.Administration.ConfigurationElement FindElement(Microsoft.Web.Administration.ConfigurationElementCollection collection, string elementTagName, params string[] keyValues)
        {
            foreach (Microsoft.Web.Administration.ConfigurationElement element in collection)
            {
                if (String.Equals(element.ElementTagName, elementTagName, StringComparison.OrdinalIgnoreCase))
                {
                    bool matches = true;
                    for (int i = 0; i < keyValues.Length; i += 2)
                    {
                        object o = element.GetAttributeValue(keyValues[i]);
                        string value = null;
                        if (o != null)
                        {
                            value = o.ToString();
                        }
                        if (!String.Equals(value, keyValues[i + 1], StringComparison.OrdinalIgnoreCase))
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        return element;
                    }
                }
            }
            return null;
        }

        private void LogError(string error, string path)
        {
            using (StreamWriter swr = new StreamWriter(path))
            {
                swr.WriteLine(error);
                swr.Close();
            }
        }

        public string AddSubdominNSRecord(string subdomainName)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            string nsHostAddress = System.Configuration.ConfigurationManager.AppSettings["nsAddress"];
            string nsHostUserName = System.Configuration.ConfigurationManager.AppSettings["nsUserName"];
            string nsHostPassword = System.Configuration.ConfigurationManager.AppSettings["nsPassword"];
            string address = System.Configuration.ConfigurationManager.AppSettings["siteAddress"];
            string logpath = System.Configuration.ConfigurationManager.AppSettings["logfile"];
            var webClient = new WebClient();

            try
            {
                string domainResult = webClient.DownloadString(String.Format("{0}/manager/ispmgr/core?authinfo={1}:{2}&out=xml&func=domain.sublist.edit&name={3}&plid=tradebel.com&sdtype=A&sok=ok&addr={4}",
                    nsHostAddress, nsHostUserName, nsHostPassword, subdomainName, address));
            
            }catch(WebException ex)
            {
                LogError(ex.Message, logpath);
                return "Connection error occured" + ex.Message;
            }

            string subdomains;
            try
            {
                subdomains = webClient.DownloadString(String.Format("{0}/manager/ispmgr/core?authinfo={1}:{2}&out=xml&func=domain.sublist&elid=tradebel.com",
                    nsHostAddress, nsHostUserName, nsHostPassword));
            }catch(WebException)
            {
                return "Connection error occured please check domain aviability by hand";
            }
            var doc = XDocument.Parse(subdomains);
            var domain = from p in doc.Descendants("name")
                         where p.Value == subdomainName
                         select p;
            if (domain != null && domain.Count() > 0)
                return "Domain added succesfully";

            return "Failed to add domain";
        }

        public string DeleteSubdomain(string domainName)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            string nsHostAddress = System.Configuration.ConfigurationManager.AppSettings["nsAddress"];
            string nsHostUserName = System.Configuration.ConfigurationManager.AppSettings["nsUserName"];
            string nsHostPassword = System.Configuration.ConfigurationManager.AppSettings["nsPassword"];
            string address = System.Configuration.ConfigurationManager.AppSettings["siteAddress"];
            string logpath = System.Configuration.ConfigurationManager.AppSettings["logfile"];
            var webClient = new WebClient();

            string subdomains;
            try
            {
                subdomains = webClient.DownloadString(String.Format("{0}/manager/ispmgr/core?authinfo={1}:{2}&out=xml&func=domain.sublist&elid=tradebel.com",
                    nsHostAddress, nsHostUserName, nsHostPassword));
            }
            catch (WebException ex)
            {
                LogError(ex.Message, logpath);
                return "Connection error occured" + ex.Message; 
            }
            var doc = XDocument.Parse(subdomains);
            var domain = from p in doc.Descendants("name")
                         where p.Value == domainName
                         select p;
            if (domain != null && domain.Count() > 0)
            {
                try
                {
                    string deleteResult = webClient.DownloadString(String.Format("{0}/manager/ispmgr/core?authinfo={1}:{2}&out=xml&func=domain.sublist.delete&elid={3}+A++{4}&plid=tradebel.com",
                        nsHostAddress, nsHostUserName, nsHostPassword, domainName, address));
                }
                catch (WebException)
                {
                    return "Connection error occured"; 
                }

                try
                {
                    subdomains = webClient.DownloadString(String.Format("{0}/manager/ispmgr/core?authinfo={1}:{2}&out=xml&func=domain.sublist&elid=tradebel.com",
                        nsHostAddress, nsHostUserName, nsHostPassword));
                }
                catch (WebException)
                {
                    return "Connection error occured check domain delition by hand";
                }

                doc = XDocument.Parse(subdomains);
                var deletedDomain = from p in doc.Descendants("name")
                             where p.Value == domainName
                             select p;

                if (deletedDomain != null && deletedDomain.Count() > 0)
                {
                    return "Unknown api error domain was not deleted";
                }

                return "Domain deleted succesfully";
            }

            return "There is no such domain in domain list";
        }
    }
}
