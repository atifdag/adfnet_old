using System;
using System.Collections.Generic;
using ADF.Net.Data;
using ADF.Net.Data.DataAccess.EF;
using ADF.Net.Data.DataEntities;
using ADF.Net.Service;
using ADF.Net.Service.GenericCrudModels;
using ADF.Net.Service.Implementations;
using ADF.Net.Service.Models;
using ADF.Net.Web.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ADF.Net.Web.Api.Tests
{
    public class ProductTests
    {
        private readonly Product[] _items =
        {
            new Product
            {
                Id = Guid.Parse("5d981459-8f5a-4fb6-ba4a-479590917876"),
                Code="Product1",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Product 1",
                Version = 1
            },
            new Product
            {
                Id = Guid.Parse("208d799a-ee2f-4d07-bfe3-d5928cc6cf30"),
                Code="Product2",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Product 2",
                Version = 1
            },
            new Product
            {
                Id = Guid.Parse("2501203a-1484-4394-8c48-ed2a29d10fb9"),
                Code="Product3",
                CreationTime = DateTime.Now,
                Description = string.Empty,
                DisplayOrder = 1,
                IsApproved = true,
                LastModificationTime = DateTime.Now,
                Name = "Product 3",
                Version = 1
            }
        };

        private readonly ProductsController _controller;

        public ProductTests()
        {
            var repository = new FakeProductRepository(new EfDbContext(new DbContextOptions<EfDbContext>()), _items);
            var service = new ProductService(new MainService(), repository);
            _controller = new ProductsController(service);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _controller.Get();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);

        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            // Act
            var okResult = _controller.Get().Result as OkObjectResult;
            // Assert
            var model = Assert.IsType<ListModel<ProductModel>>(okResult?.Value);
            Assert.Equal(3, model.Items.Count);
        }

        [Fact]
        public void GetById_ExistingGuidPassed_ReturnsOkResult()
        {
            // Arrange
            var testGuid = new Guid("5d981459-8f5a-4fb6-ba4a-479590917876");
            // Act
            var okResult = _controller.Get(testGuid);
            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }
    }
}
