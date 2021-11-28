using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace UniqueMap
{
    public class NoOrderNoRepeatMap<T> : AbstractUniqueMap<T> 
        where T: IComparable
    {
        /// <summary>
        /// Unique map of groups with no repetitions and no importance to order.
        /// </summary>
        /// <param name="domain">Domain items.</param>
        public NoOrderNoRepeatMap(ICollection<T> domain) : base(domain) { }

        /// <summary>
        /// Calculate the unique value of the given collection regardless order.
        /// </summary>
        /// <param name="group">Collection of elements from domain regardless order.</param>
        /// <returns>BitArray that present the unique value.</returns>
        public override BitArray GetUniqueID(T[] group)
        {
            BitArray bits = new BitArray(_orderedDomain.Length);
            for (int i = 0; i < _orderedDomain.Length; i++)
                if (group.Contains(_orderedDomain[i]))
                    bits[i] = true;
                else
                    bits[i] = false;
            
            return bits;
        }

        /// <summary>
        /// Calculate the array of objects from domain that the given BitArray present.
        /// </summary>
        /// <param name="id">The identifier of the array of objects from domain.</param>
        /// <returns>The array of objects from domain that the given BitArray present.</returns>
        public override T[] GetUniqueMap(BitArray id)
        {
            if (id.Length > _orderedDomain.Length)
                throw new Exception("The inserted ID presenting a value out of domain range.");
            List<T> accumulator = new List<T>();
            for (int i = 0; i < id.Length; i++)
                if (id[i])
                    accumulator.Add(_orderedDomain[i]);

            return accumulator.ToArray();
        }
    }
}
