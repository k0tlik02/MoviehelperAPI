﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movie.Models
{
    public class filmsDBRepository
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string genre { get; set; }
        public string description_full { get; set; }
        public string large_cover_image { get; set; }
        public string url { get; set; }
        public string runtime { get; set; }
        public string year { get; set; }
    }
}
