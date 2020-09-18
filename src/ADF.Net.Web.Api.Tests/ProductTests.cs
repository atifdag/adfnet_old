using System;
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
            IProductService service = new ProductService(new MainService(), repository);
            _controller = new ProductsController(service);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _controller.Get().Result;

            // Assert
            Assert.IsType<OkObjectResult>(okResult);

        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {
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
            var okResult = _controller.Get(testGuid).Result;
            // Assert
            Assert.IsType<OkObjectResult>(okResult);
        }


        [Fact]
        public void GetById_ExistingGuidPassed_ReturnsRightItem()
        {
            // Arrange
            var testGuid = new Guid("5d981459-8f5a-4fb6-ba4a-479590917876");
            // Act
            var okResult = _controller.Get(testGuid).Result as OkObjectResult;
            // Assert
            Assert.IsType<DetailModel<ProductModel>>(okResult?.Value);
            Assert.Equal(testGuid, ((DetailModel<ProductModel>) okResult.Value).Item.Id);
        }

        [Fact]
        public void GetById_UnknownGuidPassed_ReturnsNotFoundResult()
        {
            // Act
            var notFoundResult = _controller.Get(Guid.NewGuid()).Result;
            // Assert
            Assert.IsType<NotFoundObjectResult>(notFoundResult);
        }


        [Fact]
        public void Add_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var nameMissingItem = new AddModel<ProductModel>
            {
                Item = new ProductModel
                {
                    Code = "code1"
                }
            };
            // Act
            var badResponse = _controller.Post(nameMissingItem);
            // Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
        }


        [Fact]
        public void Add_ValidObjectPassed_ReturnsCreatedResponse()
        {
            // Arrange
            var testItem = new AddModel<ProductModel>
            {
                Item = new ProductModel
                {
                    Code = "code1",
                    Name = "ProductTest1"
                }
            };
            // Act
            var createdResponse = _controller.Post(testItem);
            // Assert
            Assert.IsType<CreatedAtActionResult>(createdResponse);
        }


        [Fact]
        public void Add_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            // Arrange
            var testItem = new AddModel<ProductModel>
            {
                Item = new ProductModel
                {
                    Code = "code1",
                    Name = "ProductTest1"
                }
            };
            // Act
            var createdResponse = _controller.Post(testItem) as CreatedAtActionResult;
            var model = createdResponse?.Value as AddModel<ProductModel>;
            // Assert
            Assert.IsType<AddModel<ProductModel>>(model);
            Assert.Equal("code1", model.Item.Code);
        }


        [Fact]
        public void Remove_NotExistingGuidPassed_ReturnsNotFoundResponse()
        {
            // Arrange
            var notExistingGuid = Guid.NewGuid();
            // Act
            var badResponse = _controller.Delete(notExistingGuid);
            // Assert
            Assert.IsType<NotFoundObjectResult>(badResponse);
        }
        [Fact]
        public void Remove_ExistingGuidPassed_ReturnsOkResult()
        {
            // Arrange
            var existingGuid = new Guid("5d981459-8f5a-4fb6-ba4a-479590917876");
            // Act
            var okResponse = _controller.Delete(existingGuid);
            // Assert
            Assert.IsType<OkResult>(okResponse);
        }
       
    }
}
