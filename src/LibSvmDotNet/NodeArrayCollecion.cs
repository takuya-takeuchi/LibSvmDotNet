using LibSvmDotNet.Interop;

namespace LibSvmDotNet
{

    /// <summary>
    /// Represents an collection of <see cref="NodeArray"/>.
    /// </summary>
    public sealed class NodeArrayCollecion
    {

        #region Fields

        private readonly int[] _LengthArray;

        private readonly unsafe NativeMethods.svm_node** _NativePtr;

        #endregion

        #region Constructors

        internal unsafe NodeArrayCollecion(NativeMethods.svm_node** ptr, int[] lengthArray)
        {
            this._NativePtr = ptr;
            this._LengthArray = lengthArray;
            this.Count = lengthArray.Length;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="NodeArrayCollecion"/>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="NodeArrayCollecion"/>.</returns>
        public int Count
        {
            get;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The index of the element to get or set.</returns>
        public NodeArray this[int index]
        {
            get
            {
                unsafe
                {
                    return new NodeArray(this._NativePtr[index], this._LengthArray[index]);
                }
            }
        }

        #endregion

    }

}