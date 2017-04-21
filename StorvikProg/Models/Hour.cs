using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StorvikProg.Models
{
    public class Hour
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
        public DateTime RegDate { get; set; }
        public float Hours { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }

        //regtype

        //worktype

        //hourtype

        public Equipment Equipment { get; set; }
        public int EquipmentId { get; set; }
        public string Comment { get; set; }
        public bool Controlled { get; set; }
        public bool Approved { get; set; }
        public bool Billed { get; set; }
        
    }
}