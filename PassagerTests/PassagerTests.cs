using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
  [TestClass()]
  public class PassagerTests {
    [TestMethod()]
    [ExpectedException(typeof(PassagerException<int>))]
    public void ThrowIf() {
      var i = 0;
      try {
        var sw = Stopwatch.StartNew();
        Assert.AreEqual(0, i.ThrowIf(v => v > 0));
        Console.WriteLine(new { sw.ElapsedMilliseconds });
        i.ThrowIf(v => v == 0);
      } catch (PassagerException<int> exc) {
        Console.WriteLine(exc.Message);
        Assert.IsTrue(exc.Message.Contains("v == 0"));
        throw;
      }
    }
    [TestMethod()]
    [ExpectedException(typeof(PassagerException<int>))]
    public void ThrowIfWithReference() {
      var i = 0;
      try {
        var sw = Stopwatch.StartNew();
        Assert.AreEqual(0, Passager.ThrowIf(() => i, v => v > 0));
        Console.WriteLine(new { sw.ElapsedMilliseconds });
        Assert.AreEqual(0, Passager.ThrowIf(() => i, v => v == 0));
      } catch (PassagerException<int> exc) {
        Console.WriteLine(exc.Message);
        Assert.IsTrue(exc.Message.Contains("v == 0"));
        throw;
      }
    }
    [TestMethod()]
    [ExpectedException(typeof(PassagerException))]
    public void ThrowIfNotValid() {
      var s = "";
      try {
        var sw = Stopwatch.StartNew();
        Passager.ThrowIf(() => s != "");
        Console.WriteLine(new { sw.ElapsedMilliseconds });
        sw = Stopwatch.StartNew();
        Passager.ThrowIf(() => !string.IsNullOrEmpty(s));
        Console.WriteLine(new { sw.ElapsedMilliseconds });
        Passager.ThrowIf(() => string.IsNullOrEmpty(s));
      } catch (PassagerException exc) {
        Console.WriteLine(exc.Message);
        Assert.IsTrue(exc.Message.Contains("IsNullOrEmpty"));
        throw;
      }
    }
  }
}