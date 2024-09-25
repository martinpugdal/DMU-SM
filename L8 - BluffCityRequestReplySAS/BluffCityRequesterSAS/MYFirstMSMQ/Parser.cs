using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

namespace BluffCityInformationCenterRouter
{
    class ParserETA
    {

        public string formaterETA(Message message)
        {
            string label = message.Label;

            Console.WriteLine(label);

            message.Formatter = new System.Messaging.XmlMessageFormatter(new String[] { });
            StreamReader sr = new StreamReader(message.BodyStream);
            string eta = "";
            while (sr.Peek() >= 0)
            {
                eta += sr.ReadLine();
            }

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(eta);
            XmlNode itemNode = xml.SelectSingleNode("string");
            var passengertargetNode = xml.ImportNode(itemNode, true);

            if (itemNode != null)
            {
                XmlNode value = itemNode.SelectSingleNode("//string");
                string text = value.InnerText;
                Console.WriteLine(text);
                return text;
            }
            else
            {
                return "Ukendt fly";
            }
        }
            }
        }