using System;
using System.Collections;
using System.Collections.Generic;
using LibSvmDotNet.Interop;

namespace LibSvmDotNet
{
    /// <summary>
    /// Represents an vector data.
    /// </summary>
    public sealed class NodeArray : IEnumerable<Node>
    {

        #region Fields

        private readonly unsafe NativeMethods.svm_node* _UnsafePtr;

        #endregion

        #region Constructors

        internal unsafe NodeArray(NativeMethods.svm_node* ptr, int length)
        {
            this.NativePtr = (IntPtr)ptr;
            this._UnsafePtr = ptr;
            this.Length = length;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total number of elements in all the dimensions of the <see cref="NodeArray"/>.
        /// </summary>
        /// <returns>The total number of elements in all the dimensions of the <see cref="NodeArray"/>; zero if there are no elements in the array.</returns>
        public int Length { get; }

        internal IntPtr NativePtr
        {
            get;
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public Node this[int index]
        {
            get
            {
                unsafe
                {
                    var ptr = this._UnsafePtr;
                    return new Node(&ptr[index]);
                }
            }
        }

        #endregion

        #region Implementat IEnumerable<Node>

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{Node}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<Node> GetEnumerator()
        {
            for (var index = 0; index < this.Length; index++)
                yield return new Node(this.NativePtr, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}