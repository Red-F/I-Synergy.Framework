﻿using System.Collections.Generic;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Entities
{
    public class Company : Entity
    {
        public string Name { get; set; }

        public long? MainCompanyId { get; set; }

        public MainCompany MainCompany { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}