﻿using System.Web.Mvc;
using Xunit;

namespace Microsoft.Web.Mvc.Test
{
    public class SkipBindingAttributeTest
    {
        [Fact]
        public void GetBinderReturnsModelBinderWhichReturnsNull()
        {
            // Arrange
            CustomModelBinderAttribute attr = new SkipBindingAttribute();
            IModelBinder binder = attr.GetBinder();

            // Act
            object result = binder.BindModel(null, null);

            // Assert
            Assert.Null(result);
        }
    }
}
