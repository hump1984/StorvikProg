﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StorvikProg.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public int Salary { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }

        // Add Image, Skills, as needed later.

    }
}