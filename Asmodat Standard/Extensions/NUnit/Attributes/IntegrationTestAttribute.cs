using System;
using NUnit.Framework;

namespace AsmodatStandard.Extensions.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IntegrationTestAttribute : CategoryAttribute
    { }
}
