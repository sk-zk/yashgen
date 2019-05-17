using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yashgen
{
    class YoutubeDlException : Exception
    {
        public YoutubeDlException() { }
        public YoutubeDlException(string message) : base(message) { }
        public YoutubeDlException(string message, Exception inner) : base(message, inner) { }
        protected YoutubeDlException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
