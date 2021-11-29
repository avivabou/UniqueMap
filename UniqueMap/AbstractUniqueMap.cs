using System;
using System.Collections;
using System.Collections.Generic;

namespace UniqueMap
{
    public abstract class AbstractUniqueMap<T> 
        where T: IComparable
    {
        protected T[] _orderedDomain;

        public BitArray MinValue { get; protected init; } = new BitArray(1,false);              // = 0
        public BitArray MaxValue { get; protected init; } = null;                               // = Infinity

        /// <summary>
        /// Base constructor for any uniques map.
        /// Verify the domain do not contain equal values.
        /// </summary>
        /// <param name="domain">Collection of domain objects.</param>
        protected AbstractUniqueMap(ICollection<T> domain)
        {
            List<T> tempDomain = new List<T>();
            foreach (T item in domain)
            {
                if (tempDomain.Contains(item))
                    throw new ArgumentException("The domain contain two or more equivalent objects.");

                tempDomain.Add(item);
            }

            tempDomain.Sort();
            _orderedDomain = tempDomain.ToArray();
        }

        /// <summary>
        /// Calculate the unique value of the given collection.
        /// </summary>
        /// <param name="group">Collection of elemnts from domain.</param>
        /// <returns>BitArray the present uniquly the given collection.</returns>
        public abstract BitArray GetUniqueID(T[] group);

        /// <summary>
        /// Calculate the array of objects from domain that the given BitArray present.
        /// </summary>
        /// <param name="id">The identifier of the array of objects from domain.</param>
        /// <returns>The array of objects from domain that the given BitArray present.</returns>
        public abstract T[] GetUniqueMap(BitArray id);
    }
}
