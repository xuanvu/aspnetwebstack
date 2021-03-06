﻿using System;

namespace Microsoft.Web.Http.Data
{
    /// <summary>
    /// Attribute applied to a <see cref="DataController"/> method to indicate that it is an insert method.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = false, Inherited = true)]
    public sealed class InsertAttribute : Attribute
    {
    }
}
