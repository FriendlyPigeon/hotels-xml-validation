using System.Net;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class Program
    {
        public static string xmlURL = "https://raw.githubusercontent.com/FriendlyPigeon/hotels-xml-validation/refs/heads/main/Assignment4/static/Hotels.xml";
        public static string xmlErrorURL = "https://raw.githubusercontent.com/FriendlyPigeon/hotels-xml-validation/refs/heads/main/Assignment4/static/HotelsErrors.xml";
        public static string xsdURL = "https://raw.githubusercontent.com/FriendlyPigeon/hotels-xml-validation/refs/heads/main/Assignment4/static/Hotels.xsd";

        public static void Main(string[] args)
        {
            // Working XML file that conforms to schema
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            // Non-working XML file that doesn't conform to schema
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            // Convert our valid XML to JSON
            result = Xml2Json(xmlURL);
            Console.WriteLine(result); 
        }

        public static string Verification(string xmlUrl, string xsdUrl)
        {
            // Bool to see if we encountered schema validation errors or not
            bool schemaErrors = false;
            string errors = "";

            XmlSchemaSet sc = new XmlSchemaSet();
            // Use the schema defined at the URL
            sc.Add(null, xsdUrl);
            XmlReaderSettings settings = new XmlReaderSettings();
            // Set the validation type to Schema
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;

            // Store each schema validation error
            settings.ValidationEventHandler += (sender, e) =>
            {
                schemaErrors = true;
                errors += $"Validation Error: {e.Message}\n";
            };

            try
            {
                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read()) { }
                }
            }
            catch (XmlException ex)
            {
                schemaErrors = true;
            }
            catch (Exception ex)
            {
                schemaErrors = true;
                errors += $"Other Error: {ex.Message}\n";
            }

            if (!schemaErrors)
                return "No Error";
            else
                return errors;
        }

        // Converts XML to JSON
        public static string Xml2Json(string xmlUrl)
        {
            using (var client = new WebClient())
            {
                string content = client.DownloadString(xmlUrl);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(content);
                string convertedJson = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);

                return convertedJson;
            }
        }
    }
}