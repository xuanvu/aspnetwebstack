﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.Metadata;
using System.Web.Http.Metadata.Providers;
using Moq;
using Moq.Protected;
using Xunit;
using Assert = Microsoft.TestCommon.AssertEx;

namespace System.Web.Http.Validation.Validators
{
    public class DataAnnotationsModelValidatorTest
    {
        private static CachedDataAnnotationsModelMetadataProvider _metadataProvider = new CachedDataAnnotationsModelMetadataProvider();
        private static IEnumerable<ModelValidatorProvider> _noValidatorProviders = Enumerable.Empty<ModelValidatorProvider>();

        [Fact]
        public void ConstructorGuards()
        {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(null, typeof(object));
            var attribute = new RequiredAttribute();

            // Act & Assert
            Assert.ThrowsArgumentNull(
                () => new DataAnnotationsModelValidator(null, _noValidatorProviders, attribute),
                "metadata");
            Assert.ThrowsArgumentNull(
                () => new DataAnnotationsModelValidator(metadata, null, attribute),
                "validatorProviders");
            Assert.ThrowsArgumentNull(
                () => new DataAnnotationsModelValidator(metadata, _noValidatorProviders, null),
                "attribute");
        }

        [Fact]
        public void ValuesSet()
        {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => 15, typeof(string), "Length");
            var attribute = new RequiredAttribute();

            // Act
            var validator = new DataAnnotationsModelValidator(metadata, _noValidatorProviders, attribute);

            // Assert
            Assert.Same(attribute, validator.Attribute);
            Assert.Equal(attribute.FormatErrorMessage("Length"), validator.ErrorMessage);
        }

        [Fact]
        public void ValidateWithIsValidTrue()
        {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => 15, typeof(string), "Length");
            Mock<ValidationAttribute> attribute = new Mock<ValidationAttribute> { CallBase = true };
            attribute.Setup(a => a.IsValid(metadata.Model)).Returns(true);
            DataAnnotationsModelValidator validator = new DataAnnotationsModelValidator(metadata, _noValidatorProviders, attribute.Object);

            // Act
            IEnumerable<ModelValidationResult> result = validator.Validate(null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ValidateWithIsValidFalse()
        {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => 15, typeof(string), "Length");
            Mock<ValidationAttribute> attribute = new Mock<ValidationAttribute> { CallBase = true };
            attribute.Setup(a => a.IsValid(metadata.Model)).Returns(false);
            DataAnnotationsModelValidator validator = new DataAnnotationsModelValidator(metadata, _noValidatorProviders, attribute.Object);

            // Act
            IEnumerable<ModelValidationResult> result = validator.Validate(null);

            // Assert
            var validationResult = result.Single();
            Assert.Equal("", validationResult.MemberName);
            Assert.Equal(attribute.Object.FormatErrorMessage("Length"), validationResult.Message);
        }

        [Fact]
        public void ValidatateWithValidationResultSuccess()
        {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => 15, typeof(string), "Length");
            Mock<ValidationAttribute> attribute = new Mock<ValidationAttribute> { CallBase = true };
            attribute.Protected()
                     .Setup<ValidationResult>("IsValid", ItExpr.IsAny<object>(), ItExpr.IsAny<ValidationContext>())
                     .Returns(ValidationResult.Success);
            DataAnnotationsModelValidator validator = new DataAnnotationsModelValidator(metadata, _noValidatorProviders, attribute.Object);

            // Act
            IEnumerable<ModelValidationResult> result = validator.Validate(null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void IsRequiredTests()
        {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => 15, typeof(string), "Length");

            // Act & Assert
            Assert.False(new DataAnnotationsModelValidator(metadata, _noValidatorProviders, new RangeAttribute(10, 20)).IsRequired);
            Assert.True(new DataAnnotationsModelValidator(metadata, _noValidatorProviders, new RequiredAttribute()).IsRequired);
            Assert.True(new DataAnnotationsModelValidator(metadata, _noValidatorProviders, new DerivedRequiredAttribute()).IsRequired);
        }

        class DerivedRequiredAttribute : RequiredAttribute
        {
        }
    }
}
