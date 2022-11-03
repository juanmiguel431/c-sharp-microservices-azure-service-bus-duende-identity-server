﻿using Mango.Services.ShoppingCartApi.Models.Dtos;

namespace Mango.Services.OrderApi.Messages;

public class CartDetailDto
{
    public int CartDetailId { get; set; }
    public int CartHeaderId { get; set; }
    public int ProductId { get; set; }
    public virtual ProductDto Product { get; set; }
    public int Count { get; set; }
}