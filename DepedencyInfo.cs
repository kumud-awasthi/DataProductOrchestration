﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProductOrchestration
{
        public class DependencyInfo
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string OrchestrationId { get; set; }
        }
    }
