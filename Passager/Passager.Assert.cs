using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassagerDebug {
  public static class Assert {
    public static void IsTrue(bool value, string message = null, params object[] parameters) {
      if (!value)
        Throw(true, value, string.Format(message ?? "", parameters));
    }
    public static void AreEqual<T>(T v1, T v2, string error = null) {
      if (v1.Equals(v2)) return;
      var message = "Expected <" + v1 + "> actual <" + v2 + ">." + (string.IsNullOrWhiteSpace(error) ? "" : " " + error);
      throw new Exception(message);

    }
    public static void IsNotNull<T>(T value, string message = "") {
      if (value == null)
        Throw("Not NULL", value, message);
    }
    public static void IsFalse(bool value, string message = "") {
      if (value)
        Throw(false, value, message);
    }
    public class AssertionException : Exception {
      public AssertionException(string message) : base(message) {
      }
    }
    static void Throw<E, A>(E expected, A actual, string error = "") {
      var message = "Expected <" + expected + "> actual <" + actual + ">." + (string.IsNullOrWhiteSpace(error) ? "" : " " + error);
      throw new AssertionException(message + "");
    }
  }

}