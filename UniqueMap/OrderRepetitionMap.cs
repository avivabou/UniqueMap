using System;
using System.Collections;
using System.Collections.Generic;

namespace UniqueMap
{

    class OrderRepetitionMap<T> : AbstractUniqueMap<T> 
        where T : IComparable
    {
        private Dictionary<T, uint> _domainDigits;
        private uint _digitsBase;

        /// <summary>
        /// Unique map of groups with repetitions and importance to order.
        /// </summary>
        /// <param name="domain">Domain items.</param>
        public OrderRepetitionMap(ICollection<T> domain)
            : base(domain)
        {
            _domainDigits = new Dictionary<T, uint>();
            _digitsBase = (uint)_orderedDomain.Length;
            for (uint i = 0; i < _digitsBase; i++)
                _domainDigits[_orderedDomain[i]] = i;

            MinValue = new DigitNode(_digitsBase - 2, _digitsBase).ToBinaryBitArray();
        }

        /// <summary>
        /// Calculate the unique value of the given ordered collection.
        /// </summary>
        /// <param name="orderedGroup">Ordered collection of elements from domain.</param>
        /// <returns>BitArray that present the unique value.</returns>
        public override BitArray GetUniqueID(T[] orderedGroup)
        {
            if (orderedGroup.Length == 0)
                return MinValue;

            DigitNode step = null;
            for (int i = 0; i < orderedGroup.Length; i++)
            {
                if (!_domainDigits.ContainsKey(orderedGroup[i]))
                    throw new ArgumentException(string.Format("The {0} item in the given group isn't part of the domain.", i));

                DigitNode current = new DigitNode(_domainDigits[orderedGroup[i]], _digitsBase);
                if (i == orderedGroup.Length - 1)
                    current += (_digitsBase - 1);

                if (current.Prev != null)
                    current.Prev.Prev = step;
                else
                    current.Prev = step;

                step = current;
            }

            return step.ToBinaryBitArray();
        }

        /// <summary>
        /// Calculate the ordered array of objects from domain that the given BitArray present.
        /// </summary>
        /// <param name="id">The identifier of the ordered array of objects from domain.</param>
        /// <returns>The ordered array of objects from domain that the given BitArray present.</returns>
        public override T[] GetUniqueMap(BitArray id)
        {
            DigitNode convertedId = DigitNode.FromBinary(id, _digitsBase);
            Console.WriteLine(convertedId);
            Stack<T> value = new Stack<T>();
            ReadFirstDigit(ref convertedId, ref value);
            while (convertedId.Prev != null)
            {
                convertedId = convertedId.Prev;
                value.Push(_orderedDomain[convertedId.Value]);
            }

            return value.ToArray();
        }

        /// <summary>
        /// Reduce the last digit added and calculate the rest digits as domain objects.
        /// </summary>
        /// <param name="convertedId">The translated identifer.</param>
        /// <param name="value">Stack of objects which will build the resulted map.</param>
        private void ReadFirstDigit(ref DigitNode convertedId, ref Stack<T> value)
        {
            DigitNode temp = new DigitNode(convertedId.Value, _digitsBase);
            if (temp.Value == 1)
            {
                if (convertedId.Prev == null)
                    return;

                convertedId = convertedId.Prev;
                temp.Prev = new DigitNode(convertedId.Value, _digitsBase);
            }

            temp -= (_digitsBase - 1);
            if (temp == null)
                value.Push(_orderedDomain[0]);
            else
                while (temp != null)
                {
                    value.Push(_orderedDomain[temp.Value]);
                    temp = temp.Prev;
                }
        }
    }
}
