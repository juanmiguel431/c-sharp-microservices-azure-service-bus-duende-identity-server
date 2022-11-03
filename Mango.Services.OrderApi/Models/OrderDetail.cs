﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderApi.Models;

public class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }
    public int OrderHeaderId { get; set; }
    
    [ForeignKey(nameof(OrderHeaderId))]
    public virtual OrderHeader OrderHeader { get; set; }
    
    public int ProductId { get; set; }
    public int Count { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
}