using System;
using System.Collections.Generic;
using System.Text;

namespace _09
{
    public class Node<T>
    {
        public Node(T data)
        {
            Value = data;
        }
        public T Value { get; set; }
        public Node<T> Next { get; set; }
        public Node<T> Prev { get; set; }
    }
}
