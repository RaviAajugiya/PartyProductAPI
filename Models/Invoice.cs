using System;
using System.Collections.Generic;

namespace PartyProductAPI.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int PartyId { get; set; }

    public int ProductId { get; set; }

    public decimal Rate { get; set; }

    public int Quantity { get; set; }

    public decimal? Total { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;


    public virtual Party Party { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
