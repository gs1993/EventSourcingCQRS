using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EventSourcingCQRS.ReadModel.Cart;
using EventSourcingCQRS.ReadModel.Persistence;

namespace EventSourcingCQRS.Application.Services
{
    public interface ICartReader
    {
        Task<Cart> Get(string id);

        Task<IEnumerable<Cart>> FindAll(Expression<Func<Cart, bool>> predicate);

        Task<IEnumerable<CartItem>> GetCartItems(string cartId);
    }

    public class CartReader : ICartReader
    {
        private readonly IReadOnlyRepository<Cart> _cartRepository;
        private readonly IReadOnlyRepository<CartItem> _cartItemRepository;

        public CartReader(IReadOnlyRepository<Cart> cartRepository, 
            IReadOnlyRepository<CartItem> cartItemRepository)
        {
            this._cartRepository = cartRepository;
            this._cartItemRepository = cartItemRepository;
        }


        public Task<Cart> Get(string id)
        {
            return _cartRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Cart>> FindAll(Expression<Func<Cart, bool>> predicate)
        {
            return _cartRepository.FindAllAsync(predicate);
        }

        public Task<IEnumerable<CartItem>> GetCartItems(string cartId)
        {
            return _cartItemRepository.FindAllAsync(x => x.CartId == cartId);
        }
    }
}
