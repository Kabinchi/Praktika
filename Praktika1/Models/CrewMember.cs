﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.Models
{
    public partial class CrewMember
    {
        [Key]
        public int CrewMember_ID { get; set; }

        public string Last_Name { get; set; }

        public string First_Name { get; set; }

        public string Middle_Name { get; set; }

        public string Position { get; set; }

        public int Airplane_ID { get; set; }

        public virtual Airplane Airplane { get; set; }
    }
}
