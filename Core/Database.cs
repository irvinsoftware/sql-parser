﻿using System.Collections.Generic;

namespace Irvin.SqlParser
{
    public class Database
    {
        public List<Filegroup> Filegroups { get; set; }
        public List<Schema> Schemas { get; set; }
    }
}