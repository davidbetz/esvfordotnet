using System;

namespace EsvBible.Service.Parameter
{
    public abstract class Parameter
    {
        private String key;

        /// <summary>
        ///     Your access key. For testing purposes, you can use the key TEST. For general-purpose queries, you can use the key IP.
        /// </summary>
        public String Key
        {
            get
            {
                if (String.IsNullOrEmpty(key))
                {
                    key = EsvBibleServiceConfiguration.EsvBibleServiceKey;
                }
                return key;
            }
            set { key = value; }
        }
    }
}