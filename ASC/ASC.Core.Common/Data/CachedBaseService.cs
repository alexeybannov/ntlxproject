using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Core.Common.Data
{
    class CachedBaseService<T>
    {
        protected T Service
        {
            get;
            private set;
        }


        protected CachedBaseService(T service)
        {
            Service = service;
        }

        //protected 
    }
}
