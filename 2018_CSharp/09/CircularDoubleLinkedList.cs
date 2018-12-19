using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace _09
{
    public sealed class CircularDoubleLinkedList<T>: IEnumerable<T>
        where T: IComparable<T>
    {
        Node<T> current = null;
        int count = 0;
        T beginValue;

        public CircularDoubleLinkedList()
        {
        }

        public CircularDoubleLinkedList(T item)
        {
            this.AddFirstItem(item);
        }

        void AddFirstItem(T item)
        {
            current = new Node<T>(item);
            Current.Next = Current;
            Current.Prev = Current;
            beginValue = item;
            ++count;
        }

        public Node<T> Current { get { return current; } }
        public T CurrentValue { get { return current.Value; } }

        public int Count { get { return count; } }

        public CircularDoubleLinkedList<T> MoveRight(int cnt = 1)
        {
            for (var i = 0; i < cnt; i++)
            {
                current = current.Next;
            }
            return this;
        }

        public CircularDoubleLinkedList<T> MoveLeft(int cnt = 1)
        {
            for (var i = 0; i < cnt; i++)
            {
                current = current.Prev;
            }
            return this;
        }

        public CircularDoubleLinkedList<T> Insert(T item)
        {
            if (current == null)
                AddFirstItem(item);
            else
            {
                Node<T> newNode = new Node<T>(item);
                newNode.Next = Current.Next;
                Current.Next.Prev = newNode;
                newNode.Prev = Current;
                Current.Next = newNode;
                current = newNode;
                ++count;
            }
            return this;
        }

        public CircularDoubleLinkedList<T> Remove()
        {
            var nextNode = Current.Next;
            Current.Prev.Next = nextNode;
            nextNode.Prev = Current.Prev;
            Current.Next = null;
            Current.Prev = null;
            current = nextNode;
            --count;
            return this;
        }

        public CircularDoubleLinkedList<T> Clear()
        {
            current = null;
            count = 0;
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (current != null)
            {
                var cur = current;
                while (!cur.Value.Equals(beginValue))
                {
                    cur = cur.Prev;
                }
                do
                {
                    yield return cur.Value;
                    cur = cur.Next;
                } while (!cur.Value.Equals(beginValue));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}