﻿using Domain.CartModule;
using Microsoft.AspNetCore.Mvc;
using ReadModel.Customer;
using ReadModel.Persistence;
using ReadModel.Product;
using ReadModel.Services;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("{controller}")]
    public class CartsController : Controller
    {
        private readonly ICartReader _cartReader;
        private readonly ICartWriter _cartWriter;
        private readonly IReadOnlyRepository<Customer> _customerRepository;
        private readonly IReadOnlyRepository<Product> _productRepository;

        public CartsController(ICartReader cartReader,
            ICartWriter cartWriter,
            IReadOnlyRepository<Customer> customerRepository,
            IReadOnlyRepository<Product> productRepository)
        {
            _cartReader = cartReader;
            _cartWriter = cartWriter;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }


        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var viewModel = new CartIndexViewModel
            {
                Carts = await _cartReader.Find(x => true),
                Customers = await _customerRepository.Find(x => true)
            };

            return View(viewModel);
        }

        [HttpGet("{action}/{id}")]
        public async Task<ActionResult> Details(string id)
        {
            var viewModel = new CartDetailsViewModel
            {
                Cart = await _cartReader.Get(id),
                CartItems = await _cartReader.GetCartItems(id),
                Products = await _productRepository.Find(x => true)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string customerId)
        {
            await _cartWriter.CreateAsync(customerId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Carts/{id:length(41)}/AddProduct")]
        public async Task<IActionResult> AddProduct(string id, string productId, int quantity)
        {
            await _cartWriter.AddProductAsync(id, productId, quantity);

            return RedirectToAction(nameof(Details), new { id });
        }

        [Route("Carts/{id:length(41)}/ChangeProductQuantity")]
        [HttpPost]
        public async Task<IActionResult> ChangeProductQuantity(string id, string productId, int quantity)
        {
            await _cartWriter.ChangeProductQuantityAsync(id, productId, quantity);

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
