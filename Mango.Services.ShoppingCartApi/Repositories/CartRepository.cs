using AutoMapper;
using Mango.Services.ShoppingCartApi.DbContexts;
using Mango.Services.ShoppingCartApi.Models;
using Mango.Services.ShoppingCartApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartApi.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CartRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartByUserId(string userId)
    {
        var cart = new Cart
        {
            CartHeader = await _db.CartHeaders.SingleAsync(p => p.UserId == userId)
        };
        
        var cartCartDetails = _db.CartDetails.Where(p =>
                p.CartHeaderId == cart.CartHeader.CartHeaderId)
            .Include(u => u.Product);

        cart.CartDetails = cartCartDetails;

        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
    {
        var cart = _mapper.Map<Cart>(cartDto);
        var cartDetail = cart.CartDetails.Single();

        var cartDetailDto = cartDto.CartDetails.Single();

        //check if the product exists in the database, if not create it!
        var prodInDb = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == cartDetailDto.ProductId);

        if (prodInDb == null)
        {
            _db.Products.Add(cartDetailDto.Product);
            await _db.SaveChangesAsync();
        }

        //Check if header is null
        var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

        if (cartHeaderFromDb == null)
        {
            //Create the header and details
            _db.CartHeaders.Add(cart.CartHeader);
            await _db.SaveChangesAsync();

            cartDetail.CartHeaderId = cart.CartHeader.CartHeaderId;
            cartDetail.Product = null;

            _db.CartDetails.Add(cartDetail);
            await _db.SaveChangesAsync();
        }
        else
        {
            // if header is not null
            // check if details has some products
            var cartDetailsFromDb = await _db.CartDetails.AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.ProductId == cartDetail.ProductId
                    && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

            if (cartDetailsFromDb == null)
            {
                //create details
                cartDetail.CartHeaderId = cartHeaderFromDb.CartHeaderId;
                cartDetail.Product = null;
                _db.CartDetails.Add(cartDetail);
                await _db.SaveChangesAsync();
            }
            else
            {
                // update the count
                cartDetail.Product = null;
                cartDetail.Count += cartDetailsFromDb.Count;
                _db.CartDetails.Update(cartDetail);
                await _db.SaveChangesAsync();
            }
        }

        return _mapper.Map<CartDto>(cart);
    }

    public async Task<bool> RemoveFromCart(int cartDetailId)
    {
        try
        {
            var cartDetails = await _db.CartDetails.SingleOrDefaultAsync(p => p.CartDetailId == cartDetailId);

            var totalCountOfCartItems = await _db.CartDetails.CountAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

            _db.CartDetails.Remove(cartDetails);

            if (totalCountOfCartItems == 1)
            {
                var cartHeader = await _db.CartHeaders.SingleOrDefaultAsync(p => p.CartHeaderId == cartDetails.CartHeaderId);
                _db.CartHeaders.Remove(cartHeader);
            }

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            
        }

        return false;
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
        if (cartHeaderFromDb == null) return false;

        _db.CartDetails.RemoveRange(_db.CartDetails.Where(u => u.CartHeaderId == cartHeaderFromDb.CartHeaderId));
        _db.CartHeaders.Remove(cartHeaderFromDb);
        await _db.SaveChangesAsync();
        return true;
    }
}