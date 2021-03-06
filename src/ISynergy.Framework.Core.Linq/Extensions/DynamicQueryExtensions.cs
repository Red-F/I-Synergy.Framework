﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Constants;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Helpers;
using ISynergy.Framework.Core.Linq.Parsers;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Core.Linq.Extensions
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for querying data structures that implement <see cref="IQueryable" />.
    /// It allows dynamic string based querying. Very handy when, at compile time, you don't know the type of queries that will be generated,
    /// or when downstream components only return column names to sort and filter by.
    /// </summary>
    public static class DynamicQueryExtensions
    {
        /// <summary>
        /// The trace source
        /// </summary>
        private static readonly TraceSource TraceSource = new TraceSource(typeof(DynamicQueryExtensions).Name);

        /// <summary>
        /// Optimizes the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Expression.</returns>
        private static Expression OptimizeExpression(Expression expression)
        {
            if (ExtensibilityPoint.QueryOptimizer != null)
            {
                var optimized = ExtensibilityPoint.QueryOptimizer(expression);

                if (optimized != expression)
                {
                    TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Expression before : {0}", expression);
                    TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Expression after  : {0}", optimized);
                }
                return optimized;
            }

            return expression;
        }

        #region Aggregate
        /// <summary>
        /// Dynamically runs an aggregate function on the IQueryable.
        /// </summary>
        /// <param name="source">The IQueryable data source.</param>
        /// <param name="function">The name of the function to run. Can be Sum, Average, Min or Max.</param>
        /// <param name="member">The name of the property to aggregate over.</param>
        /// <returns>The value of the aggregate function run over the specified property.</returns>
        public static object Aggregate(this IQueryable source, string function, string member)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNullOrEmpty(nameof(function), function); 
            Argument.IsNotNullOrEmpty(nameof(member), member); 

            // Properties
            var property = source.ElementType.GetProperty(member);
            var parameter = Expression.Parameter(source.ElementType, "s");
            Expression selector = Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter);
            // We've tried to find an expression of the type Expression<Func<TSource, TAcc>>,
            // which is expressed as ( (TSource s) => s.Price );

            var methods = typeof(Queryable).GetMethods().Where(x => x.Name == function && x.IsGenericMethod);

            // Method
            var aggregateMethod = methods.SingleOrDefault(m =>
            {
                var lastParameter = m.GetParameters().LastOrDefault();

                return lastParameter != null ? TypeHelper.GetUnderlyingType(lastParameter.ParameterType) == property.PropertyType : false;
            });

            // Sum, Average
            if (aggregateMethod != null)
            {
                return source.Provider.Execute(
                    Expression.Call(
                        null,
                        aggregateMethod.MakeGenericMethod(source.ElementType),
                        new[] { source.Expression, Expression.Quote(selector) }));
            }

            // Min, Max
            aggregateMethod = methods.SingleOrDefault(m => m.Name == function && m.GetGenericArguments().Length == 2);

            return source.Provider.Execute(
                Expression.Call(
                    null,
                    aggregateMethod.MakeGenericMethod(source.ElementType, property.PropertyType),
                    new[] { source.Expression, Expression.Quote(selector) }));
        }
        #endregion Aggregate

        #region All
        /// <summary>
        /// All predicate
        /// </summary>
        private static readonly MethodInfo _AllPredicate = GetMethod(nameof(Queryable.All), 1);

        /// <summary>
        /// Determines whether all the elements of a sequence satisfy a condition.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable" /> to calculate the All of.</param>
        /// <param name="predicate">A projection function to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
        /// <remarks>Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        /// that All asynchronous operations have completed before calling another method on this context.</remarks>
        public static bool All(this IQueryable source, string predicate, params object[] args)
        {
            return All(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Determines whether all the elements of a sequence satisfy a condition.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable" /> to calculate the All of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A projection function to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
        /// <remarks>Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        /// that All asynchronous operations have completed before calling another method on this context.</remarks>
        public static bool All(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<bool>(_AllPredicate, source, Expression.Quote(lambda));
        }
        #endregion AllAsync

        #region Any
        /// <summary>
        /// Any
        /// </summary>
        private static readonly MethodInfo _any = GetMethod(nameof(Queryable.Any));

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Any();
        /// </code>
        /// </example>
        public static bool Any(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute<bool>(_any, source);
        }

        /// <summary>
        /// Any predicate
        /// </summary>
        private static readonly MethodInfo _anyPredicate = GetMethod(nameof(Queryable.Any), 1);

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Any("Income &gt; 50");
        /// var result2 = queryable.Any("Income &gt; @0", 50);
        /// var result3 = queryable.Select("Roles.Any()");
        /// </code>
        /// </example>
        public static bool Any(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<bool>(_anyPredicate, source, lambda);
        }

        /// <summary>
        /// Anies the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <inheritdoc cref="Any(IQueryable, ParsingConfig, string, object[])" />
        public static bool Any(this IQueryable source, string predicate, params object[] args)
        {
            return Any(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public static bool Any(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(lambda), lambda); 

            return Execute<bool>(_anyPredicate, source, lambda);
        }
        #endregion Any

        #region Average
        /// <summary>
        /// Computes the average of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the average of.</param>
        /// <returns>The average of the values in the sequence.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Average();
        /// var result2 = queryable.Select("Roles.Average()");
        /// </code>
        /// </example>
        public static double Average(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            var average = GetMethod(nameof(Queryable.Average), source.ElementType, typeof(double));
            return Execute<double>(average, source);
        }

        /// <summary>
        /// Computes the average of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the average of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The average of the values in the sequence.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Average("Income");
        /// </code>
        /// </example>
        public static double Average(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Average(source, lambda);
        }

        /// <summary>
        /// Averages the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Double.</returns>
        /// <inheritdoc cref="Average(IQueryable, ParsingConfig, string, object[])" />
        public static double Average(this IQueryable source, string predicate, params object[] args)
        {
            return Average(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Computes the average of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the average of.</param>
        /// <param name="lambda">A Lambda Expression.</param>
        /// <returns>The average of the values in the sequence.</returns>
        public static double Average(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(lambda), lambda); 

            var averageSelector = GetMethod(nameof(Queryable.Average), lambda.ReturnType, typeof(double), 1);
            return Execute<double>(averageSelector, source, lambda);
        }
        #endregion Average

        #region AsEnumerable
#if NET35
        /// <summary>
        /// Returns the input typed as <see cref="IEnumerable{T}"/> of <see cref="object"/>./>
        /// </summary>
        /// <param name="source">The sequence to type as <see cref="IEnumerable{T}"/> of <see cref="object"/>.</param>
        /// <returns>The input typed as <see cref="IEnumerable{T}"/> of <see cref="object"/>.</returns>
        public static IEnumerable<object> AsEnumerable(this IQueryable source)
#else
        /// <summary>
        /// Returns the input typed as <see cref="IEnumerable{T}" /> of dynamic.
        /// </summary>
        /// <param name="source">The sequence to type as <see cref="IEnumerable{T}" /> of dynamic.</param>
        /// <returns>The input typed as <see cref="IEnumerable{T}" /> of dynamic.</returns>
        public static IEnumerable<dynamic> AsEnumerable(this IQueryable source)
#endif
        {
            foreach (var obj in source)
            {
                yield return obj;
            }
        }
        #endregion AsEnumerable

        #region Cast
        /// <summary>
        /// The cast
        /// </summary>
        private static readonly MethodInfo _cast = GetGenericMethod(nameof(Queryable.Cast));

        /// <summary>
        /// Converts the elements of an <see cref="IQueryable" /> to the specified type.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be converted.</param>
        /// <param name="type">The type to convert the elements of source to.</param>
        /// <returns>An <see cref="IQueryable" /> that contains each element of the source sequence converted to the specified type.</returns>
        public static IQueryable Cast(this IQueryable source, Type type)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(type), type); 

            var optimized = OptimizeExpression(Expression.Call(null, _cast.MakeGenericMethod(new Type[] { type }), new Expression[] { source.Expression }));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Converts the elements of an <see cref="IQueryable" /> to the specified type.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be converted.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="typeName">The type to convert the elements of source to.</param>
        /// <returns>An <see cref="IQueryable" /> that contains each element of the source sequence converted to the specified type.</returns>
        public static IQueryable Cast(this IQueryable source, ParsingConfig config, string typeName)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(typeName), typeName); 

            var finder = new TypeFinder(config, new KeywordsHelper(config));
            var type = finder.FindTypeByName(typeName, null, true);

            return Cast(source, type);
        }

        /// <summary>
        /// Converts the elements of an <see cref="IQueryable" /> to the specified type.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be converted.</param>
        /// <param name="typeName">The type to convert the elements of source to.</param>
        /// <returns>An <see cref="IQueryable" /> that contains each element of the source sequence converted to the specified type.</returns>
        public static IQueryable Cast(this IQueryable source, string typeName)
        {
            return Cast(source, ParsingConfig.Default, typeName);
        }
        #endregion Cast

        #region Count
        /// <summary>
        /// The count
        /// </summary>
        private static readonly MethodInfo _count = GetMethod(nameof(Queryable.Count));

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Count();
        /// </code>
        /// </example>
        public static int Count(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute<int>(_count, source);
        }

        /// <summary>
        /// The count predicate
        /// </summary>
        private static readonly MethodInfo _countPredicate = GetMethod(nameof(Queryable.Count), 1);

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be counted.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Count("Income &gt; 50");
        /// var result2 = queryable.Count("Income &gt; @0", 50);
        /// var result3 = queryable.Select("Roles.Count()");
        /// </code>
        /// </example>
        public static int Count(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<int>(_countPredicate, source, lambda);
        }

        /// <summary>
        /// Counts the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        /// <inheritdoc cref="Count(IQueryable, ParsingConfig, string, object[])" />
        public static int Count(this IQueryable source, string predicate, params object[] args)
        {
            return Count(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be counted.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        public static int Count(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(lambda), lambda); 

            return Execute<int>(_countPredicate, source, lambda);
        }
        #endregion Count

        #region DefaultIfEmpty
        /// <summary>
        /// The default if empty
        /// </summary>
        private static readonly MethodInfo _defaultIfEmpty = GetMethod(nameof(Queryable.DefaultIfEmpty));
        /// <summary>
        /// The default if empty with parameter
        /// </summary>
        private static readonly MethodInfo _defaultIfEmptyWithParam = GetMethod(nameof(Queryable.DefaultIfEmpty), 1);

        /// <summary>
        /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return a default value for if empty.</param>
        /// <returns>An <see cref="IQueryable" /> that contains default if source is empty; otherwise, source.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.DefaultIfEmpty();
        /// </code>
        /// </example>
        public static IQueryable DefaultIfEmpty(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return CreateQuery(_defaultIfEmpty, source);
        }

        /// <summary>
        /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return a default value for if empty.</param>
        /// <param name="defaultValue">The value to return if the sequence is empty.</param>
        /// <returns>An <see cref="IQueryable" /> that contains defaultValue if source is empty; otherwise, source.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.DefaultIfEmpty(new Employee());
        /// </code>
        /// </example>
        public static IQueryable DefaultIfEmpty(this IQueryable source, object defaultValue)
        {
            Argument.IsNotNull(nameof(source), source); 

            return CreateQuery(_defaultIfEmptyWithParam, source, Expression.Constant(defaultValue));
        }
        #endregion

        #region Distinct
        /// <summary>
        /// The distinct
        /// </summary>
        private static readonly MethodInfo _distinct = GetMethod(nameof(Queryable.Distinct));

        /// <summary>
        /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
        /// </summary>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <returns>An <see cref="IQueryable" /> that contains distinct elements from the source sequence.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Distinct();
        /// var result2 = queryable.Select("Roles.Distinct()");
        /// </code>
        /// </example>
        public static IQueryable Distinct(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return CreateQuery(_distinct, source);
        }
        #endregion Distinct

        #region First
        /// <summary>
        /// The first
        /// </summary>
        private static readonly MethodInfo _first = GetMethod(nameof(Queryable.First));

        /// <summary>
        /// Returns the first element of a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the first element of.</param>
        /// <returns>The first element in source.</returns>
#if NET35
        public static object First(this IQueryable source)
#else
        public static dynamic First(this IQueryable source)
#endif
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute(_first, source);
        }

        /// <summary>
        /// The first predicate
        /// </summary>
        private static readonly MethodInfo _firstPredicate = GetMethod(nameof(Queryable.First), 1);

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the first element of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object First(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
#else
        public static dynamic First(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
#endif
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_firstPredicate, source, lambda);
        }

        /// <summary>
        /// Firsts the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>dynamic.</returns>
        /// <inheritdoc cref="First(IQueryable, ParsingConfig, string, object[])" />
#if NET35
        public static object First(this IQueryable source, string predicate, params object[] args)
#else
        public static dynamic First(this IQueryable source, string predicate, params object[] args)
#endif
        {
            return First(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the first element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object First(this IQueryable source, LambdaExpression lambda)
#else
        public static dynamic First(this IQueryable source, LambdaExpression lambda)
#endif
        {
            Argument.IsNotNull(nameof(source), source); 
            return Execute(_firstPredicate, source, lambda);
        }
        #endregion First

        #region FirstOrDefault
        /// <summary>
        /// The first or default
        /// </summary>
        private static readonly MethodInfo _firstOrDefault = GetMethod(nameof(Queryable.FirstOrDefault));

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the first element of.</param>
        /// <returns>default if source is empty; otherwise, the first element in source.</returns>
        public static dynamic FirstOrDefault(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute(_firstOrDefault, source);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the first element of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
        public static dynamic FirstOrDefault(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_firstOrDefaultPredicate, source, lambda);
        }

        /// <summary>
        /// Firsts the or default.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>dynamic.</returns>
        /// <inheritdoc cref="FirstOrDefault(IQueryable, ParsingConfig, string, object[])" />
        public static dynamic FirstOrDefault(this IQueryable source, string predicate, params object[] args)
        {
            return FirstOrDefault(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the first element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
        public static dynamic FirstOrDefault(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute(_firstOrDefaultPredicate, source, lambda);
        }
        /// <summary>
        /// The first or default predicate
        /// </summary>
        private static readonly MethodInfo _firstOrDefaultPredicate = GetMethod(nameof(Queryable.FirstOrDefault), 1);
        #endregion FirstOrDefault

        #region GroupBy
        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="resultSelector">A string expression to specify a result value from each group.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable" /> where each element represents a projection over a group and its key.</returns>
        /// <example>
        ///   <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy(this IQueryable source, ParsingConfig config, string keySelector, string resultSelector, object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(keySelector), keySelector); 
            Argument.IsNotNullOrEmpty(nameof(resultSelector), resultSelector); 

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var keyLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, keySelector, args);
            var elementLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, resultSelector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.GroupBy),
                new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, string, object[])" />
        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, object[] args)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, resultSelector, args);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="resultSelector">A string expression to specify a result value from each group.</param>
        /// <returns>A <see cref="IQueryable" /> where each element represents a projection over a group and its key.</returns>
        /// <example>
        ///   <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy(this IQueryable source, ParsingConfig config, string keySelector, string resultSelector)
        {
            return GroupBy(source, config, keySelector, resultSelector, null);
        }

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, string)" />
        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, resultSelector);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable" /> where each element represents a projection over a group and its key.</returns>
        /// <example>
        ///   <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy(this IQueryable source, ParsingConfig config, string keySelector, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(keySelector), keySelector); 

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var keyLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, keySelector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.GroupBy),
                new[] { source.ElementType, keyLambda.Body.Type }, source.Expression, Expression.Quote(keyLambda)));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable GroupBy(this IQueryable source, string keySelector, params object[] args)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, args);
        }
        #endregion GroupBy

        #region GroupByMany
        /// <summary>
        /// Groups the elements of a sequence according to multiple specified key string functions
        /// and creates a result value from each group (and subgroups) and its key.
        /// </summary>
        /// <typeparam name="TElement">The type of the t element.</typeparam>
        /// <param name="source">A <see cref="IEnumerable{T}" /> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="keySelectors"><see cref="string" /> expressions to specify the keys for each element.</param>
        /// <returns>A <see cref="IEnumerable{T}" /> of type <see cref="GroupResult" /> where each element represents a projection over a group, its key, and its subgroups.</returns>
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> source, ParsingConfig config, params string[] keySelectors)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.HasNoNulls(nameof(keySelectors), keySelectors);

            var selectors = new List<Func<TElement, object>>(keySelectors.Length);

            var createParameterCtor = true;
            foreach (var selector in keySelectors)
            {
                var l = DynamicExpressionParser.ParseLambda(config, createParameterCtor, typeof(TElement), typeof(object), selector);
                selectors.Add((Func<TElement, object>)l.Compile());
            }

            return GroupByManyInternal(source, selectors.ToArray(), 0);
        }

        /// <summary>
        /// Groups the by many.
        /// </summary>
        /// <typeparam name="TElement">The type of the t element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelectors">The key selectors.</param>
        /// <returns>IEnumerable&lt;GroupResult&gt;.</returns>
        /// <inheritdoc cref="GroupByMany{TElement}(IEnumerable{TElement}, ParsingConfig, string[])" />
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> source, params string[] keySelectors)
        {
            return GroupByMany(source, ParsingConfig.Default, keySelectors);
        }

        /// <summary>
        /// Groups the elements of a sequence according to multiple specified key functions
        /// and creates a result value from each group (and subgroups) and its key.
        /// </summary>
        /// <typeparam name="TElement">The type of the t element.</typeparam>
        /// <param name="source">A <see cref="IEnumerable{T}" /> whose elements to group.</param>
        /// <param name="keySelectors">Lambda expressions to specify the keys for each element.</param>
        /// <returns>A <see cref="IEnumerable{T}" /> of type <see cref="GroupResult" /> where each element represents a projection over a group, its key, and its subgroups.</returns>
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> source, params Func<TElement, object>[] keySelectors)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.HasNoNulls(nameof(keySelectors), keySelectors);

            return GroupByManyInternal(source, keySelectors, 0);
        }

        /// <summary>
        /// Groups the by many internal.
        /// </summary>
        /// <typeparam name="TElement">The type of the t element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelectors">The key selectors.</param>
        /// <param name="currentSelector">The current selector.</param>
        /// <returns>IEnumerable&lt;GroupResult&gt;.</returns>
        private static IEnumerable<GroupResult> GroupByManyInternal<TElement>(IEnumerable<TElement> source, Func<TElement, object>[] keySelectors, int currentSelector)
        {
            if (currentSelector >= keySelectors.Length)
            {
                return null;
            }

            var selector = keySelectors[currentSelector];

            var result = source.GroupBy(selector).Select(
                g => new GroupResult
                {
                    Key = g.Key,
                    Count = g.Count(),
                    Items = g,
                    Subgroups = GroupByManyInternal(g, keySelectors, currentSelector + 1)
                });

            return result;
        }
        #endregion GroupByMany

        #region GroupJoin
        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> obtained by performing a grouped join on two sequences.</returns>
        public static IQueryable GroupJoin(this IQueryable outer, ParsingConfig config, IEnumerable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            Argument.IsNotNull(nameof(outer), outer); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNull(nameof(inner), inner); 
            Argument.IsNotNullOrEmpty(nameof(outerKeySelector), outerKeySelector); 
            Argument.IsNotNullOrEmpty(nameof(innerKeySelector), innerKeySelector); 
            Argument.IsNotNullOrEmpty(nameof(resultSelector), resultSelector); 

            var outerType = outer.ElementType;
            var innerType = inner.AsQueryable().ElementType;

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, outer);
            var outerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, outerType, null, outerKeySelector, args);
            var innerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, innerType, null, innerKeySelector, args);

            CheckOuterAndInnerTypes(config, createParameterCtor, outerType, innerType, outerKeySelector, innerKeySelector, ref outerSelectorLambda, ref innerSelectorLambda, args);

            ParameterExpression[] parameters =
            {
                Expression.Parameter(outerType, "outer"),
                Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(innerType), "inner")
            };

            var resultSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, parameters, null, resultSelector, args);

            return outer.Provider.CreateQuery(Expression.Call(
                typeof(Queryable), nameof(Queryable.GroupJoin),
                new[] { outer.ElementType, innerType, outerSelectorLambda.Body.Type, resultSelectorLambda.Body.Type },
                outer.Expression,
                inner.AsQueryable().Expression,
                Expression.Quote(outerSelectorLambda),
                Expression.Quote(innerSelectorLambda),
                Expression.Quote(resultSelectorLambda)));
        }

        /// <summary>
        /// Groups the join.
        /// </summary>
        /// <param name="outer">The outer.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="outerKeySelector">The outer key selector.</param>
        /// <param name="innerKeySelector">The inner key selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="GroupJoin(IQueryable, ParsingConfig, IEnumerable, string, string, string, object[])" />
        public static IQueryable GroupJoin(this IQueryable outer, IEnumerable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            return GroupJoin(outer, ParsingConfig.Default, inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }
        #endregion

        #region Join
        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from two matching elements.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> obtained by performing an inner join on two sequences.</returns>
        public static IQueryable Join(this IQueryable outer, ParsingConfig config, IEnumerable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            //http://stackoverflow.com/questions/389094/how-to-create-a-dynamic-linq-join-extension-method

            Argument.IsNotNull(nameof(outer), outer); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNull(nameof(inner), inner); 
            Argument.IsNotNullOrEmpty(nameof(outerKeySelector), outerKeySelector); 
            Argument.IsNotNullOrEmpty(nameof(innerKeySelector), innerKeySelector); 
            Argument.IsNotNullOrEmpty(nameof(resultSelector), resultSelector); 

            var outerType = outer.ElementType;
            var innerType = inner.AsQueryable().ElementType;

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, outer);
            var outerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, outerType, null, outerKeySelector, args);
            var innerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, innerType, null, innerKeySelector, args);

            CheckOuterAndInnerTypes(config, createParameterCtor, outerType, innerType, outerKeySelector, innerKeySelector, ref outerSelectorLambda, ref innerSelectorLambda, args);

            ParameterExpression[] parameters =
            {
                Expression.Parameter(outerType, "outer"),
                Expression.Parameter(innerType, "inner")
            };

            var resultSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, parameters, null, resultSelector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), "Join",
                new[] { outerType, innerType, outerSelectorLambda.Body.Type, resultSelectorLambda.Body.Type },
                outer.Expression, // outer: The first sequence to join.
                inner.AsQueryable().Expression, // inner: The sequence to join to the first sequence.
                Expression.Quote(outerSelectorLambda), // outerKeySelector: A function to extract the join key from each element of the first sequence.
                Expression.Quote(innerSelectorLambda), // innerKeySelector: A function to extract the join key from each element of the second sequence.
                Expression.Quote(resultSelectorLambda) // resultSelector: A function to create a result element from two matching elements.
            ));

            return outer.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Joins the specified inner.
        /// </summary>
        /// <param name="outer">The outer.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="outerKeySelector">The outer key selector.</param>
        /// <param name="innerKeySelector">The inner key selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="Join(IQueryable, ParsingConfig, IEnumerable, string, string, string, object[])" />
        public static IQueryable Join(this IQueryable outer, IEnumerable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            return Join(outer, ParsingConfig.Default, inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements of both sequences, and the result.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from two matching elements.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable{T}" /> that has elements of type TResult obtained by performing an inner join on two sequences.</returns>
        /// <remarks>This overload only works on elements where both sequences and the resulting element match.</remarks>
        public static IQueryable<TElement> Join<TElement>(this IQueryable<TElement> outer, ParsingConfig config, IEnumerable<TElement> inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            return (IQueryable<TElement>)Join(outer, config, (IEnumerable)inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }

        /// <summary>
        /// Joins the specified inner.
        /// </summary>
        /// <typeparam name="TElement">The type of the t element.</typeparam>
        /// <param name="outer">The outer.</param>
        /// <param name="inner">The inner.</param>
        /// <param name="outerKeySelector">The outer key selector.</param>
        /// <param name="innerKeySelector">The inner key selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable&lt;TElement&gt;.</returns>
        /// <inheritdoc cref="Join{TElement}(IQueryable{TElement}, ParsingConfig, IEnumerable{TElement}, string, string, string, object[])" />
        public static IQueryable<TElement> Join<TElement>(this IQueryable<TElement> outer, IEnumerable<TElement> inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            return Join(outer, ParsingConfig.Default, inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }
        #endregion Join

        #region Last
        /// <summary>
        /// The last
        /// </summary>
        private static readonly MethodInfo _last = GetMethod(nameof(Queryable.Last));
        /// <summary>
        /// Returns the last element of a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <returns>The last element in source.</returns>
        public static dynamic Last(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute(_last, source);
        }

        /// <summary>
        /// The last predicate
        /// </summary>
        private static readonly MethodInfo _lastPredicate = GetMethod(nameof(Queryable.Last), 1);

        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic Last(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_lastPredicate, source, lambda);
        }

        /// <summary>
        /// Lasts the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>dynamic.</returns>
        /// <inheritdoc cref="Last(IQueryable, ParsingConfig, string, object[])" />
        public static dynamic Last(this IQueryable source, string predicate, params object[] args)
        {
            return Last(source, ParsingConfig.Default, predicate, args);
        }


        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic Last(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            return Execute(_lastPredicate, source, lambda);
        }
        #endregion Last

        #region LastOrDefault
        /// <summary>
        /// The last default
        /// </summary>
        private static readonly MethodInfo _lastDefault = GetMethod(nameof(Queryable.LastOrDefault));
        /// <summary>
        /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <returns>default if source is empty; otherwise, the last element in source.</returns>
        public static dynamic LastOrDefault(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute(_lastDefault, source);
        }

        /// <summary>
        /// The last default predicate
        /// </summary>
        private static readonly MethodInfo _lastDefaultPredicate = GetMethod(nameof(Queryable.LastOrDefault), 1);

        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic LastOrDefault(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_lastDefaultPredicate, source, lambda);
        }

        /// <summary>
        /// Lasts the or default.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>dynamic.</returns>
        /// <inheritdoc cref="LastOrDefault(IQueryable, ParsingConfig, string, object[])" />
        public static dynamic LastOrDefault(this IQueryable source, string predicate, params object[] args)
        {
            return LastOrDefault(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic LastOrDefault(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            return Execute(_lastDefaultPredicate, source, lambda);
        }
        #endregion LastOrDefault

        #region LongCount
        /// <summary>
        /// The long count
        /// </summary>
        private static readonly MethodInfo _longCount = GetMethod(nameof(Queryable.LongCount));

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.LongCount();
        /// </code>
        /// </example>
        public static long LongCount(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Execute<long>(_longCount, source);
        }

        /// <summary>
        /// The long count predicate
        /// </summary>
        private static readonly MethodInfo _longCountPredicate = GetMethod(nameof(Queryable.LongCount), 1);

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be counted.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.LongCount("Income &gt; 50");
        /// var result2 = queryable.LongCount("Income &gt; @0", 50);
        /// var result3 = queryable.Select("Roles.LongCount()");
        /// </code>
        /// </example>
        public static long LongCount(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<long>(_longCountPredicate, source, lambda);
        }

        /// <summary>
        /// Longs the count.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int64.</returns>
        /// <inheritdoc cref="LongCount(IQueryable, ParsingConfig, string, object[])" />
        public static long LongCount(this IQueryable source, string predicate, params object[] args)
        {
            return LongCount(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> that contains the elements to be counted.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        public static long LongCount(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(lambda), lambda); 

            return Execute<long>(_longCountPredicate, source, lambda);
        }
        #endregion LongCount

        #region OfType
        /// <summary>
        /// The of type
        /// </summary>
        private static readonly MethodInfo _ofType = GetGenericMethod(nameof(Queryable.OfType));

        /// <summary>
        /// Filters the elements of an <see cref="IQueryable" /> based on a specified type.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable" /> whose elements to filter.</param>
        /// <param name="type">The type to filter the elements of the sequence on.</param>
        /// <returns>A collection that contains the elements from source that have the type.</returns>
        public static IQueryable OfType(this IQueryable source, Type type)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(type), type); 

            var optimized = OptimizeExpression(Expression.Call(null, _ofType.MakeGenericMethod(new Type[] { type }), new Expression[] { source.Expression }));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Filters the elements of an <see cref="IQueryable" /> based on a specified type.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable" /> whose elements to filter.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="typeName">The type to filter the elements of the sequence on.</param>
        /// <returns>A collection that contains the elements from source that have the type.</returns>
        public static IQueryable OfType(this IQueryable source, ParsingConfig config, string typeName)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(typeName), typeName); 

            var finder = new TypeFinder(config, new KeywordsHelper(config));
            var type = finder.FindTypeByName(typeName, null, true);

            return OfType(source, type);
        }

        /// <summary>
        /// Filters the elements of an <see cref="IQueryable" /> based on a specified type.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable" /> whose elements to filter.</param>
        /// <param name="typeName">The type to filter the elements of the sequence on.</param>
        /// <returns>A collection that contains the elements from source that have the type.</returns>
        public static IQueryable OfType(this IQueryable source, string typeName)
        {
            return OfType(source, ParsingConfig.Default, typeName);
        }
        #endregion OfType


        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}" /> whose elements are sorted according to the specified <paramref name="ordering" />.</returns>
        /// <example>
        ///   <code><![CDATA[
        /// var resultSingle = queryable.OrderBy<User>("NumberProperty");
        /// var resultSingleDescending = queryable.OrderBy<User>("NumberProperty DESC");
        /// var resultMultiple = queryable.OrderBy<User>("NumberProperty, StringProperty");
        /// ]]></code>
        /// </example>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, ParsingConfig config, string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)OrderBy((IQueryable)source, config, ordering, args);
        }

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="ordering">The ordering.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IOrderedQueryable&lt;TSource&gt;.</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string ordering, params object[] args)
        {
            return OrderBy(source, ParsingConfig.Default, ordering, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable" /> whose elements are sorted according to the specified <paramref name="ordering" />.</returns>
        /// <example>
        ///   <code>
        /// var resultSingle = queryable.OrderBy("NumberProperty");
        /// var resultSingleDescending = queryable.OrderBy("NumberProperty DESC");
        /// var resultMultiple = queryable.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable OrderBy(this IQueryable source, ParsingConfig config, string ordering, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.IsNotNull(nameof(config), config);
            Argument.IsNotNullOrEmpty(nameof(ordering), ordering);

            ParameterExpression[] parameters = { Expression.Parameter(source.ElementType, string.Empty) };
            var parser = new ExpressionParser(parameters, ordering, args, config);
            var dynamicOrderings = parser.ParseOrdering();

            var queryExpr = source.Expression;

            foreach (var dynamicOrdering in dynamicOrderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), dynamicOrdering.MethodName,
                    new[] { source.ElementType, dynamicOrdering.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)));
            }

            var optimized = OptimizeExpression(queryExpr);
            return (IOrderedQueryable)source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="ordering">The ordering.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IOrderedQueryable.</returns>
        /// <inheritdoc cref="OrderBy(IQueryable, ParsingConfig, string, object[])" />
        public static IOrderedQueryable OrderBy(this IQueryable source, string ordering, params object[] args)
        {
            return OrderBy(source, ParsingConfig.Default, ordering, args);
        }

        #region Page/PageResult
        /// <summary>
        /// Returns the elements as paged.
        /// </summary>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>A <see cref="IQueryable" /> that contains the paged elements.</returns>
        public static IQueryable Page(this IQueryable source, int page, int pageSize)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.Condition(nameof(page), page, p => p > 0);
            Argument.Condition(nameof(pageSize), pageSize, ps => ps > 0);

            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Returns the elements as paged.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>A <see cref="IQueryable{TSource}" /> that contains the paged elements.</returns>
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.Condition(nameof(page), page, p => p > 0);
            Argument.Condition(nameof(pageSize), pageSize, ps => ps > 0);

            return Queryable.Take(Queryable.Skip(source, (page - 1) * pageSize), pageSize);
        }

        /// <summary>
        /// Returns the elements as paged and include the CurrentPage, PageCount, PageSize and RowCount.
        /// </summary>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>PagedResult</returns>
        public static PagedResult PageResult(this IQueryable source, int page, int pageSize)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.Condition(nameof(page), page, p => p > 0);
            Argument.Condition(nameof(pageSize), pageSize, ps => ps > 0);

            var result = new PagedResult
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = source.Count()
            };

            result.PageCount = (int)Math.Ceiling((double)result.RowCount / pageSize);
            result.Queryable = Page(source, page, pageSize);

            return result;
        }

        /// <summary>
        /// Returns the elements as paged and include the CurrentPage, PageCount, PageSize and RowCount.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>PagedResult{TSource}</returns>
        public static PagedResult<TSource> PageResult<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.Condition(nameof(page), page, p => p > 0);
            Argument.Condition(nameof(pageSize), pageSize, ps => ps > 0);

            var result = new PagedResult<TSource>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = Queryable.Count(source)
            };

            result.PageCount = (int)Math.Ceiling((double)result.RowCount / pageSize);
            result.Queryable = Page(source, page, pageSize);

            return result;
        }
        #endregion Page/PageResult

        #region Reverse
        /// <summary>
        /// Inverts the order of the elements in a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to reverse.</param>
        /// <returns>A <see cref="IQueryable" /> whose elements correspond to those of the input sequence in reverse order.</returns>
        public static IQueryable Reverse(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            return Queryable.Reverse((IQueryable<object>)source);
        }
        #endregion Reverse

        #region Select
        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        ///   <code>
        /// var singleField = queryable.Select("StringProperty");
        /// var dynamicObject = queryable.Select("new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable Select(this IQueryable source, ParsingConfig config, string selector, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(selector), selector); 

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Selects the specified selector.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="Select(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable Select(this IQueryable source, string selector, params object[] args)
        {
            return Select(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see <see href="http://solutionizing.net/category/linq/" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
        /// <returns>An <see cref="IQueryable{TResult}" /> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        ///   <code language="cs"><![CDATA[
        /// var users = queryable.Select<User>("new (Username, Pwd as Password)");
        /// ]]></code>
        /// </example>
        public static IQueryable<TResult> Select<TResult>(this IQueryable source, ParsingConfig config, string selector, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(selector), selector); 

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, typeof(TResult), selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, typeof(TResult) },
                source.Expression, Expression.Quote(lambda)));

            return source.Provider.CreateQuery<TResult>(optimized);
        }

        /// <summary>
        /// Selects the specified selector.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable&lt;TResult&gt;.</returns>
        /// <inheritdoc cref="Select{TResult}(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable<TResult> Select<TResult>(this IQueryable source, string selector, params object[] args)
        {
            return Select<TResult>(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see http://solutionizing.net/category/linq/
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="resultType">The result type.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
        /// <returns>An <see cref="IQueryable" /> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        ///   <code>
        /// var users = queryable.Select(typeof(User), "new (Username, Pwd as Password)");
        /// </code>
        /// </example>
        public static IQueryable Select(this IQueryable source, ParsingConfig config, Type resultType, string selector, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNull(nameof(resultType), resultType); 
            Argument.IsNotNullOrEmpty(nameof(selector), selector); 

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, resultType, selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, resultType },
                source.Expression, Expression.Quote(lambda)));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Selects the specified result type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="Select(IQueryable, ParsingConfig, Type, string, object[])" />
        public static IQueryable Select(this IQueryable source, Type resultType, string selector, params object[] args)
        {
            return Select(source, ParsingConfig.Default, resultType, selector, args);
        }

        #endregion Select

        #region SelectMany
        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable" /> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        ///   <code>
        /// var roles = users.SelectMany("Roles");
        /// </code>
        /// </example>
        public static IQueryable SelectMany(this IQueryable source, ParsingConfig config, string selector, params object[] args)
        {
            return SelectManyInternal(source, config, null, selector, args);
        }

        /// <summary>
        /// Selects the many.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable SelectMany(this IQueryable source, string selector, params object[] args)
        {
            return SelectMany(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable" /> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="resultType">The result type.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        ///   <code>
        /// var permissions = users.SelectMany(typeof(Permission), "Roles.SelectMany(Permissions)");
        /// </code>
        /// </example>
        public static IQueryable SelectMany(this IQueryable source, ParsingConfig config, Type resultType, string selector, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNull(nameof(resultType), resultType); 
            Argument.IsNotNullOrEmpty(nameof(selector), selector); 

            return SelectManyInternal(source, config, resultType, selector, args);
        }

        /// <summary>
        /// Selects the many.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, Type, string, object[])" />
        public static IQueryable SelectMany(this IQueryable source, Type resultType, string selector, params object[] args)
        {
            return SelectMany(source, ParsingConfig.Default, resultType, selector, args);
        }

        /// <summary>
        /// Selects the many internal.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        private static IQueryable SelectManyInternal(IQueryable source, ParsingConfig config, Type resultType, string selector, params object[] args)
        {
            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, selector, args);

            //Extra help to get SelectMany to work from StackOverflow Answer
            //http://stackoverflow.com/a/3001674/2465182

            // if resultType is not specified, create one based on the lambda.Body.Type
            if (resultType == null)
            {
                // SelectMany assumes that lambda.Body.Type is a generic type and throws an exception on
                // lambda.Body.Type.GetGenericArguments()[0] when used over an array as GetGenericArguments() returns an empty array.
                if (lambda.Body.Type.IsArray)
                {
                    resultType = lambda.Body.Type.GetElementType();
                }
                else
                {
                    resultType = lambda.Body.Type.GetGenericArguments()[0];
                }
            }

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(resultType);
            var inputType = source.Expression.Type.GetTypeInfo().GenericTypeArguments[0];
            var delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.SelectMany),
                new[] { source.ElementType, resultType },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable{TResult}" /> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable{TResult}" /> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        ///   <code><![CDATA[
        /// var permissions = users.SelectMany<Permission>("Roles.SelectMany(Permissions)");
        /// ]]></code>
        /// </example>
        public static IQueryable<TResult> SelectMany<TResult>(this IQueryable source, ParsingConfig config, string selector, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(selector), selector); 

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            var inputType = source.Expression.Type.GetTypeInfo().GenericTypeArguments[0];
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(TResult));
            var delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.SelectMany),
                new[] { source.ElementType, typeof(TResult) },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery<TResult>(optimized);
        }

        /// <summary>
        /// Selects the many.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable&lt;TResult&gt;.</returns>
        /// <inheritdoc cref="SelectMany{TResult}(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable<TResult> SelectMany<TResult>(this IQueryable source, string selector, params object[] args)
        {
            return SelectMany<TResult>(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable" />
        /// and invokes a result selector function on each element therein. The resulting
        /// values from each intermediate sequence are combined into a single, one-dimensional
        /// sequence and returned.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="collectionSelector">A projection function to apply to each element of the input sequence.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each intermediate sequence. Should only use x and y as parameter names.</param>
        /// <param name="collectionSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="resultSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> whose elements are the result of invoking the one-to-many
        /// projection function <paramref name="collectionSelector" /> on each element of source and then mapping
        /// each of those sequence elements and their corresponding source element to a result element.</returns>
        /// <example>
        ///   <code><![CDATA[
        /// // TODO
        /// ]]></code>
        /// </example>
        public static IQueryable SelectMany(this IQueryable source, ParsingConfig config, string collectionSelector, string resultSelector, object[] collectionSelectorArgs = null, params object[] resultSelectorArgs)
        {
            return SelectMany(source, collectionSelector, resultSelector, "x", "y", collectionSelectorArgs, resultSelectorArgs);
        }

        /// <summary>
        /// Selects the many.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="collectionSelector">The collection selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="collectionSelectorArgs">The collection selector arguments.</param>
        /// <param name="resultSelectorArgs">The result selector arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, string, string, string, string, object[], object[])" />
        public static IQueryable SelectMany(this IQueryable source, string collectionSelector, string resultSelector, object[] collectionSelectorArgs = null, params object[] resultSelectorArgs)
        {
            return SelectMany(source, ParsingConfig.Default, collectionSelector, resultSelector, "x", "y", collectionSelectorArgs, resultSelectorArgs);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable" />
        /// and invokes a result selector function on each element therein. The resulting
        /// values from each intermediate sequence are combined into a single, one-dimensional
        /// sequence and returned.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="collectionSelector">A projection function to apply to each element of the input sequence.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each intermediate sequence.</param>
        /// <param name="collectionParameterName">The name from collectionParameter to use. Default is x.</param>
        /// <param name="resultParameterName">The name from resultParameterName to use. Default is y.</param>
        /// <param name="collectionSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="resultSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> whose elements are the result of invoking the one-to-many
        /// projection function <paramref name="collectionSelector" /> on each element of source and then mapping
        /// each of those sequence elements and their corresponding source element to a result element.</returns>
        /// <example>
        ///   <code><![CDATA[
        /// // TODO
        /// ]]></code>
        /// </example>
        public static IQueryable SelectMany(this IQueryable source, ParsingConfig config, string collectionSelector, string resultSelector, string collectionParameterName, string resultParameterName, object[] collectionSelectorArgs = null, params object[] resultSelectorArgs)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(collectionSelector), collectionSelector); 
            Argument.IsNotNullOrEmpty(nameof(collectionParameterName), collectionParameterName); 
            Argument.IsNotNullOrEmpty(nameof(resultSelector), resultSelector); 
            Argument.IsNotNullOrEmpty(nameof(resultParameterName), resultParameterName); 

            var createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            var sourceSelectLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, collectionSelector, collectionSelectorArgs);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            var sourceLambdaInputType = source.Expression.Type.GetGenericArguments()[0];
            var sourceLambdaResultType = sourceSelectLambda.Body.Type.GetGenericArguments()[0];
            var sourceLambdaEnumerableType = typeof(IEnumerable<>).MakeGenericType(sourceLambdaResultType);
            var sourceLambdaDelegateType = typeof(Func<,>).MakeGenericType(sourceLambdaInputType, sourceLambdaEnumerableType);

            sourceSelectLambda = Expression.Lambda(sourceLambdaDelegateType, sourceSelectLambda.Body, sourceSelectLambda.Parameters);

            //we have to create additional lambda for result selection
            var xParameter = Expression.Parameter(source.ElementType, collectionParameterName);
            var yParameter = Expression.Parameter(sourceLambdaResultType, resultParameterName);

            var resultSelectLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, new[] { xParameter, yParameter }, null, resultSelector, resultSelectorArgs);
            var resultLambdaResultType = resultSelectLambda.Body.Type;

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.SelectMany),
                new[] { source.ElementType, sourceLambdaResultType, resultLambdaResultType },
                source.Expression, Expression.Quote(sourceSelectLambda), Expression.Quote(resultSelectLambda))
            );

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Selects the many.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="collectionSelector">The collection selector.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <param name="collectionParameterName">Name of the collection parameter.</param>
        /// <param name="resultParameterName">Name of the result parameter.</param>
        /// <param name="collectionSelectorArgs">The collection selector arguments.</param>
        /// <param name="resultSelectorArgs">The result selector arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, string, string, string, string, object[], object[])" />
        public static IQueryable SelectMany(this IQueryable source, string collectionSelector, string resultSelector, string collectionParameterName, string resultParameterName, object[] collectionSelectorArgs = null, params object[] resultSelectorArgs)
        {
            return SelectMany(source, ParsingConfig.Default, collectionSelector, resultSelector, collectionParameterName, resultParameterName, collectionSelectorArgs, resultSelectorArgs);
        }

        #endregion SelectMany

        #region Single/SingleOrDefault
        /// <summary>
        /// Returns the only element of a sequence, and throws an exception if there
        /// is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> to return the single element of.</param>
        /// <returns>The single element of the input sequence.</returns>
        public static dynamic Single(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.Single), new[] { source.ElementType }, source.Expression));
            return source.Provider.Execute(optimized);
        }

        /// <summary>
        /// The single predicate
        /// </summary>
        private static readonly MethodInfo _singlePredicate = GetMethod(nameof(Queryable.Single), 1);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
        /// is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic Single(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_singlePredicate, source, lambda);
        }

        /// <summary>
        /// Singles the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>dynamic.</returns>
        /// <inheritdoc cref="Single(IQueryable, ParsingConfig, string, object[])" />
        public static dynamic Single(this IQueryable source, string predicate, params object[] args)
        {
            return Single(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
        /// is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic Single(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            return Execute(_singlePredicate, source, lambda);
        }

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence
        /// is empty; this method throws an exception if there is more than one element
        /// in the sequence.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> to return the single element of.</param>
        /// <returns>The single element of the input sequence, or default if the sequence contains no elements.</returns>
        public static dynamic SingleOrDefault(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.SingleOrDefault), new[] { source.ElementType }, source.Expression));
            return source.Provider.Execute(optimized);
        }

        /// <summary>
        /// The single default predicate
        /// </summary>
        private static readonly MethodInfo _singleDefaultPredicate = GetMethod(nameof(Queryable.SingleOrDefault), 1);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
        /// is empty; and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic SingleOrDefault(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_singleDefaultPredicate, source, lambda);
        }

        /// <summary>
        /// Singles the or default.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>dynamic.</returns>
        /// <inheritdoc cref="SingleOrDefault(IQueryable, ParsingConfig, string, object[])" />
        public static dynamic SingleOrDefault(this IQueryable source, string predicate, params object[] args)
        {
            return SingleOrDefault(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
        /// is empty; and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable" /> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        public static dynamic SingleOrDefault(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(lambda), lambda); 

            return Execute(_singleDefaultPredicate, source, lambda);
        }
        #endregion Single/SingleOrDefault

        #region Skip
        /// <summary>
        /// The skip
        /// </summary>
        private static readonly MethodInfo _skip = GetMethod(nameof(Queryable.Skip), 1);

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> to return elements from.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A <see cref="IQueryable" /> that contains elements that occur after the specified index in the input sequence.</returns>
        public static IQueryable Skip(this IQueryable source, int count)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.Condition(nameof(count), count, x => x >= 0);

            //no need to skip if count is zero
            if (count == 0)
                return source;

            return CreateQuery(_skip, source, Expression.Constant(count));
        }
        #endregion Skip

        #region SkipWhile

        /// <summary>
        /// The skip while predicate
        /// </summary>
        private static readonly MethodInfo _skipWhilePredicate = GetMethod(nameof(Queryable.SkipWhile), 1, mi =>
        {
            return mi.GetParameters().Length == 2 &&
                   mi.GetParameters()[1].ParameterType.GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>);
        });

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> that contains elements from source starting at the first element in the linear series that does not pass the test specified by predicate.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.SkipWhile("Income &gt; 50");
        /// var result2 = queryable.SkipWhile("Income &gt; @0", 50);
        /// </code>
        /// </example>
        public static IQueryable SkipWhile(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNull(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return CreateQuery(_skipWhilePredicate, source, lambda);
        }

        /// <summary>
        /// Skips the while.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="SkipWhile(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable SkipWhile(this IQueryable source, string predicate, params object[] args)
        {
            return SkipWhile(source, ParsingConfig.Default, predicate, args);
        }

        #endregion SkipWhile

        #region Sum
        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Sum();
        /// var result2 = queryable.Select("Roles.Sum()");
        /// </code>
        /// </example>
        public static object Sum(this IQueryable source)
        {
            Argument.IsNotNull(nameof(source), source); 

            var sum = GetMethod(nameof(Queryable.Sum), source.ElementType);
            return Execute<object>(sum, source);
        }

        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Sum("Income");
        /// </code>
        /// </example>
        public static object Sum(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            var sumSelector = GetMethod(nameof(Queryable.Sum), lambda.ReturnType, 1);

            return Execute<object>(sumSelector, source, lambda);
        }

        /// <summary>
        /// Sums the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        /// <inheritdoc cref="Sum(IQueryable, ParsingConfig, string, object[])" />
        public static object Sum(this IQueryable source, string predicate, params object[] args)
        {
            return Sum(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <param name="lambda">A Lambda Expression.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        public static object Sum(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(lambda), lambda); 

            var sumSelector = GetMethod(nameof(Queryable.Sum), lambda.ReturnType, 1);

            return Execute<object>(sumSelector, source, lambda);
        }
        #endregion Sum

        #region Take
        /// <summary>
        /// The take
        /// </summary>
        private static readonly MethodInfo _take = GetMethod(nameof(Queryable.Take), 1);
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A <see cref="IQueryable" /> that contains the specified number of elements from the start of source.</returns>
        public static IQueryable Take(this IQueryable source, int count)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.Condition(nameof(count), count, x => x >= 0);

            return CreateQuery(_take, source, Expression.Constant(count));
        }
        #endregion Take

        #region TakeWhile

        /// <summary>
        /// The take while predicate
        /// </summary>
        private static readonly MethodInfo _takeWhilePredicate = GetMethod(nameof(Queryable.TakeWhile), 1, mi =>
        {
            return mi.GetParameters().Length == 2 &&
                   mi.GetParameters()[1].ParameterType.GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>);
        });

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable" /> that contains elements from the input sequence occurring before the element at which the test specified by predicate no longer passes.</returns>
        /// <example>
        ///   <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.TakeWhile("Income &gt; 50");
        /// var result2 = queryable.TakeWhile("Income &gt; @0", 50);
        /// </code>
        /// </example>
        public static IQueryable TakeWhile(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNull(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return CreateQuery(_takeWhilePredicate, source, lambda);
        }

        /// <summary>
        /// Takes the while.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="TakeWhile(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable TakeWhile(this IQueryable source, string predicate, params object[] args)
        {
            return TakeWhile(source, ParsingConfig.Default, predicate, args);
        }

        #endregion TakeWhile

        #region ThenBy
        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IOrderedQueryable{T}" /> whose elements are sorted according to the specified <paramref name="ordering" />.</returns>
        /// <example>
        ///   <code><![CDATA[
        /// var result = queryable.OrderBy<User>("LastName");
        /// var resultSingle = result.ThenBy<User>("NumberProperty");
        /// var resultSingleDescending = result.ThenBy<User>("NumberProperty DESC");
        /// var resultMultiple = result.ThenBy<User>("NumberProperty, StringProperty");
        /// ]]></code>
        /// </example>
        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, ParsingConfig config, string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)ThenBy((IOrderedQueryable)source, config, ordering, args);
        }

        /// <summary>
        /// Thens the by.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="ordering">The ordering.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IOrderedQueryable&lt;TSource&gt;.</returns>
        /// <inheritdoc cref="ThenBy{TSource}(IOrderedQueryable{TSource}, ParsingConfig, string, object[])" />
        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, string ordering, params object[] args)
        {
            return ThenBy(source, ParsingConfig.Default, ordering, args);
        }
        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable" /> whose elements are sorted according to the specified <paramref name="ordering" />.</returns>
        /// <example>
        ///   <code>
        /// var result = queryable.OrderBy("LastName");
        /// var resultSingle = result.OrderBy("NumberProperty");
        /// var resultSingleDescending = result.OrderBy("NumberProperty DESC");
        /// var resultMultiple = result.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable ThenBy(this IOrderedQueryable source, ParsingConfig config, string ordering, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(ordering), ordering); 

            ParameterExpression[] parameters = { Expression.Parameter(source.ElementType, string.Empty) };
            var parser = new ExpressionParser(parameters, ordering, args, config);
            var dynamicOrderings = parser.ParseOrdering(forceThenBy: true);

            var queryExpr = source.Expression;

            foreach (var dynamicOrdering in dynamicOrderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), dynamicOrdering.MethodName,
                    new[] { source.ElementType, dynamicOrdering.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)));
            }

            var optimized = OptimizeExpression(queryExpr);
            return (IOrderedQueryable)source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Thens the by.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="ordering">The ordering.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IOrderedQueryable.</returns>
        /// <inheritdoc cref="ThenBy(IOrderedQueryable, ParsingConfig, string, object[])" />
        public static IOrderedQueryable ThenBy(this IOrderedQueryable source, string ordering, params object[] args)
        {
            return ThenBy(source, ParsingConfig.Default, ordering, args);
        }

        #endregion OrderBy

        #region Where
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A <see cref="IQueryable{TSource}" /> to filter.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">An expression string to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{TSource}" /> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        ///   <code language="cs">
        /// var result1 = queryable.Where("NumberProperty = 1");
        /// var result2 = queryable.Where("NumberProperty = @0", 1);
        /// var result3 = queryable.Where("StringProperty = null");
        /// var result4 = queryable.Where("StringProperty = \"abc\"");
        /// var result5 = queryable.Where("StringProperty = @0", "abc");
        /// </code>
        /// </example>
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, ParsingConfig config, string predicate, params object[] args)
        {
            return (IQueryable<TSource>)Where((IQueryable)source, config, predicate, args);
        }

        /// <summary>
        /// Wheres the specified predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable&lt;TSource&gt;.</returns>
        /// <inheritdoc cref="Where{TSource}(IQueryable{TSource}, ParsingConfig, string, object[])" />
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string predicate, params object[] args)
        {
            return Where(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> to filter.</param>
        /// <param name="config">The <see cref="ParsingConfig" />.</param>
        /// <param name="predicate">An expression string to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable" /> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        ///   <code>
        /// var result1 = queryable.Where("NumberProperty = 1");
        /// var result2 = queryable.Where("NumberProperty = @0", 1);
        /// var result3 = queryable.Where("StringProperty = null");
        /// var result4 = queryable.Where("StringProperty = \"abc\"");
        /// var result5 = queryable.Where("StringProperty = @0", "abc");
        /// </code>
        /// </example>
        public static IQueryable Where(this IQueryable source, ParsingConfig config, string predicate, params object[] args)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(config), config); 
            Argument.IsNotNullOrEmpty(nameof(predicate), predicate); 

            var createParameterCtor = SupportsLinqToObjects(config, source);
            var lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Wheres the specified predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IQueryable.</returns>
        /// <inheritdoc cref="Where(IQueryable, ParsingConfig, string, object[])" />
        public static IQueryable Where(this IQueryable source, string predicate, params object[] args)
        {
            return Where(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable" /> to filter.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>A <see cref="IQueryable" /> that contains elements from the input sequence that satisfy the condition specified by LambdaExpression.</returns>
        public static IQueryable Where(this IQueryable source, LambdaExpression lambda)
        {
            Argument.IsNotNull(nameof(source), source); 
            Argument.IsNotNull(nameof(lambda), lambda); 

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
            return source.Provider.CreateQuery(optimized);
        }
        #endregion

        #region Private Helpers

        /// <summary>
        /// Supportses the linq to objects.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="query">The query.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool SupportsLinqToObjects(ParsingConfig config, IQueryable query)
        {
            return config.QueryableAnalyzer.SupportsLinqToObjects(query);
        }

        /// <summary>
        /// Checks the outer and inner types.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="createParameterCtor">if set to <c>true</c> [create parameter ctor].</param>
        /// <param name="outerType">Type of the outer.</param>
        /// <param name="innerType">Type of the inner.</param>
        /// <param name="outerKeySelector">The outer key selector.</param>
        /// <param name="innerKeySelector">The inner key selector.</param>
        /// <param name="outerSelectorLambda">The outer selector lambda.</param>
        /// <param name="innerSelectorLambda">The inner selector lambda.</param>
        /// <param name="args">The arguments.</param>
        /// <exception cref="ParseException">-1</exception>
        private static void CheckOuterAndInnerTypes(ParsingConfig config, bool createParameterCtor, Type outerType, Type innerType, string outerKeySelector, string innerKeySelector, ref LambdaExpression outerSelectorLambda, ref LambdaExpression innerSelectorLambda, params object[] args)
        {
            var outerSelectorReturnType = outerSelectorLambda.Body.Type;
            var innerSelectorReturnType = innerSelectorLambda.Body.Type;

            // If types are not the same, try to convert to Nullable and generate new LambdaExpression
            if (outerSelectorReturnType != innerSelectorReturnType)
            {
                if (TypeHelper.IsNullableType(outerSelectorReturnType) && !TypeHelper.IsNullableType(innerSelectorReturnType))
                {
                    innerSelectorReturnType = ExpressionParser.ToNullableType(innerSelectorReturnType);
                    innerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, innerType, innerSelectorReturnType, innerKeySelector, args);
                }
                else if (!TypeHelper.IsNullableType(outerSelectorReturnType) && TypeHelper.IsNullableType(innerSelectorReturnType))
                {
                    outerSelectorReturnType = ExpressionParser.ToNullableType(outerSelectorReturnType);
                    outerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, outerType, outerSelectorReturnType, outerKeySelector, args);
                }

                // If types are still not the same, throw an Exception
                if (outerSelectorReturnType != innerSelectorReturnType)
                {
                    throw new ParseException(string.Format(CultureInfo.CurrentCulture, Res.IncompatibleTypes, outerSelectorReturnType, innerSelectorReturnType), -1);
                }
            }
        }

        // Code below is based on https://github.com/aspnet/EntityFramework/blob/9186d0b78a3176587eeb0f557c331f635760fe92/src/Microsoft.EntityFrameworkCore/EntityFrameworkQueryableExtensions.cs
        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <returns>IQueryable.</returns>
        private static IQueryable CreateQuery(MethodInfo operatorMethodInfo, IQueryable source)
        {
            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
            }

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));
            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>IQueryable.</returns>
        private static IQueryable CreateQuery(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression)
            => CreateQuery(operatorMethodInfo, source, Expression.Quote(expression));

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>IQueryable.</returns>
        private static IQueryable CreateQuery(MethodInfo operatorMethodInfo, IQueryable source, Expression expression)
        {
            operatorMethodInfo = operatorMethodInfo.GetGenericArguments().Length == 2
                    ? operatorMethodInfo.MakeGenericMethod(source.ElementType, typeof(object))
                    : operatorMethodInfo.MakeGenericMethod(source.ElementType);

            return source.Provider.CreateQuery(Expression.Call(null, operatorMethodInfo, source.Expression, expression));
        }

        /// <summary>
        /// Executes the specified operator method information.
        /// </summary>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <returns>System.Object.</returns>
        private static object Execute(MethodInfo operatorMethodInfo, IQueryable source)
        {
            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
            }

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));
            return source.Provider.Execute(optimized);
        }

        /// <summary>
        /// Executes the specified operator method information.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <returns>TResult.</returns>
        private static TResult Execute<TResult>(MethodInfo operatorMethodInfo, IQueryable source)
        {
            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
            }

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));
            var result = source.Provider.Execute(optimized);

            return (TResult)Convert.ChangeType(result, typeof(TResult));
        }

        /// <summary>
        /// Executes the specified operator method information.
        /// </summary>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Object.</returns>
        private static object Execute(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression)
            => Execute(operatorMethodInfo, source, Expression.Quote(expression));

        /// <summary>
        /// Executes the specified operator method information.
        /// </summary>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>System.Object.</returns>
        private static object Execute(MethodInfo operatorMethodInfo, IQueryable source, Expression expression)
        {
            operatorMethodInfo = operatorMethodInfo.GetGenericArguments().Length == 2
                    ? operatorMethodInfo.MakeGenericMethod(source.ElementType, typeof(object))
                    : operatorMethodInfo.MakeGenericMethod(source.ElementType);

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression, expression));
            return source.Provider.Execute(optimized);
        }

        /// <summary>
        /// Executes the specified operator method information.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>TResult.</returns>
        private static TResult Execute<TResult>(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression)
            => Execute<TResult>(operatorMethodInfo, source, Expression.Quote(expression));

        /// <summary>
        /// Executes the specified operator method information.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="operatorMethodInfo">The operator method information.</param>
        /// <param name="source">The source.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>TResult.</returns>
        private static TResult Execute<TResult>(MethodInfo operatorMethodInfo, IQueryable source, Expression expression)
        {
            operatorMethodInfo = operatorMethodInfo.GetGenericArguments().Length == 2
                    ? operatorMethodInfo.MakeGenericMethod(source.ElementType, typeof(TResult))
                    : operatorMethodInfo.MakeGenericMethod(source.ElementType);

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression, expression));
            var result = source.Provider.Execute(optimized);

            return (TResult)Convert.ChangeType(result, typeof(TResult));
        }

        /// <summary>
        /// Gets the generic method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>MethodInfo.</returns>
        private static MethodInfo GetGenericMethod(string name)
        {
            return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(mi => mi.IsGenericMethod);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="argumentType">Type of the argument.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="parameterCount">The parameter count.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>MethodInfo.</returns>
        private static MethodInfo GetMethod(string name, Type argumentType, Type returnType, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, returnType, parameterCount, mi => mi.ToString().Contains(argumentType.ToString()) && ((predicate == null) || predicate(mi)));

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="parameterCount">The parameter count.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>MethodInfo.</returns>
        private static MethodInfo GetMethod(string name, Type returnType, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, parameterCount, mi => (mi.ReturnType == returnType) && ((predicate == null) || predicate(mi)));

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameterCount">The parameter count.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>MethodInfo.</returns>
        /// <exception cref="Exception">Specific method not found: " + name</exception>
        private static MethodInfo GetMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null)
        {
            try
            {
                return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(mi =>
                    mi.GetParameters().Length == parameterCount + 1 && (predicate == null || predicate(mi)));
            }
            catch (Exception ex)
            {
                throw new Exception("Specific method not found: " + name, ex);
            }
        }
        #endregion Private Helpers
    }
}
