﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CoCore.Base
{
    /// <summary>
    /// Wraps a generic Comparison<T> delegate in an IComparer to make it easy
    /// to use a lambda expression for methods that take an IComparer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComparisonComparer<T> : IComparer<T>, IComparer
    {
        private readonly Comparison<T> _comparison;

        public ComparisonComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }

        public int Compare(object o1, object o2)
        {
            return _comparison((T)o1, (T)o2);
        }
    }
}
