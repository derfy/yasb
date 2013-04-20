using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasb.Common.Extensions;

namespace Yasb.Tests.Common.Extensions
{
    /// <summary>
    /// Summary description for ArrayExtensionsTest
    /// </summary>
    [TestClass]
    public class ArrayExtensionsTest
    {
        

        [TestMethod]
        public void ToMultiByteArrayTest()
        {
            var strings = new string[] { "foo","bar"};
            var bytes=strings.ToMultiByteArray();
            Assert.AreEqual("foo", Encoding.Default.GetString(bytes[0]));
            Assert.AreEqual("bar", Encoding.Default.GetString(bytes[1]));
        }
        [TestMethod]
        public void ToUtf8BytesIntTest()
        {
            var num=10;
            var bytes = num.ToUtf8Bytes();
            Assert.AreEqual("10", Encoding.Default.GetString(bytes));
        }

        [TestMethod]
        public void ToUtf8BytesStringTest()
        {
            var str = "foo";
            var bytes = str.ToUtf8Bytes();
            Assert.AreEqual("foo", Encoding.Default.GetString(bytes));
        }

        [TestMethod]
        public void FromUtf8BytesTest()
        {
            var bytes = Encoding.Default.GetBytes("foo");
            var str = bytes.FromUtf8Bytes();
            Assert.AreEqual("foo", str);
        }

        [TestMethod]
        public void FromUtf8BytesShouldReturnNullTest()
        {
            byte[] bytes = null;
            var str = bytes.FromUtf8Bytes();
            Assert.AreEqual(null, str);
        }
    }
}
