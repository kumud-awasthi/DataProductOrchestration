﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProductOrchestration
{
      public class DataProductDefinition
        {
            public Guid Id { get; set; }
            public int? order { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public bool? completed { get; set; }
        }
    
}
