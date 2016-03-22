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
        Passager.ThrowIf(() => i, v => v == 0, " Value={0}", new { i, d = "dimok" });
      } catch (PassagerException<int> exc) {
        Console.WriteLine(exc.Message);
        Assert.AreEqual("Parameter i<0> didn't pass validation (v == 0) Value={ i = 0, d = dimok }", exc.Message);
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
    [TestMethod()]
    [ExpectedException(typeof(PassagerException))]
    public void ThrowIfComplex() {
      var s = new[] { "" };
      var sw = Stopwatch.StartNew();
      try {
        Passager.ThrowIf(() => s.Any(), ".{0}", new { s = s[0] });
      } catch (PassagerException exc) {
        Console.WriteLine(new { sw.ElapsedMilliseconds });
        Console.WriteLine(exc.Message);
        Assert.IsTrue(exc.Message.Contains("Any"));
        throw;
      }
    }
    [TestMethod()]
    [ExpectedException(typeof(PassagerException))]
    public void ThrowIfPassthrough() {
      var s = "";
      try {
        var sw = Stopwatch.StartNew();
        Assert.AreEqual("", Passager.ThrowIf(s, () => s != ""));
        Console.WriteLine(new { sw.ElapsedMilliseconds });
        Passager.ThrowIf(s, () => string.IsNullOrEmpty(s));
      } catch (PassagerException exc) {
        Console.WriteLine(exc.Message);
        Assert.IsTrue(exc.Message.Contains("IsNullOrEmpty"));
        throw;
      }
    }
    [TestMethod()]
    [ExpectedException(typeof(PassagerException<string>))]
    public void ThrowIfExtension() {
      var s = "aaa";
      try {
        var sw = Stopwatch.StartNew();
        s.ThrowIf(str=> str != "bbb");
        Console.WriteLine(new { sw.ElapsedMilliseconds });
      } catch (PassagerException<string> exc) {
        Console.WriteLine(exc.Message);
        Assert.IsTrue(exc.Message.Contains("aaa"));
        Assert.IsTrue(exc.Message.Contains("bbb"));
        throw;
      }
    }
  }
}