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
        Task<Cart> GetByIdAsync(Guid id);

        Task<IEnumerable<Cart>> FindAllAsync(Expression<Func<Cart, bool>> predicate);

        Task<IEnumerable<CartItem>> GetItemsOfAsync(string cartId);
    }

    public class CartReader : ICartReader
    {
        private readonly IReadOnlyRepository<Cart> _cartRepository;
        private readonly IReadOnlyRepository<CartItem> _cartItemRepository;

        public CartReader(IReadOnlyRepository<Cart> cartRepository, IReadOnlyRepository<CartItem> cartItemRepository)
        {
            this._cartRepository = cartRepository;
            this._cartItemRepository = cartItemRepository;
        }


        public async Task<Cart> GetByIdAsync(Guid id)
        {
            return await _cartRepository.GetByIdAsync(id.ToString()); //TODO: change repo arg to guid
        }

        public async Task<IEnumerable<Cart>> FindAllAsync(Expression<Func<Cart, bool>> predicate)
        {
            return await _cartRepository.FindAllAsync(predicate);
        }

        public async Task<IEnumerable<CartItem>> GetItemsOfAsync(string cartId)
        {
            return await _cartItemRepository.FindAllAsync(x => x.CartId == cartId);
        }
    }
}
