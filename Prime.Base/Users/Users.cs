using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core
{
    public class Users
    {
        private readonly ServerContext _context;

        public Users(ServerContext context)
        {
            _context = context;
        }
    }
}
