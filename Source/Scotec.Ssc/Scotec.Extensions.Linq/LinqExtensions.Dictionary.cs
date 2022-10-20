#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#endregion


namespace Scotec.Extensions.Linq
{
    public static partial class LinqExtensions
    {
        public static IDictionary<string, string> Merge(this IDictionary<string, string> first, IDictionary<string, string> second, MergeBehaviour mergeBehaviour)
        {
            return mergeBehaviour switch
            {
                MergeBehaviour.IgnoreDuplicate => first.Select(item => item)
                                                       .Concat(second.Select(item => item))
                                                       .GroupBy(item => item.Key)
                                                       .Select(group => @group.First())
                                                       .ToDictionary(item => item.Key, item => item.Value)
              , MergeBehaviour.Override => first.Select(item => item)
                                                .Concat(second.Select(item => item))
                                                .GroupBy(item => item.Key)
                                                .Select(group => @group.Last())
                                                .ToDictionary(item => item.Key, item => item.Value)
              , MergeBehaviour.Throw => first.Select(item => item).Concat(second.Select(item => item)).ToDictionary(item => item.Key, item => item.Value)
              , _ => throw new Exception()
            };
        }
    }

    public enum MergeBehaviour
    {
        /// <summary>
        /// Ignores the items from the second dictionary if they have the same key as the items in the first dictionary..
        /// </summary>
        IgnoreDuplicate,

        // Overwrites items in the first dictionary with items from the second dictionary that have the same key.
        /// <summary>
        /// 
        /// </summary>
        Override,

        /// <summary>
        /// Throw exception if an item with same key exists.
        /// </summary>
        Throw
    }
}
