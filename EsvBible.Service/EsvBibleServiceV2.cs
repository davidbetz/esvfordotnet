using System;
using System.Collections.Generic;
using System.Xml;
using EsvBible.Service.Parameter;
using EsvBible.Service.Setting;
using Nalarium.Xml;

namespace EsvBible.Service
{
    /// <summary>
    ///     Proxy for Esv Bible Web Service API (Version 2)
    /// </summary>
    public class EsvBibleServiceV2
    {
        private OutputFormat _outputFormat = OutputFormat.Html;
        private IOutputSetting _settings;

        /// <summary>
        ///     Proxy for Esv Bible Web Service API (Version 2)
        /// </summary>
        public EsvBibleServiceV2()
        {
        }

        /// <summary>
        ///     Proxy for Esv Bible Web Service API (Version 2)
        /// </summary>
        /// <param name="outputFormat">Enumerating stating the output format to use.  Setting this without stating setting a settings object will leave the output settings at their defaults for this output format.</param>
        public EsvBibleServiceV2(OutputFormat outputFormat)
        {
            OutputFormat = outputFormat;
        }

        /// <summary>
        ///     Proxy for Esv Bible Web Service API (Version 2)
        /// </summary>
        /// <param name="settings">Settings regarding the format of the output message.</param>
        public EsvBibleServiceV2(IOutputSetting settings)
        {
            Settings = settings;
        }

        private IOutputSetting TempSettings { get; set; }

        /// <summary>
        ///     Read-only view of the output format of the proxy.  Set either OutputFormat or Settings; changing one alters the other.
        /// </summary>
        public OutputFormat OutputFormat
        {
            get { return _outputFormat; }

            set
            {
                _outputFormat = value;
                _settings = CreateSettings(_outputFormat);
            }
        }

        /// <summary>
        ///     Settings regarding the format of the output message.  Set either OutputFormat or Settings; changing one alters the other.
        /// </summary>
        public IOutputSetting Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = CreateSettings(OutputFormat);
                }

                return _settings;
            }
            set
            {
                _settings = value;
                _outputFormat = GetOutputFormat(_settings);
            }
        }

        private OutputFormat GetOutputFormat(IOutputSetting settings)
        {
            if (settings is CrosswayXmlVersion10Setting)
            {
                return OutputFormat.CrosswayXmlVersion10;
            }
            else if (settings is HtmlOutputSetting)
            {
                return OutputFormat.Html;
            }
            else if (settings is PlainTextSetting)
            {
                return OutputFormat.PlainText;
            }
            else
            {
                throw new EsvBibleException("Unknown settings type");
            }
        }

        private IOutputSetting CreateSettings(OutputFormat outputFormat)
        {
            switch (outputFormat)
            {
                case OutputFormat.CrosswayXmlVersion10:
                    return new CrosswayXmlVersion10Setting();

                case OutputFormat.Html:
                    return new HtmlOutputSetting();

                case OutputFormat.PlainText:
                    return new PlainTextSetting();

                default:
                    throw new EsvBibleException("Unknown output format");
            }
        }

        /// <summary>
        ///     Provides a new verse daily.
        /// </summary>
        /// <returns>Passage data</returns>
        public String DailyVerse()
        {
            return DailyVerse(new DailyVerseParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey});
        }

        /// <summary>
        ///     Provides a new verse daily.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public String DailyVerse(DailyVerseParameter parameters)
        {
            return new RestServiceCaller().CallDailyVerseMethod(parameters, Settings);
        }

        /// <summary>
        ///     Looks up a passage.
        /// </summary>
        /// <returns>Passage data</returns>
        public String PassageQuery(String passage)
        {
            return PassageQuery(new PassageQueryParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey, Passage = passage});
        }

        /// <summary>
        ///     Looks up a passage.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public String PassageQuery(PassageQueryParameter parameters)
        {
            return new RestServiceCaller().CallPassageQueryMethod(parameters, Settings);
        }

        /// <summary>
        ///     Looks up a passage.
        /// </summary>
        /// <param name="parameters">aAssage to query</param>
        /// <returns>Passage data</returns>
        public XmlDocument PassageQueryAsXmlDocument(String passage)
        {
            return PassageQueryAsXmlDocument(new PassageQueryParameter {Passage = passage});
        }

        /// <summary>
        ///     Looks up a passage.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public XmlDocument PassageQueryAsXmlDocument(PassageQueryParameter parameters)
        {
            if (!(Settings is CrosswayXmlVersion10Setting))
            {
                TempSettings = Settings;
                Settings = new CrosswayXmlVersion10Setting();
            }

            String data = String.Empty;
            try
            {
                data = new RestServiceCaller().CallPassageQueryMethod(parameters, Settings);
            }
            catch (EsvBibleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EsvBibleException("Error in service call or message format.  See InnerException for more information.", ex);
            }

            if (TempSettings != null && !(TempSettings is CrosswayXmlVersion10Setting))
            {
                Settings = TempSettings;
            }

            var doc = new XmlDocument();
            doc.LoadXml(data);

            if (data.Contains("<error>"))
            {
                String error = doc.DocumentElement.SelectSingleNode("//crossway-bible/error").InnerXml;
                throw new EsvBibleException(error);
            }

            return doc;
        }

        /// <summary>
        ///     Provides the result of an XPath query on passage query data.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Result from XPath query</returns>
        public String PassageQueryValueViaXPath(PassageQueryParameter parameters)
        {
            if (!String.IsNullOrEmpty(parameters.XPath))
            {
                XmlDocument doc = PassageQueryAsXmlDocument(parameters);
                if (doc != null && doc.DocumentElement != null)
                {
                    XmlNode node = doc.DocumentElement.SelectSingleNode(parameters.XPath);
                    if (node != null)
                    {
                        return node.InnerXml;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                else
                {
                    throw new EsvBibleException("Error retreiving XML document.");
                }
            }
            else
            {
                throw new EsvBibleException("XPath (used for finding values in returned XML document )is required when calling QueryInfoValueViaXml.");
            }
        }

        /// <summary>
        ///     Provides multiple results of an XPath query on passage query data.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Results from XPath query</returns>
        public String[] PassageQueryValueViaXPathMulti(PassageQueryParameter parameters)
        {
            var results = new List<String>();
            if (parameters.XPathSet != null && parameters.XPathSet.Length > 0)
            {
                XmlDocument doc = PassageQueryAsXmlDocument(parameters);
                if (doc != null && doc.DocumentElement != null)
                {
                    foreach (String xpath in parameters.XPathSet)
                    {
                        XmlNode node = doc.DocumentElement.SelectSingleNode(xpath);
                        if (node != null)
                        {
                            results.Add(node.InnerXml);
                        }
                    }
                    return results.ToArray();
                }
                else
                {
                    throw new EsvBibleException("Error retreiving XML document.");
                }
            }
            else
            {
                throw new EsvBibleException("XPathSet (used for finding values in returned XML document )is required when calling QueryInfoValueViaXml.");
            }
        }

        /// <summary>
        ///     Looks up a passage or shows word-search results, depending on the query.
        /// </summary>
        /// <returns>Keywords with which to query</returns>
        public String Query(String query)
        {
            return Query(new QueryParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey, Q = query});
        }

        /// <summary>
        ///     Looks up a passage or shows word-search results, depending on the query.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public String Query(QueryParameter parameters)
        {
            return new RestServiceCaller().CallQueryMethod(parameters, Settings);
        }

        /// <summary>
        ///     Provides parsing and display information about your query, including identify whether it is a passage reference or a word search.
        /// </summary>
        /// <param name="q">Text for which to query</param>
        /// <returns>Passage data</returns>
        public String QueryInfo(String q)
        {
            return QueryInfo(new QueryInfoParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey, Q = q});
        }

        /// <summary>
        ///     Accesses passages from the Esv devotional section.
        /// </summary>
        /// <returns>Passage data</returns>
        public String ReadingPlanQuery()
        {
            return ReadingPlanQuery(new ReadingPlanQueryParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey});
        }

        /// <summary>
        ///     Accesses passages from the Esv devotional section.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public String ReadingPlanQuery(ReadingPlanQueryParameter parameters)
        {
            return new RestServiceCaller().CallReadingPlanQueryMethod(parameters, Settings);
        }

        /// <summary>
        ///     Provides parsing and display information about your query, including identify whether it is a passage reference or a word search.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public String QueryInfo(QueryInfoParameter parameters)
        {
            return new RestServiceCaller().CallQueryInfoMethod(parameters, Settings);
        }

        /// <summary>
        ///     Provides parsing and display information about your query, including identify whether it is a passage reference or a word search.
        /// </summary>
        /// <param name="parameters">Text for which to query</param>
        /// <returns>Passage data in an XmlDocument object</returns>
        public XmlDocument QueryInfoAsXmlDocument(String q)
        {
            return QueryInfoAsXmlDocument(new QueryInfoParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey, Q = q});
        }

        /// <summary>
        ///     Provides parsing and display information about your query, including identify whether it is a passage reference or a word search.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data in an XmlDocument object</returns>
        public XmlDocument QueryInfoAsXmlDocument(QueryInfoParameter parameters)
        {
            String data = new RestServiceCaller().CallQueryInfoMethod(parameters, Settings);

            var doc = new XmlDocument();
            doc.LoadXml(data);

            if (data.Contains("<error>"))
            {
                String error = doc.DocumentElement.SelectSingleNode("//crossway-bible/error").InnerXml;
                throw new EsvBibleException(error);
            }

            return doc;
        }

        /// <summary>
        ///     Provides parsing and display information about your query, including identify whether it is a passage reference or a word search.
        /// </summary>
        /// <param name="parameters">Text for which to query</param>
        /// <returns>Query information in a QueryInfoData object</returns>
        public QueryInfoData QueryInfoAsObject(String q)
        {
            return QueryInfoAsObject(new QueryInfoParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey, Q = q});
        }

        /// <summary>
        ///     Provides parsing and display information about your query, including identify whether it is a passage reference or a word search.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Query information in a QueryInfoData object</returns>
        public QueryInfoData QueryInfoAsObject(QueryInfoParameter parameters)
        {
            XmlDocument doc = QueryInfoAsXmlDocument(parameters);
            var qi = new QueryInfoData();

            qi.Error = XPathProcessor.GetString(doc, "//crossway-bible/error");
            qi.Query = XPathProcessor.GetString(doc, "//crossway-bible/query");
            qi.QueryType = XPathProcessor.GetString(doc, "//crossway-bible/query-type") == "passage" ? QueryType.Passage : QueryType.WordSearch;

            switch (qi.QueryType)
            {
                case QueryType.Passage:
                    qi.Readable = XPathProcessor.GetString(doc, "//crossway-bible/readable");
                    qi.Unit = XPathProcessor.GetString(doc, "//crossway-bible/unit");
                    qi.Readable = XPathProcessor.GetString(doc, "//crossway-bible/readable");
                    qi.IsCompleteChapter = Int32.Parse(XPathProcessor.GetString(doc, "//crossway-bible/is-complete-chapter")) == 1 ? true : false;
                    qi.AlternateQueryType = XPathProcessor.GetString(doc, "//crossway-bible/alternate-query-type") == "passage" ? QueryType.Passage : QueryType.WordSearch;

                    Int32 count = 0;
                    if (Int32.TryParse(XPathProcessor.GetString(doc, "//crossway-bible/alternate-result-count"), out count))
                    {
                        qi.AlternateResultCount = count;
                    }

                    break;
                case QueryType.WordSearch:
                    count = 0;
                    if (Int32.TryParse(XPathProcessor.GetString(doc, "//crossway-bible/result-count"), out count))
                    {
                        qi.ResultCount = count;
                    }
                    break;
            }

            XmlNode node = XPathProcessor.GetNode(doc, "//crossway-bible/warnings");
            if (node != null && node.HasChildNodes)
            {
                qi.Warnings = new List<Warning>();
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    qi.Warnings.Add(new Warning
                        {
                            Code = childNode.ChildNodes[0].InnerText,
                            Readable = childNode.ChildNodes[1].InnerText
                        });
                }
            }

            return qi;
        }

        /// <summary>
        ///     Provides information about the readings for a given day.
        /// </summary>
        /// <returns>Passage data</returns>
        public String ReadingPlanInfo()
        {
            return ReadingPlanInfo(new ReadingPlanInfoParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey});
        }

        /// <summary>
        ///     Provides information about the readings for a given day.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public String ReadingPlanInfo(ReadingPlanInfoParameter parameters)
        {
            return new RestServiceCaller().CallReadingPlanInfoMethod(parameters, Settings);
        }

        /// <summary>
        ///     Provides a random verse from a selected list, or specified verse. See http://www.gnpcb.org/esv/share/rss2.0/?show-verses=true for verse list.
        /// </summary>
        /// <returns>Passage data</returns>
        public String Verse()
        {
            return Verse(new VerseParameter {Key = EsvBibleServiceConfiguration.EsvBibleServiceKey});
        }

        /// <summary>
        ///     Provides a random verse from a selected list, or specified verse. See http://www.gnpcb.org/esv/share/rss2.0/?show-verses=true for verse list.
        /// </summary>
        /// <param name="parameters">Parameters for request</param>
        /// <returns>Passage data</returns>
        public String Verse(VerseParameter parameters)
        {
            return new RestServiceCaller().CallVerseMethod(parameters, Settings);
        }
    }
}