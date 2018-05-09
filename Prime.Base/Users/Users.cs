using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public class Users
    {
        private readonly PrimeContext _context;

        public Users(PrimeContext context)
        {
            _context = context;
        }
    }
}
