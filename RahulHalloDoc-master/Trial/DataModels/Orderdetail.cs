using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Trial.DataModels;

[Table("orderdetails")]
public partial class Orderdetail
{
    [Key]
    [Column("orderid")]
    public int Orderid { get; set; }

    [Column("productname", TypeName = "character varying")]
    public string? Productname { get; set; }

    [Column("customername", TypeName = "character varying")]
    public string? Customername { get; set; }

    [Column("amount")]
    public decimal? Amount { get; set; }
}
