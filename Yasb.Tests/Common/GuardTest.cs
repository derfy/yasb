using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common;

namespace Yasb.Tests.Common
{
    /// <summary>
    /// Summary description for GuardTest
    /// </summary>
    [TestClass]
    public class GuardTest
    {
        public class TestFoo {
            public string MyProp { get; set; }
            public int? IntProp { get; set; }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRaiseOnNull()
        {
            var foo = new TestFoo();
            Guard.NotNull<int?>(()=>foo.IntProp, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRaiseOnEmpty()
        {
            var foo = new TestFoo();
            Guard.NotNullOrEmpty(() => foo.MyProp, string.Empty);
        }
        [TestMethod]
        public void ShouldNotRaise()
        {
            var foo = new TestFoo();
            Guard.NotNullOrEmpty(() => foo.MyProp, "foo");
            
        }
    }
}
