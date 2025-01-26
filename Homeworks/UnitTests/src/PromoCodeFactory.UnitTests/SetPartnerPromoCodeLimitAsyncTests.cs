using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests
{
    public class PartnerBuilder
    {
        private Guid _id;
        private bool _isActive;
        private List<PartnerPromoCodeLimit> _partnerLimits = new List<PartnerPromoCodeLimit>();

        public PartnerBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public PartnerBuilder WithIsActive(bool isActive)
        {
            _isActive = isActive;
            return this;
        }

        public PartnerBuilder WithPartnerLimits(List<PartnerPromoCodeLimit> limits)
        {
            _partnerLimits = limits;
            return this;
        }

        public Partner Build()
        {
            return new Partner
            {
                Id = _id,
                IsActive = _isActive,
                PartnerLimits = _partnerLimits
            };
        }
    }

    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnerRepositoryMock;
        private readonly PartnersController _controller;
        private readonly IFixture _fixture;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _fixture = new Fixture();
            _partnerRepositoryMock = new Mock<IRepository<Partner>>();
            _controller = new PartnersController(_partnerRepositoryMock.Object);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            _partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync((Partner)null);

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsInactive_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithIsActive(false)
                .Build();
            _partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_LimitShouldBeGreaterThanZero_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithIsActive(true)
                .Build();
            _partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 0,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SavesNewLimitToDatabase_WhenLimitIsValid()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithIsActive(true)
                .Build();
            _partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 15,
                EndDate = DateTime.UtcNow.AddMonths(3)
            };

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            _partnerRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ResetIssuedPromoCodes_WhenLimitIsActive()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithIsActive(true)
                .Build();

            var existingLimit = new PartnerPromoCodeLimit
            {
                PartnerId = partnerId,
                Limit = 5,
                CreateDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                CancelDate = null 
            };

            partner.PartnerLimits.Add(existingLimit);
            _partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId)).ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.UtcNow.AddMonths(2)
            };

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenActiveLimitExists_ShouldCancelOldLimitAndResetCounter()
        {
            var partnerId = Guid.NewGuid();
            var existingActiveLimit = new PartnerPromoCodeLimit
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Limit = 5,
                CreateDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10),
                CancelDate = null
            };

            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithIsActive(true)
                .WithPartnerLimits(new List<PartnerPromoCodeLimit> { existingActiveLimit })
                .Build();

            partner.NumberIssuedPromoCodes = 7;

            _partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            existingActiveLimit.CancelDate.Should().NotBeNull("Previous active limit should be cancelled");
            partner.NumberIssuedPromoCodes.Should().Be(0, "Issued promo codes should reset if old limit was active");
            partner.PartnerLimits.Should().HaveCount(2);

            _partnerRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_WhenOldLimitExpired_ShouldNotResetCounter()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var expiredLimit = new PartnerPromoCodeLimit
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Limit = 5,
                CreateDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-1),
                CancelDate = null
            };

            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithIsActive(true)
                .WithPartnerLimits(new List<PartnerPromoCodeLimit> { expiredLimit })
                .Build();

            partner.NumberIssuedPromoCodes = 5;

            _partnerRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.Now.AddMonths(1)
            };

            // Act
            var result = await _controller.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            partner.NumberIssuedPromoCodes.Should().Be(5, "Should not reset if old limit is already expired");
            expiredLimit.CancelDate.Should().BeNull("Expired limit is ended by time, no manual cancel needed");
            partner.PartnerLimits.Should().HaveCount(2);

            _partnerRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }


    }
}
