using System;
using LibSvmDotNet.Interop;

namespace LibSvmDotNet
{

    /// <summary>
    /// Represents an element for vector data.
    /// </summary>
    public struct Node
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> struct with the specified node data and index of node data.
        /// </summary>
        /// <param name="ptr">Pointer to an array of <see cref="NativeMethods.svm_node"/>.</param>
        /// <param name="index">The zero-based index at which node is located.</param>
        internal unsafe Node(IntPtr ptr, int index)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException($"{nameof(ptr)} should not be IntPtr.Zero");

            var p = (NativeMethods.svm_node*)ptr;
            var node = p[index];
            this.Index = node.index;
            this.Value = node.value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> struct with the specified node data.
        /// </summary>
        /// <param name="ptr">Pointer to an array of <see cref="NativeMethods.svm_node"/>.</param>
        internal unsafe Node(NativeMethods.svm_node* ptr)
        {
            if ((IntPtr)ptr == IntPtr.Zero)
                throw new ArgumentException($"{nameof(ptr)} should not be IntPtr.Zero");

            this.Index = ptr->index;
            this.Value = ptr->value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating the position of the element in vector.
        /// </summary>
        /// <returns>The one-based index representing the position of the element in vector.</returns>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value of the element in vector.
        /// </summary>
        /// <returns>The value of the element in vector.</returns>
        public double Value
        {
            get;
            set;
        }

        #endregion

    }

}