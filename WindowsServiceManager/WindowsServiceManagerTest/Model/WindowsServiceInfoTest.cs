using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsServiceManager.Model;

namespace WindowsServiceManagerTest.Model
{
    /// <summary>
    /// This class represents unit testing <see cref="WindowsServiceInfo"/> class.
    /// </summary>
    /// <remarks>
    /// [UnitOfWork_StateUnderTest_ExpectedBehavior]
    /// </remarks>
    [TestClass]
    public class WindowsServiceInfoTest
    {
        /// <summary>
        /// GetHashCode Compare Succssed
        /// </summary>
        [TestMethod]
        public void GetHashCode_Compare_Succssed()
        {
            var info = new WindowsServiceInfo() { ServiceName = "Test1" };
            Assert.AreEqual(info.GetHashCode(), info.ServiceName.GetHashCode());
        }

        [TestMethod]
        public void Equals_Compare_Succssed()
        {
            var info_a = new WindowsServiceInfo() { ServiceName = "Test1", Status = System.ServiceProcess.ServiceControllerStatus.Running };
            var info_b = new WindowsServiceInfo() { ServiceName = "Test1", Status = System.ServiceProcess.ServiceControllerStatus.Stopped };
            Assert.IsTrue(info_a.Equals(info_b));
        }
    }
}
