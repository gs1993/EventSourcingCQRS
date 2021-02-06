using System.Linq;
using System.Threading.Tasks;
using ReadModel.Customer;
using ReadModel.Product;
using ReadModel.Persistence;
using CartReadModel = ReadModel.Cart.Cart;
using CartItemReadModel = ReadModel.Cart.CartItem;
using Domain.CartModule.Events;
using Domain.CartModule.Models;
using Domain.PubSub;

namespace EventSourcingCQRS.Application.Handlers
{
    public class CartUpdater : IDomainEventHandler<CartId, CartCreatedEvent>,
        IDomainEventHandler<CartId, ProductAddedEvent>,
        IDomainEventHandler<CartId, ProductQuantityChangedEvent>
    {
        private readonly IReadOnlyRepository<Customer> _customerRepository;
        private readonly IReadOnlyRepository<Product> _productRepository;
        private readonly IRepository<CartReadModel> _cartRepository;
        private readonly IRepository<CartItemReadModel> _cartItemRepository;

        public CartUpdater(IReadOnlyRepository<Customer> customerRepository,
            IReadOnlyRepository<Product> productRepository,
            IRepository<CartReadModel> cartRepository,
            IRepository<CartItemReadModel> cartItemRepository)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }


        public async Task HandleAsync(CartCreatedEvent @event)
        {
            var customer = await _customerRepository.Get(@event.CustomerId.IdAsString());
            var cartReadModel = new CartReadModel
            {
                Id = @event.AggregateId.IdAsString(),
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                TotalItems = 0
            };

            await _cartRepository.Insert(cartReadModel);
        }

        public async Task HandleAsync(ProductAddedEvent @event)
        {
            var product = await _productRepository.Get(@event.ProductId.IdAsString());
            var cart = await _cartRepository.Get(@event.AggregateId.IdAsString());
            var cartItem = CartItemReadModel.CreateFor(@event.AggregateId.IdAsString(), @event.ProductId.IdAsString());

            cartItem.ProductName = product.Name;
            cartItem.Quantity = @event.Quantity;
            cart.TotalItems += @event.Quantity;
            await _cartRepository.Update(cart);
            await _cartItemRepository.Insert(cartItem);
        }

        public async Task HandleAsync(ProductQuantityChangedEvent @event)
        {
            var cartItemId = CartItemReadModel.IdFor(@event.AggregateId.IdAsString(), @event.ProductId.IdAsString());
            var cartItem = (await _cartItemRepository
                .Find(x => x.Id == cartItemId))
                .Single();

            var cart = await _cartRepository.Get(@event.AggregateId.IdAsString());

            cart.TotalItems += @event.NewQuantity - @event.OldQuantity;
            cartItem.Quantity = @event.NewQuantity;

            await _cartRepository.Update(cart);
            await _cartItemRepository.Update(cartItem);
        }
    }
}
