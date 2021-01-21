using Domain.CartModule.Events;
using Domain.CartModule.Models;
using Domain.Core;
using Domain.CustomerModule;
using Domain.Persistence;
using Domain.ProductModule;
using Domain.PubSub;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.CartModule
{
    public interface ICartWriter
    {
        Task CreateAsync(string customerId);
        Task AddProductAsync(string cartId, string productId, int quantity);
        Task ChangeProductQuantityAsync(string cartId, string productId, int quantity);
    }

    public class CartWriter : ICartWriter
    {
        private readonly IRepository<Cart, CartId> _cartRepository;
        private readonly ITransientDomainEventSubscriber _subscriber;
        private readonly IEnumerable<IDomainEventHandler<CartId, CartCreatedEvent>> _cartCreatedEventHandlers;
        private readonly IEnumerable<IDomainEventHandler<CartId, ProductAddedEvent>> _productAddedEventHandlers;
        private readonly IEnumerable<IDomainEventHandler<CartId, ProductQuantityChangedEvent>> _productQuantityChangedEventHandlers;

        public CartWriter(IRepository<Cart, CartId> cartRepository,
            ITransientDomainEventSubscriber subscriber,
            IEnumerable<IDomainEventHandler<CartId, CartCreatedEvent>> cartCreatedEventHandlers,
            IEnumerable<IDomainEventHandler<CartId, ProductAddedEvent>> productAddedEventHandlers,
            IEnumerable<IDomainEventHandler<CartId, ProductQuantityChangedEvent>> productQuantityChangedEventHandlers)
        {
            _cartRepository = cartRepository;
            _subscriber = subscriber;
            _cartCreatedEventHandlers = cartCreatedEventHandlers;
            _productAddedEventHandlers = productAddedEventHandlers;
            _productQuantityChangedEventHandlers = productQuantityChangedEventHandlers;
        }


        public async Task AddProductAsync(string cartId, string productId, int quantity)
        {
            var cartResult = await _cartRepository.GetByIdAsync(new CartId(cartId));
            if (cartResult.HasNoValue)
                throw new ArgumentException(nameof(cartId));

            var cart = cartResult.Value;
            _subscriber.Subscribe<ProductAddedEvent>(async @event => await HandleAsync(_productAddedEventHandlers, @event));

            cart.AddProduct(new ProductId(productId), quantity);
            await _cartRepository.SaveAsync(cart);
        }

        public async Task ChangeProductQuantityAsync(string cartId, string productId, int quantity)
        {
            var cartResult = await _cartRepository.GetByIdAsync(new CartId(cartId));
            if (cartResult.HasNoValue)
                throw new ArgumentException(nameof(cartId));

            var cart = cartResult.Value;
            _subscriber.Subscribe<ProductQuantityChangedEvent>(async @event => await HandleAsync(_productQuantityChangedEventHandlers, @event));
            cart.ChangeProductQuantity(new ProductId(productId), quantity);
            await _cartRepository.SaveAsync(cart);
        }

        public async Task CreateAsync(string customerId)
        {
            var cart = new Cart(CartId.NewCartId(), new CustomerId(customerId));

            _subscriber.Subscribe<CartCreatedEvent>(async @event => await HandleAsync(_cartCreatedEventHandlers, @event));
            await _cartRepository.SaveAsync(cart);
        }

        public async Task HandleAsync<T>(IEnumerable<IDomainEventHandler<CartId, T>> handlers, T @event) where T : IDomainEvent<CartId>
        {
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(@event);
            }
        }
    }
}
