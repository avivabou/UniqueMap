using System;
using System.Collections;
using System.Collections.Generic;

namespace NumbersWithBases
{
    public class DigitNode
    {
        private bool _isFirst, _reducableBases;

        public uint Value { get; set; }
        public uint Base { get; set; }

        public DigitNode Prev { get; private set; } = null;
        public DigitNode Next { get; private set; } = null;
        public uint Rest { get; private set; } = 0;

        public DigitNode(uint digitValue, uint numberBase, bool reduceNextBase = false)
        {
            Base = numberBase;
            Value = digitValue;
            _isFirst = true;
            _reducableBases = reduceNextBase;
        }

        public override string ToString()
        {
            if (Prev != null)
                return string.Format("[{0}]{1}", Value, Prev);
            return string.Format("[{0}] ({1})", Value, Rest);
        }

        public bool IsZero()
        {
            return (Value == 0) && ((Prev == null) || Prev.IsZero());
        }

        public void AppendPrev(DigitNode digit)
        {
            if (digit== null)
                return;
            Prev = digit;
            Prev._isFirst = false;
            Prev.Next = this;
        }

        public void AppendNext(DigitNode digit)
        {
            Next = digit;
            _isFirst = false;
            if (Next != null)
                Next.Prev = this;
        }

        public DigitNode Clone()
        {
            DigitNode newdig = new DigitNode(Value, Base);
            if (Prev != null)
            {
                newdig.Prev = Prev.Clone();
                newdig.Prev._isFirst = false;
            }
            return newdig;
        }

        public BitArray ToBinaryBitArray()
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

        public DigitNode ToBase(uint toBase)
        {
            DigitNode root = null;
            DigitNode dig = Clone();
            while (!dig.IsZero())
            {
                dig /= toBase;
                DigitNode temp = new DigitNode(dig.Rest, toBase);
                temp.AppendPrev(root);
                root = temp;
            }

            return root;
        }

        public void LinearReduceNextBases()
        {
            DigitNode dn = this;
            while (dn.Prev != null)
                dn = dn.Prev;

            uint baseReduce = 0;
            while (dn.Next != null)
            {
                baseReduce++;
                dn = dn.Next;
                dn.Base = (uint)Math.Max(2, dn.Base - baseReduce);
                if (dn.Value >= dn.Base)
                {
                    if (dn.Next != null)
                        dn.Next.Value += dn.Value / dn.Base;
                    else
                        dn.AppendNext(new DigitNode(dn.Value / dn.Base, dn.Base + baseReduce));

                    dn.Value %= dn.Base;
                }
            }
        }

        public static DigitNode FromBinary(BitArray bits, uint toBase, bool reduceNextBases = false)
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
                temp.AppendPrev(root);
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
                temp.AppendPrev(root);
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