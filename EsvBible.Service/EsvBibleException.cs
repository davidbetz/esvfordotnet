using System;
using System.Runtime.Serialization;

namespace EsvBible.Service
{
    [Serializable]
    public class EsvBibleException : Exception
    {
        public EsvBibleException()
        {
        }

        public EsvBibleException(String message) : base(message)
        {
        }

        public EsvBibleException(String message, Exception inner) : base(message, inner)
        {
        }

        protected EsvBibleException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}