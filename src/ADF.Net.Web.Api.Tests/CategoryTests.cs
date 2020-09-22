using System;
using ADF.Net.Core.Enums;
using ADF.Net.Data.DataAccess.EF;
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
    public class CategoryTests
    {
       

        private readonly CategoriesController _controller;

        public CategoryTests()
        {
            var dbContext = new EfDbContext(new DbContextOptions<EfDbContext>());

            var fakeCategoryRepository = new FakeCategoryRepository(dbContext);

            ICategoryService serviceCategory = new CategoryService(new MainService(), fakeCategoryRepository);

            _controller = new CategoriesController(serviceCategory);

        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {

            // Act
            var okResult = _controller.Get(new FilterModel { Status = StatusOption.All.GetHashCode() }).Result;

            // Assert
            Assert.IsType<OkObjectResult>(okResult);

        }

        [Fact]
        public void Get_WhenCalled_ReturnsAllItems()
        {

            var okResult = _controller.Get(new FilterModel{Status = StatusOption.All.GetHashCode()}).Result as OkObjectResult;
           
            // Assert
            var model = Assert.IsType<ListModel<CategoryModel>>(okResult?.Value);

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
            Assert.IsType<DetailModel<CategoryModel>>(okResult?.Value);

            Assert.Equal(testGuid, ((DetailModel<CategoryModel>) okResult.Value).Item.Id);

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
            var nameMissingItem = new AddModel<CategoryModel>
            {
                Item = new CategoryModel
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
            var testItem = new AddModel<CategoryModel>
            {
                Item = new CategoryModel
                {
                    Code = "code1",
                    Name = "CategoryTest1"
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
            var testItem = new AddModel<CategoryModel>
            {
                Item = new CategoryModel
                {
                    Code = "code1",
                    Name = "CategoryTest1"
                }
            };

            // Act
            var createdResponse = _controller.Post(testItem) as CreatedAtActionResult;

            var model = createdResponse?.Value as AddModel<CategoryModel>;

            // Assert
            Assert.IsType<AddModel<CategoryModel>>(model);

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
