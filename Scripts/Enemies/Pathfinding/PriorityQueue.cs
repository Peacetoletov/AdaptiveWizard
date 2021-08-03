using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// http://www.java2s.com/Code/CSharp/Collections-Data-Structure/PriorityQueue2.htm

namespace Pathfinding {
    
    /// <summary>
    /// Represents a strongly typed priority queue of objects. 
    /// Objects are ordered in the priority queue according to the provided ordering comparator 
    /// (from smallest to largest) and not by the order of the insertion as in the regular queue.
    /// Provides methods to add(enqueue) and remove(dequeue) object from ordered queue. 
    /// </summary>
    /// <remarks> PriorityQueue(Of T) is implemented on the basis of List(Of T) and intentionally does not 
    /// hide base class functions that can possibly invalidate the queue ordering. After operations 
    /// that manipulates with list objects of the list itself (adding/removing) You have to restore queue 
    /// ordering by using AdjustHeap methods.</remarks>
    public class PriorityQueue<T> : List<T> 
    {
        public bool IsEmpty() {
            return Count == 0;
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty, 
        /// has the default initial capacity, default comparator for the items.
        /// </summary>
        public PriorityQueue()
        {
            comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty, 
        /// has the default initial capacity, and uses the specified IComparer. 
        /// </summary>
        /// <param name="comparer">The IComparer implementation to use when comparing items.</param>
        public PriorityQueue(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }


        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty, 
        /// has the specified initial capacity and default comparator for the items.        
        /// </summary>
        /// <param name="capacity">The initial number of elements that the queue can contain.</param>
        public PriorityQueue(int capacity)
            : base(capacity)
        {
            comparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty, 
        /// has the specified initial capacity and comparator for the items.        
        /// </summary>
        /// <param name="capacity">The initial number of elements that the queue can contain.</param>
        /// <param name="comparer">The IComparer implementation to use when comparing items.</param>
        public PriorityQueue(int capacity, IComparer<T> comparer)
            : base(capacity)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Returns the the smallest (first) object of the Queue without removing it. 
        /// </summary>
        /// <returns>The object at the beginning of the Queue.</returns>
        public T Peek()
        {
            return this[0];
        }

        /// <summary>
        /// Adds an object to the Priority Queue. 
        /// </summary>
        /// <param name="item">The object to add to the Queue.</param>
        public void Enqueue(T item)
        {
            Add(item);
            AdjustHeap(Count - 1);
        }

        /// <summary>
        /// Adds the elements of an ICollection to queue. 
        /// </summary>
        /// <param name="collection">The ICollection whose elements should be added to the queue</param>
        public void EnqueueRange(IEnumerable<T> collection)
        {
            int pcount = Count;
            AddRange(collection);
            AdjustHeap(pcount, Count - pcount);
        }

        /// <summary>
        /// Removes and returns the smallest object of the Queue. 
        /// </summary>
        /// <returns>The smallest object that is removed from the beginning of the Queue. </returns>
        public T Dequeue()
        {
            int last = Count - 1;
            swap(0, last);
            T res = this[last];

            this.RemoveAt(last);
            AdjustHeap(0);

            return res;
        }

        /// <summary>
        /// Rebuild heap after change of one heap element
        /// </summary>
        /// <param name="position">Position of changed item</param>
        public void AdjustHeap(int position)
        {
            int up = position;
            while (up > 0)
            {
            int parent = (up - 1) / 2;

            if (comparer.Compare(this[parent], this[up]) <= 0)
                break;

            swap(parent, up);
            up = parent;
            }
            if (up != position)
            return;

            int down = position;
            while (down * 2 + 1 < Count)
            {
            int child = down * 2 + 1;
            if (child + 1 < Count &&
                comparer.Compare(this[child + 1], this[child]) <= 0)
                child++;

            if (comparer.Compare(this[down], this[child]) <= 0)
                break;

            swap(child, down);
            down = child;
            }
        }

        /// <summary>
        /// Rebuild heap structure of the list after change of element range
        /// </summary>
        /// <remarks>Can be used to restore heap structure of the Priority Queue after changes were 
        /// made to part of the underlying array</remarks>
        public void AdjustHeap(int from, int count)
        {
            for (int i = 0; i < count; ++i)
            AdjustHeap(from + i);
        }

        /// <summary>
        /// Rebuild heap structure of the changed list
        /// </summary>
        /// <remarks>Can be used to restore heap structure of the Priority Queue after changes were 
        /// made to underlying array</remarks>
        public void AdjustHeap()
        {
            AdjustHeap(0, (Count + 1) / 2);
        }


        private void swap(int i, int j)
        {
            T t = this[i];
            this[i] = this[j];
            this[j] = t;
        }

        private readonly IComparer<T> comparer;

        /// <summary>
        /// Gets the IComparer for the queue. 
        /// </summary>
        public IComparer<T> Comparer
        {
            get { return comparer; }
        }
    }
}
