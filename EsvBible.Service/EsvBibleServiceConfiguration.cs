using System;
using Nalarium.Configuration;

namespace EsvBible.Service
{
    public static class EsvBibleServiceConfiguration
    {
        /// <summary>
        ///     Representation of the EsvBibleServiceKey appSettings element in the configuration file.
        /// </summary>
        public static String EsvBibleServiceKey
        {
            get
            {
                try
                {
                    return ConfigAccessor.ApplicationSettings("EsvBibleServiceKey");
                }
                catch
                {
                    throw new EsvBibleException("Key (your access key) is required.  Use TEST for testing and IP for general purpose.");
                }
            }
        }
    }
}