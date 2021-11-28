using System;
using System.Collections;
using System.Collections.Generic;

namespace UniqueMap
{
    class OrderNoRepeatMap<T> :AbstractUniqueMap<T> 
        where T : IComparable
    {
        /// <summary>
        /// Unique map of groups with no repetitions and importance to order.
        /// </summary>
        /// <param name="domain">Domain items.</param>
        public OrderNoRepeatMap(ICollection<T> domain) : base(domain) { }

        /// <summary>
        /// Calculate the unique value of the given ordered collection.
        /// </summary>
        /// <param name="group">Ordered collection of elements from domain.</param>
        /// <returns>BitArray that present the unique value.</returns>
        public override BitArray GetUniqueID(T[] orderedGroup)
        {
            if (orderedGroup.Length == 0)
                return new DigitNode(0,2).ToBinaryBitArray();

            uint digitBase = (uint) _orderedDomain.Length+1;
            List<T> identifierList = new List<T>(_orderedDomain);
            DigitNode root = new DigitNode((uint) orderedGroup.Length, digitBase);
            for (int i = 0; i < orderedGroup.Length; i++)
            {
                digitBase--;
                uint currentVal = (uint) identifierList.IndexOf(orderedGroup[i]);
                root.Next=new DigitNode(currentVal, digitBase);
                root = root.Next;
                identifierList.Remove(orderedGroup[i]);
            }

            return root.ToBinaryBitArray();
        }

        /// <summary>
        /// Calculate the ordered array of objects from domain that the given BitArray present.
        /// </summary>
        /// <param name="id">The identifier of the array of objects from domain.</param>
        /// <returns>The ordered array of objects from domain that the given BitArray present.</returns>
        public override T[] GetUniqueMap(BitArray id)
        {
            DigitNode convertedId = DigitNode.FromBinary(id, (uint)_orderedDomain.Length+1,true);
            while (convertedId.Prev != null)
                convertedId = convertedId.Prev;

            List<T> result = new List<T>();
            List<T> tempDomain = new List<T>(_orderedDomain);
            uint len = convertedId.Value;
            convertedId = convertedId.Next;
            for (int i = 0; i < len; i++)
            {
                int index;
                if (convertedId != null)
                {
                    index = (int)convertedId.Value;
                    convertedId = convertedId.Next;
                }
                else
                    index = 0;

                result.Add(tempDomain[index]);
                tempDomain.RemoveAt(index);
            }

            return result.ToArray();
        }
    }
}
