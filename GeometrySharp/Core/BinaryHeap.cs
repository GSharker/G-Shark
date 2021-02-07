using System;
using System.Collections.Generic;

// ToDo this class has to be tested.
// ToDo this class has to be commented.
// ToDo this class has to be reviewed
namespace GeometrySharp.Core
{
    /// <summary>
    /// A min-type priority queue of Nodes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryHeap<T> where T : IComparable<T>
    {
        private IComparer<T> _cmp;
        private List<T> elements = new List<T>();
        public BinaryHeap()
        {
            _cmp = Comparer<T>.Default;
        }

        public BinaryHeap(IComparer<T> cmp)
        {
            _cmp = cmp;
        }

        public int Parent(int index)
        {
            if (index <= 0)
                return -1;
            return (index - 1) / 2;
        }

        public int Left(int index) => 2 * index + 1;
        public int Right(int index) => 2 * index + 2;

        public T Pop()
        {
            if (elements.Count > 0)
            {
                T item = elements[0];
                elements[0] = elements[elements.Count - 1];
                elements.RemoveAt(elements.Count - 1);
                HeapifyDown(0);
                return item;
            }
            throw new InvalidOperationException("No elements in the heap");
        }

        private void HeapifyDown(int index)
        {
            var smallest = index;
            var left = Left(index);
            var right = Right(index);
            if (left < elements.Count && _cmp.Compare(elements[left], elements[index]) < 0)
                smallest = left;
            if (right < elements.Count && _cmp.Compare(elements[right], elements[smallest]) < 0)
                smallest = right;
            if (smallest != index)
            {
                Swap(index, smallest);
                HeapifyDown(smallest);
            }
        }

        public void Add(T item)
        {
            elements.Add(item);
            HeapifyUp(elements.Count - 1);
        }

        private void HeapifyUp(int index)
        {
            var parent = Parent(index);
            if(parent >= 0 && _cmp.Compare(elements[index], elements[parent]) < 0)
            {
                Swap(index, parent);
                HeapifyUp(parent);
            }
        }

        private void Swap(int i, int j)
        {
            var temp = elements[i];
            elements[i] = elements[j];
            elements[j] = temp;
        }
    }
}
