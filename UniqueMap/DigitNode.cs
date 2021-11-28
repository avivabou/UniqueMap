using System.Collections;
using System.Collections.Generic;

namespace UniqueMap
{
    internal class DigitNode
    {
        private bool _isFirst, _reducableBases;
        private DigitNode _prev = null;
        private DigitNode _next = null;

        internal uint Value { get; set; }
        internal uint Base { get; set; }
        internal uint Rest { get; private set; } = 0;
        internal DigitNode Prev
        {
            get
            {
                return _prev;
            }
            set
            {
                if (value == null)
                    return;
                _prev = value;
                _prev._isFirst = false;
                _prev._next = this;
            }
        }
        internal DigitNode Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = value;
                _isFirst = false;
                if (_next != null)
                    _next._prev = this;
            }
        }

        /// <summary>
        /// DigitNode constructor.
        /// </summary>
        /// <param name="digitValue">Digit decimal value.</param>
        /// <param name="numberBase">Base of digit.</param>
        /// <param name="reduceNextBase">Is next digits' bases should linear decrease.</param>
        internal DigitNode(uint digitValue, uint numberBase, bool reduceNextBase = false)
        {
            Base = numberBase;
            Value = digitValue;
            _isFirst = true;
            _reducableBases = reduceNextBase;
        }

        /// <summary>
        /// Check if the current DigitNode and it tail value is 0.
        /// </summary>
        /// <returns>True if the current DigitNode and it tail value is 0, otherwise False.</returns>
        internal bool IsZero()
        {
            return (Value == 0) && ((Prev == null) || Prev.IsZero());
        }

        /// <summary>
        /// Create a clone of the current DigitNode and it tail.
        /// </summary>
        /// <returns>A clone of the current DigitNode and it tail.</returns>
        internal DigitNode Clone()
        {
            DigitNode newdig = new DigitNode(Value, Base);
            if (Prev != null)
            {
                newdig.Prev = Prev.Clone();
                newdig.Prev._isFirst = false;
            }
            return newdig;
        }

        /// <summary>
        /// Calculate the current DigitNode and it tail binary value.
        /// </summary>
        /// <returns>BitArray that present the current DigitNode and it tail binary value.</returns>
        internal BitArray ToBinaryBitArray()
        {
            Stack<bool> bits = new Stack<bool>();
            DigitNode dig = Clone();
            while (!dig.IsZero())
            {
                dig /= 2;
                bits.Push(dig.Rest == 1 ? true : false);
            }
            return new BitArray(bits.ToArray());
        }

        /// <summary>
        /// Present the current DigitNode, it tail and the rest value.
        /// </summary>
        /// <returns>String that present the current DigitNode, it tail and the rest value.</returns>
        public override string ToString()
        {
            if (Prev != null)
                return string.Format("[{0}]{1}", Value, Prev);
            return string.Format("[{0}] ({1})", Value, Rest);
        }

        /// <summary>
        /// Build a DigitNodes chain that present the given binary value of the BitArray.
        /// </summary>
        /// <param name="bits">The binary value that should be converted.</param>
        /// <param name="toBase">The base of the resulted DigitNodes chain.</param>
        /// <param name="reduceNextBases">Mark true if next bases should be linear decreased.</param>
        /// <returns>The calculated DigitNodes chain that present the given binary value.</returns>
        internal static DigitNode FromBinary(BitArray bits, uint toBase, bool reduceNextBases = false)
        {
            if (bits.Count == 0)
                return new DigitNode(0, toBase, reduceNextBases);

            DigitNode root = new DigitNode((uint)(bits[0] ? 1 : 0), toBase, reduceNextBases);
            for (int i = 1; i < bits.Length; i++)
            {
                root *= 2;
                root += (uint)(bits[i] ? 1 : 0);
            }

            return root;
        }

        public static DigitNode operator +(DigitNode digit, uint additional)
        {
            while (digit.Prev != null)
                digit = digit.Prev;

            DigitNode root = null;
            while (digit != null)
            {
                root = digit;
                digit.Value += additional;
                additional = digit.Value / digit.Base;
                digit.Value %= digit.Base;
                digit = digit.Next;
            }

            if (additional > 0)
            {
                uint nextBase = root._reducableBases ? root.Base - 1 : root.Base;
                DigitNode temp = new DigitNode(additional, nextBase, root._reducableBases);
                temp.Prev=root;
                root = temp;
            }
            return root;
        }

        public static DigitNode operator -(DigitNode digit, uint sub)
        {
            DigitNode root = digit;
            while (digit.Prev != null)
                digit = digit.Prev;

            if (sub <= digit.Value)
                digit.Value -= sub;
            else
            {
                digit.Value += digit.Base - sub;
                digit.Next--;
            }

            if (root.Value == 0)
                root = root.Prev;

            return root;
        }

        public static DigitNode operator --(DigitNode digit)
        {
            if (digit == null)
                return null;
            DigitNode root = digit;
            if (digit.Value > 0)
                digit.Value--;
            else if (digit.Next != null)
                root = digit.Next--;
            return root;
        }

        public static DigitNode operator *(DigitNode digit, uint multipiler)
        {
            while (digit.Prev != null)
                digit = digit.Prev;

            DigitNode root = null;
            uint accumulator = 0;
            while (digit != null)
            {
                root = digit;
                digit.Value *= multipiler;
                digit.Value += accumulator;
                accumulator = digit.Value / digit.Base;
                digit.Value %= digit.Base;
                digit = digit.Next;
            }

            if (accumulator > 0)
            {
                uint nextBase = root._reducableBases ? root.Base - 1 : root.Base;
                DigitNode temp = new DigitNode(accumulator, nextBase, root._reducableBases);
                temp.Prev = root;
                root = temp;
            }

            return root;
        }

        public static DigitNode operator /(DigitNode digit, uint divisor)
        {
            uint currentVal = digit.Value;
            uint rest = currentVal % divisor;
            digit.Value = currentVal / divisor;
            if (digit.Prev == null)
            {
                digit.Rest = rest;
                return digit;
            }

            digit.Prev.Value += rest * digit.Prev.Base;
            digit.Prev /= divisor;
            digit.Rest = digit.Prev.Rest;
            if (digit.Value == 0 && digit._isFirst)
                return digit.Prev;
            return digit;
        }
    }
}