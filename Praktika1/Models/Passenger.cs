﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airport.Models;

public partial class Passenger
{
    [Key]
    public int Passenger_ID { get; set; }

    public string Last_Name { get; set; }

    public string First_Name { get; set; }

    public string Middle_Name { get; set; }

    public string Departure_Point { get; set; }

    public string Destination { get; set; }

    public DateTime Departure_Date { get; set; }

    public int Airplane_ID { get; set; }

    public virtual Airplane Airplane { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}