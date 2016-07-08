# ESV Bible Services for .NET

Copyright (c) 2007 David Betz

See [https://netfxharmonics.com/2007/11/esv-bible-web-service-client-for-net-35](https://netfxharmonics.com/2007/11/esv-bible-web-service-client-for-net-35) for fuller details.

## Usage

Here's a simple passage query returning HTML data:

    var service = new EsvBibleServiceV2( );
    String output = service.PassageQuery("Galatians 3:11");

    With the flip of a switch you can turn it into plain text:

    var service = new EsvBibleServiceV2(OutputFormat.PlainText);
    String output = service.PassageQuery("Galatians 3:11");

For more flexibility, you may use the provided parameter objects.  Using these in C# 3.0 is seamless thanks to object initializers:

    var pqp = new PassageQueryParameter( ) { Passage = "John 14:6" };
    var service = new EsvBibleServiceV2(new PlainTextSetting( )
    {
        LineLength = 100,
        Timeout = 30
    });
    String output = service.PassageQuery(pqp);

Here is a simple sample of accessing the verse of the day (in HTML without the audio link -- optional Setting):

    var service = new EsvBibleServiceV2(new HtmlOutputSetting( )
    {
        IncludeAudioLink = false
    });
    String output = service.DailyVerse( );

You can also access various reading plans via the provided .NET enumeration:

    var service = new EsvBibleServiceV2( );
    String output = service.ReadingPlanQuery(new ReadingPlanQueryParameter( )
    {
        ReadingPlan = ReadingPlan.EveryDayInTheWord
    });

Searching is also streamlined:

    var service = new EsvBibleServiceV2( );
    String output = service.Query("Justified");

Here is a length example showing how you can use the QueryInfoAsObject method to get information about a query as a strongly-type object:

    var service = new EsvBibleServiceV2( );
    QueryInfoData result = service.QueryInfoAsObject("Samuel");

    Console.WriteLine(result.QueryType);
    Console.WriteLine("----------------------");
    if (result.QueryType == QueryType.Passage) {
        Console.WriteLine("Passage: " + result.Readable);
        Console.WriteLine("Complete Chapter?: " + result.IsCompleteChapter);
        if (result.AlternateQueryType != QueryType.None) {
            Console.WriteLine(String.Format("Alternate: {0}, {1}", result.AlternateQueryType, result.AlternateResultCount));
        }
    }

    if (result.HasWarnings) {
        foreach (Warning w in result.Warnings) {
            Console.WriteLine(String.Format("{0}: {1}", w.Code, w.Readable));
        }
    }

For more advanced users, the Crossway XML format is also available:

    var service = new EsvBibleServiceV2(new CrosswayXmlVersion10Setting( )
    {
        IncludeWordIds = true,
        IncludeXmlDeclaration = true
    });
    String output = service.PassageQuery(new PassageQueryParameter( )
    {
        Passage = "Galatians 3"
    });
    Console.WriteLine(output);

That same XML data is also retrievable as an XmlDocument for pure XML interaction:

    var service = new EsvBibleServiceV2( );
    XmlDocument output = service.PassageQueryAsXmlDocument("Galatians 3");

For more flexible XML interaction, you may use XPath:

var service = new EsvBibleServiceV2( );

    String output = service.PassageQueryValueViaXPath(new PassageQueryParameter( )
    {
        Passage = "Gal 3:4-5",
        XPath = "//crossway-bible/passage/surrounding-chapters/current"
    });

Sometimes, however, you will want more than one result from XPath:

    String[] output = service.PassageQueryValueViaXPathMulti(new PassageQueryParameter( )
    {
        Passage = "Gal 3:4-5",
        XPathSet = new[]
        {
            "//crossway-bible/passage/surrounding-chapters/previous",
            "//crossway-bible/passage/surrounding-chapters/next"                
        }
    });
