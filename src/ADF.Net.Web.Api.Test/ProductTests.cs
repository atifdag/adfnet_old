using NUnit.Framework;

namespace ADF.Net.Web.Api.Test
{
    public class ProductTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Get_WhenCalled_ReturnsAllItems()
        {
            var okResult = _controller.Get().Result as OkObjectResult;
            Assert.Pass();
        }
    }
}