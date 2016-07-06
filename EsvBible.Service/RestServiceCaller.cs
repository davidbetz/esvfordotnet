using System;
using System.Collections.Generic;
using EsvBible.Service.Parameter;
using EsvBible.Service.Setting;
using Nalarium.Net;

namespace EsvBible.Service
{
    public class RestServiceCaller
    {
        public RestServiceCaller()
        {
            ArgumentList = new Dictionary<String, String>();
        }

        private Dictionary<String, String> ArgumentList { get; set; }

        internal String CallDailyVerseMethod(DailyVerseParameter parameters, IOutputSetting settings)
        {
            ArgumentListManager.InitArgumentList(ArgumentList, parameters.Key);
            ArgumentListManager.ApplyArguments(ArgumentList, parameters);
            return CallMethod(RestEndpoint.DailyVerse, settings);
        }

        private String CallMethod(String endpoint, IOutputSetting settings)
        {
            ArgumentListManager.ApplyArguments(ArgumentList, settings);
            String argumentList = ArgumentListManager.GetArgumentString(ArgumentList);
            return HttpAbstractor.GetWebText(GetAbsoluteUri(endpoint, argumentList), settings.Timeout);
        }

        private Uri GetAbsoluteUri(String endpoint, String argumentList)
        {
            return new Uri(String.Format("{0}?{1}", endpoint, argumentList));
        }

        internal String CallPassageQueryMethod(PassageQueryParameter parameters, IOutputSetting settings)
        {
            ArgumentListManager.InitArgumentList(ArgumentList, parameters.Key);
            ArgumentListManager.AddArgument(ArgumentList, "passage", parameters.Passage);
            return CallMethod(RestEndpoint.PassageQuery, settings);
        }

        internal String CallQueryMethod(QueryParameter parameters, IOutputSetting settings)
        {
            ArgumentListManager.InitArgumentList(ArgumentList, parameters.Key);
            ArgumentListManager.ApplyArguments(ArgumentList, parameters);
            return CallMethod(RestEndpoint.Query, settings);
        }

        internal String CallQueryInfoMethod(QueryInfoParameter parameters, IOutputSetting settings)
        {
            ArgumentListManager.InitArgumentList(ArgumentList, parameters.Key);
            ArgumentListManager.AddArgument(ArgumentList, "q", parameters.Q);
            return CallMethod(RestEndpoint.QueryInfo, settings);
        }

        internal String CallReadingPlanInfoMethod(ReadingPlanInfoParameter parameters, IOutputSetting settings)
        {
            ArgumentListManager.InitArgumentList(ArgumentList, parameters.Key);
            ArgumentListManager.ApplyArguments(ArgumentList, parameters);
            return CallMethod(RestEndpoint.ReadingPlanInfo, settings);
        }

        internal String CallReadingPlanQueryMethod(ReadingPlanQueryParameter parameters, IOutputSetting settings)
        {
            ArgumentListManager.InitArgumentList(ArgumentList, parameters.Key);
            ArgumentListManager.ApplyArguments(ArgumentList, parameters);
            return CallMethod(RestEndpoint.ReadingPlanQuery, settings);
        }

        internal String CallVerseMethod(VerseParameter parameters, IOutputSetting settings)
        {
            ArgumentListManager.InitArgumentList(ArgumentList, parameters.Key);
            ArgumentListManager.ApplyArguments(ArgumentList, parameters);
            return CallMethod(RestEndpoint.Verse, settings);
        }
    }
}