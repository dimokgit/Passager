using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class PassageException : Exception {
  public PassageException(Expression<Func<bool>> test, string message, params object[] parameters)
    : base(string.Format("Validation {{{0}}} failed" + message, TestBody(test), parameters)) {
    //((new System.Linq.Expressions.Expression.MemberExpressionProxy((new System.Linq.Expressions.Expression.BinaryExpressionProxy((new System.Linq.Expressions.Expression.BinaryExpressionProxy((new System.Linq.Expressions.Expression.LambdaExpressionProxy(test)).Body as System.Linq.Expressions.LogicalBinaryExpression)).Left as System.Linq.Expressions.LogicalBinaryExpression)).Left as System.Linq.Expressions.FieldExpression)).Member).DeclaringType
    List<MemberInfo> members = new List<MemberInfo>();
    new Visitor(mi => members.Add(mi)).Visit(test.Body);
    members.ToArray();
  }
  private static string TestBody(Expression<Func<bool>> test) {
    List<MemberInfo> members = new List<MemberInfo>();
    new Visitor(mi => members.Add(mi)).Visit(test.Body);
    return CleanDeclaringTypes(test, members);
  }

  static string CleanDeclaringTypes(Expression<Func<bool>> test, IList<MemberInfo> members) {
    return members.Aggregate(test.Body + "", (s, mi) => s.Replace("value(" + mi + ").", ""));
  }
  class Visitor : ExpressionVisitor {
    Action<MemberInfo> _visit;
    public Visitor(Action<MemberInfo> visit) {
      _visit = visit;
    }
    public override Expression Visit(Expression node) {
      var mi = node as MemberExpression;
      if (mi != null)
        _visit(mi.Member.DeclaringType);
      return base.Visit(node);
    }
  }
}
public class PassageException<TValue> : Exception {
  public PassageException(TValue value, Expression<Predicate<TValue>> test, string message, params object[] parameters)
    : base(string.Format("Value <{0}> didn't pass validation {1}" + message, value, Regex.Split(test + "", "=>")[1].Trim(), parameters)) {
  }
  public PassageException(Expression<Func<bool>> test, string message, params object[] parameters)
    : base(string.Format("Validation <{0}> failed" + message, Regex.Split(test + "", "=>")[1].Trim(), parameters)) {
  }
  public PassageException(Expression<Func<TValue>> getter, TValue value, Expression<Predicate<TValue>> test, string message, params object[] parameters)
    : base(string.Format("Parameter {0}<{1}> didn't pass validation {2}" + message, GetParameterName(getter), value, Regex.Split(test + "", "=>")[1].Trim(), parameters)) {
  }
  private static string GetParameterName(Expression reference) {
    var lambda = reference as LambdaExpression;
    var member = lambda.Body as MemberExpression;

    return member.Member.Name;
  }
}
public static class Passage {
  public static T ThrowIf<T>(Expression<Func<T>> getter, Expression<Predicate<T>> test) {
    return ThrowIf(getter, test, "");
  }
  public static T ThrowIf<T>(Expression<Func<T>> getter, Expression<Predicate<T>> test, string message, params object[] parameters) {
    var r = getter.Compile()();
    if (test.Compile()(r))
      throw new PassageException<T>(getter, r, test, message, parameters);
    return r;
  }
  public static T ThrowIf<T>(this T v, Expression<Predicate<T>> test) {
    return ThrowIfImpl(v, test, "");
  }
  public static T ThrowIf<T>(this T v, Expression<Predicate<T>> test, string message = "", params object[] parameters) {
    return ThrowIfImpl(v, test, message, parameters);
  }
  public static T ThrowIfImpl<T>(T v, Expression<Predicate<T>> test, string message, params object[] parameters) {
    if (test.Compile()(v))
      throw new PassageException<T>(v, test, message, parameters);
    return v;
  }
  public static void ThrowIf(Expression<Func<bool>> test, string message = "", params object[] parameters) {
    if (test.Compile()())
      throw new PassageException(test, message, parameters);
  }
}
