﻿using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("quotes")]
    public class Quotes
    {
        [XmlElement("quote")]
        public _Quote[] QuotesArray
        {
            get;
            set;
        }
    }
}