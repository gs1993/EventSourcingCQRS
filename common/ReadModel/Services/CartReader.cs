using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ReadModel.Persistence;
using ReadModel.Cart;

namespace ReadModel.Services
{
    public interface ICartReader
    {
        Task<Cart.Cart> Get(string id);
        Task<IEnumerable<Cart.Cart>> Find(Expression<Func<Cart.Cart, bool>> predicate);
        Task<IEnumerable<CartItem>> GetCartItems(string cartId);
    }

    public class CartReader : ICartReader
    {
        private readonly IReadOnlyRepository<Cart.Cart> _cartRepository;
        private readonly IReadOnlyRepository<CartItem> _cartItemRepository;

        public CartReader(IReadOnlyRepository<Cart.Cart> cartRepository,
            IReadOnlyRepository<CartItem> cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }


        public Task<Cart.Cart> Get(string id)
        {
            return _cartRepository.Get(id);
        }

        public Task<IEnumerable<Cart.Cart>> Find(Expression<Func<Cart.Cart, bool>> predicate)
        {
            return _cartRepository.Find(predicate);
        }

        public Task<IEnumerable<CartItem>> GetCartItems(string cartId)
        {
            return _cartItemRepository.Find(x => x.CartId == cartId);
        }
    }
}
