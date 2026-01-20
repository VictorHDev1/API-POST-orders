using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderApi.Application.Dtos;
using OrderApi.Application.Interfaces;
using OrderApi.Controllers;
using OrderApi.Domain.Enums;
using Xunit;

namespace OrderApi.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _serviceMock;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _serviceMock = new Mock<IOrderService>();
            _controller = new OrderController(_serviceMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenOrderIsCreated()
        {
            // Arrange
            var request = new CreateOrderRequest(
                 1,
                 new List<CreateOrderItemRequest>
                 {
                    new CreateOrderItemRequest(
                        "Keyboard",
                        "KB-01",
                        2,
                        100
                    )
                 }
             );
            var response = new OrderResponse(
                           10,
                           "ORD-00010",
                           1,
                           "John Doe",
                           OrderStatus.Confirmed,
                           200m,
                           DateTime.UtcNow,
                           new List<OrderItemResponse>()
                       );

            _serviceMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateOrderRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(OrderController.GetById), createdResult.ActionName);
            Assert.Equal(response, createdResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenOrderExists()
        {
            // Arrange
            var response = new OrderResponse(
                             10,
                             "ORD-00010",
                             1,
                             "John Doe",
                             OrderStatus.Shipped,
                             200m,
                             DateTime.UtcNow,
                             new List<OrderItemResponse>()
                         );
            _serviceMock
                .Setup(s => s.GetByIdAsync(5))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((OrderResponse?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}