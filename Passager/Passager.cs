﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class PassagerException : Exception {
  public PassagerException(Expression<Func<bool>> test, string message)
    : base($"Validation {TestBody(test)} failed. {message}") {
  }
  public PassagerException(Expression<Func<bool>> test, string message, params object[] parameters)
    : base(string.Format("Validation {{{0}}} failed", TestBody(test)) + string.Format(message, parameters)) {
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
public class PassagerException<TValue> : Exception {
  public PassagerException(TValue value, Expression<Predicate<TValue>> test, string message, params object[] parameters)
    : base(BuildMessage(value, test, message, parameters)) {
  }

  private static string BuildMessage(TValue value, Expression<Predicate<TValue>> test, string message, params object[] parameters) {
    var msg = $"Value <{value}> triggered error condition {test.Body}";
    return msg + (parameters.Any()
      ? (parameters.Any() ? string.Format(message, parameters) : message)
      : "");
  }

  public PassagerException(Expression<Func<TValue>> getter, TValue value, Expression<Predicate<TValue>> test, string message, params object[] parameters)
    : base(
      string.Format("Parameter [{0}]<{1}> triggered error condition {2}.", GetParameterName(getter), value, test.Body)
        + (parameters.Any() ? string.Format(message, parameters) : message)) {
  }
  private static string GetParameterName(Expression reference) {
    var lambda = reference as LambdaExpression;
    var member = lambda.Body as MemberExpression;
    return member.Member.Name;
  }
}
public static class Passager {
  public static T ThrowIf<T>(Expression<Func<T>> getter, Expression<Predicate<T>> test) {
    return ThrowIf(getter, test, "");
  }
  public static T ThrowIf<T>(Expression<Func<T>> getter, Expression<Predicate<T>> test, string message, params object[] parameters) {
    var r = getter.Compile()();
    if (test.Compile()(r))
      throw new PassagerException<T>(getter, r, test, message, parameters);
    return r;
  }
  public static T ThrowIf<T>(this T v, Expression<Predicate<T>> test, string message = "", params object[] parameters) {
    return ThrowIfImpl(v, test, message, parameters);
  }
  public static T ThrowIf<T>(this T v, Expression<Predicate<T>> test) {
    return ThrowIfImpl(v, test, "");
  }
  public static T ThrowIf<T>(this T v, Expression<Func<bool>> test, string message = "", params object[] parameters) {
    if (test.Compile()())
      throw new PassagerException(test, message, parameters);
    return v;
  }
  static T ThrowIfImpl<T>(T v, Expression<Predicate<T>> test, string message, params object[] parameters) {
    if (test.Compile()(v))
      throw new PassagerException<T>(v, test, message, parameters);
    return v;
  }
  public static void ThrowIf(Expression<Func<bool>> test, string message = "", params object[] parameters) {
    if (test.Compile()())
      throw new PassagerException(test, message, parameters);
  }
  public static void ThrowIf(Expression<Func<bool>> test, string message = "") {
    if (test.Compile()())
      throw new PassagerException(test, message);
  }
}
