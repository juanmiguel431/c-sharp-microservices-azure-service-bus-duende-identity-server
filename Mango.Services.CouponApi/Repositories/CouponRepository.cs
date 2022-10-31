using AutoMapper;
using Mango.Services.CouponApi.DbContexts;
using Mango.Services.CouponApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CouponRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _db = applicationDbContext;
        _mapper = mapper;
    }
    
    public async Task<CouponDto> GetCouponByCode(string couponCode)
    {
        var couponFromDb = await _db.Coupons.FirstOrDefaultAsync(p => p.CouponCode == couponCode);
        return _mapper.Map<CouponDto>(couponFromDb);
    }
}