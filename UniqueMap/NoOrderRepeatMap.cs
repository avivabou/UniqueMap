using System;
using System.Collections;
using System.Collections.Generic;

namespace UniqueMap
{
    public abstract class NoOrderRepeatMap<T> : AbstractUniqueMap<T>
        where T : IComparable
    {
        /// <summary>
        /// Unique map of groups with repetitions and no importance to order.
        /// </summary>
        /// <param name="domain">Domain items.</param>
        public NoOrderRepeatMap(ICollection<T> domain) : base(domain) { }

        /// <summary>
        /// Calculate the unique value of the given collection regardless order.
        /// </summary>
        /// <param name="group">Collection of elements from domain regardless order.</param>
        /// <returns>BitArray that present the unique value.</returns>
        public sealed override BitArray GetUniqueID(T[] Group)
        {
            Dictionary<T, uint> dict = new Dictionary<T, uint>();
            foreach (T item in Group)
                dict[item]++;

            return GetUniqueID(dict);
        }

        /// <summary>
        /// Calculate the array of objects from domain that the given BitArray present.
        /// </summary>
        /// <param name="id">The identifier of the array of objects from domain.</param>
        /// <returns>The array of objects from domain that the given BitArray present.</returns>
        public sealed override T[] GetUniqueMap(BitArray id)
        {
            Dictionary<T, uint> result = GetUniqueCountsDictionary(id);
            List<T> group = new List<T>();
            foreach (T key in result.Keys)
                for (int i = 0; i < result[key]; i++)
                    group.Add(key);

            return group.ToArray();
        }

        /// <summary>
        /// Calculate the unique value of the given counts of domain's items.
        /// </summary>
        /// <param name="group">Dictionary of domain's items amounts.</param>
        /// <returns>BitArray that present the unique value.</returns>
        public abstract BitArray GetUniqueID(Dictionary<T, uint> group);

        public abstract Dictionary<T, uint> GetUniqueCountsDictionary(BitArray id);
    }

    public class NoOrderBoundedRepeatsMap<T> : NoOrderRepeatMap<T>
        where T : IComparable
    {
        private uint _maxRepeats;

        /// <summary>
        /// Unique map of groups with limited repetitions and no importance to order.
        /// </summary>
        /// <param name="domain">Domain items.</param>
        public NoOrderBoundedRepeatsMap(ICollection<T> domain, uint maxRepeats)
            : base(domain)
        {
            _maxRepeats = maxRepeats;
        }

        /// <summary>
        /// Calculate the unique value of the given counts of domain's items.
        /// </summary>
        /// <param name="group">Dictionary of domain's items amounts.</param>
        /// <returns>BitArray that present the unique value.</returns>
        public override BitArray GetUniqueID(Dictionary<T, uint> group)
        {
            DigitNode root = null;
            for (int i = 0; i < _orderedDomain.Length; i++)
            {
                T item = _orderedDomain[i];
                DigitNode temp = new DigitNode(Math.Min(_maxRepeats, group[item]), _maxRepeats + 1);
                temp.Prev = root;
                root = temp;
            }

            return root.ToBinaryBitArray();
        }

        /// <summary>
        /// Calculate the amount of domain's items that the given BitArray present.
        /// </summary>
        /// <param name="id">The identifier of the array of objects from domain.</param>
        /// <returns>The dictionary of domain's items amounts.</returns>
        public override Dictionary<T, uint> GetUniqueCountsDictionary(BitArray id)
        {
            DigitNode convertedId = DigitNode.FromBinary(id, _maxRepeats + 1);
            while (convertedId.Prev != null)
                convertedId = convertedId.Prev;

            Dictionary<T, uint> result = new Dictionary<T, uint>();
            int i = 0;
            while (convertedId != null)
            {
                result[_orderedDomain[i]] = convertedId.Value;
                convertedId = convertedId.Next;
                i++;
            }

            return result;
        }
    }

    public class NoOrderNotBoundedRepeatsMap<T> : NoOrderRepeatMap<T>
        where T : IComparable
    {
        /// <summary>
        /// Unique map of groups with limitless repetitions and no importance to order.
        /// </summary>
        /// <param name="domain">Domain items.</param>
        public NoOrderNotBoundedRepeatsMap(ICollection<T> domain)
            : base(domain) { }

        /// <summary>
        /// Calculate the unique value of the given counts of domain's items.
        /// </summary>
        /// <param name="group">Dictionary of domain's items amounts.</param>
        /// <returns>BitArray that present the unique value.</returns>
        public override BitArray GetUniqueID(Dictionary<T, uint> group)
        {
            DigitNode root = null;
            uint newBase = 2;
            foreach (var item in group.Keys)
                if (group[item] > newBase)
                    newBase = group[item];
            newBase++;
            newBase = (uint)(1 << (int)Math.Ceiling(Math.Log2(newBase)));

            for (int i = 0; i < _orderedDomain.Length; i++)
            {
                T item = _orderedDomain[i];
                DigitNode temp = new DigitNode(group[item], newBase);
                temp.Prev = root;
                root = temp;
            }

            DigitNode last = new DigitNode(1, 2);
            last.Prev = root;
            return last.ToBinaryBitArray();
        }

        /// <summary>
        /// Calculate the amount of domain's items that the given BitArray present.
        /// </summary>
        /// <param name="id">The identifier of the array of objects from domain.</param>
        /// <returns>The dictionary of domain's items amounts.</returns>
        public override Dictionary<T, uint> GetUniqueCountsDictionary(BitArray id)
        {
            uint digBase = (uint)(1 << ((id.Length - 1) / _orderedDomain.Length));
            DigitNode convertedId = DigitNode.FromBinary(id, digBase);
            while (convertedId.Prev != null)
                convertedId = convertedId.Prev;

            Dictionary<T, uint> result = new Dictionary<T, uint>();
            int i = 0;
            while (convertedId.Next != null)
            {
                result[_orderedDomain[i]] = convertedId.Value;
                convertedId = convertedId.Next;
                i++;
            }

            return result;
        }
    }
}