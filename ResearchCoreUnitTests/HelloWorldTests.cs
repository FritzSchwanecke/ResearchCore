using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass()]
    public class HelloWorldTests
    {
        [TestMethod()]
        public void HelloWorldTest()
        {
            var test = new HelloWorld();
        }
    }
}