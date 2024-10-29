using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreDuringSerializationAttribute : Attribute
    {
        public bool Ignore;
        public IgnoreDuringSerializationAttribute(bool ignore)
        {
            Ignore = ignore;
        }
    }
}
