using System;
using System.Diagnostics;
using System.Xml;
using EsvBible.Service;
using EsvBible.Service.Parameter;
using EsvBible.Service.Setting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESVBible.Test
{
    /// <summary>
    ///     These are demos, not unit tests
    ///     For more information, see:
    ///     http://www.netfxharmonics.com/2007/11/Esv-Bible-Web-Service-20-Framework-for-NET-35
    /// </summary>
    [TestClass]
    public class EsvBible : TestBase
    {
        [TestMethod]
        public void SimplePassageQueryTest()
        {
            var service = new EsvBibleServiceV2();
            String output = service.PassageQuery("Galatians 3:11");
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void SimplePassageQueryPlainTextTest()
        {
            var service = new EsvBibleServiceV2(OutputFormat.PlainText);
            String output = service.PassageQuery("Galatians 3:11");
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void SimplePassageQueryWithParametersTest()
        {
            var pqp = new PassageQueryParameter { Passage = "John 14:6" };
            var service = new EsvBibleServiceV2(new PlainTextSetting
            {
                LineLength = 100,
                Timeout = 30
            });
            String output = service.PassageQuery(pqp);
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void DailyVerseTest()
        {
            var service = new EsvBibleServiceV2(new HtmlOutputSetting
            {
                IncludeAudioLink = false
            });
            String output = service.DailyVerse();
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void ReadingPlanTest()
        {
            var service = new EsvBibleServiceV2();
            String output = service.ReadingPlanQuery(new ReadingPlanQueryParameter
            {
                ReadingPlan = ReadingPlan.EveryDayInTheWord
            });
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void SearchTest()
        {
            var service = new EsvBibleServiceV2();
            String output = service.Query("Justified");
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void QueryInfoAsObjectTest()
        {
            var service = new EsvBibleServiceV2();
            QueryInfoData result = service.QueryInfoAsObject("Samuel");
            Assert.AreEqual("1 Samuel 1", result.Readable);
            Assert.AreEqual(131, result.AlternateResultCount);
            Trace.WriteLine(result.QueryType);
            Trace.WriteLine("----------------------");
            if (result.QueryType == QueryType.Passage)
            {
                Trace.WriteLine("Passage: " + result.Readable);
                Trace.WriteLine("Complete Chapter?: " + result.IsCompleteChapter);
                if (result.AlternateQueryType != QueryType.None)
                {
                    Trace.WriteLine(String.Format("Alternate: {0}, {1}", result.AlternateQueryType, result.AlternateResultCount));
                }
            }
            if (result.HasWarnings)
            {
                foreach (Warning w in result.Warnings)
                {
                    Trace.WriteLine(String.Format("{0}: {1}", w.Code, w.Readable));
                }
            }
        }

        [TestMethod]
        public void CrossWayXmlTest()
        {
            var service = new EsvBibleServiceV2(new CrosswayXmlVersion10Setting
            {
                IncludeWordIds = true,
                IncludeXmlDeclaration = true
            });
            String output = service.PassageQuery(new PassageQueryParameter
            {
                Passage = "Galatians 3"
            });
            Trace.WriteLine(output);
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void PassageQueryAsXmlDocumentTest()
        {
            var service = new EsvBibleServiceV2();
            XmlDocument output = service.PassageQueryAsXmlDocument("Galatians 3");
            Assert.IsFalse(String.IsNullOrEmpty(output.InnerXml));
        }

        [TestMethod]
        public void PassageQueryValueViaXPathTest()
        {
            var service = new EsvBibleServiceV2();
            String output = service.PassageQueryValueViaXPath(new PassageQueryParameter
            {
                Passage = "Gal 3:4-5",
                XPath = "//crossway-bible/passage/surrounding-chapters/current"
            });
            Assert.IsFalse(String.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void PassageQueryValueViaXPathMultiTest()
        {
            var service = new EsvBibleServiceV2();
            String[] output = service.PassageQueryValueViaXPathMulti(new PassageQueryParameter
            {
                Passage = "Gal 3:4-5",
                XPathSet = new[]
                        {
                            "//crossway-bible/passage/surrounding-chapters/previous",
                            "//crossway-bible/passage/surrounding-chapters/next"
                        }
            });
            Assert.IsTrue(output.Length == 2);
        }
    }
}