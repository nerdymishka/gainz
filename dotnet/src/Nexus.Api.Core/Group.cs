
using System;
using System.Collections.Generic;

namespace Nexus.Api
{

    public class Group 
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string UriPath { get; set; }

        public string Description { get; set; }

        public long? ResourceId { get; set; }

        public User[] Users { get; set; }

        public Role[] Roles { get; set; }
    }

}