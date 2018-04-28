using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using LibSvmDotNet.Interop;

namespace LibSvmDotNet
{

    /// <summary>
    /// Represents an problem for Support Vector Machine.
    /// </summary>
    public sealed class Problem : DisposableObject
    {

        #region Constructors

        internal unsafe Problem(IEnumerable<Node[]> x, double[] y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            // Get array of each length of node array
            var collection = x.ToArray();
            var lengthArray = collection.Select(nodes => nodes.Length).ToArray();

            // Create svm_node**
            var count = collection.Length;
            var index = 0;
            var ptr = (NativeMethods.svm_node**)Marshal.AllocCoTaskMem(sizeof(NativeMethods.svm_node*) * count);
            foreach (var array in collection)
                ptr[index++] = array.ToNative();

            // Create svm_problem instance
            var prob = (NativeMethods.svm_problem*)Marshal.AllocCoTaskMem(sizeof(NativeMethods.svm_problem));
            prob->l = y.Length;
            prob->x = ptr;
            prob->y = (double*)Marshal.AllocCoTaskMem(sizeof(double) * prob->l);
            Marshal.Copy(y, 0, (IntPtr)prob->y, prob->l);

            this.NativePtr = (IntPtr)prob;
            this._Y = y;
            this._X = new NodeArrayCollecion(ptr, lengthArray);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of data contained in the <see cref="Problem"/>.
        /// </summary>
        /// <returns>The number of data contained in the <see cref="Problem"/>.</returns>
        public int Length
        {
            get
            {
                this.ThrowIfDisposed();
                return this._Y.Length;
            }
        }

        private readonly NodeArrayCollecion _X;

        /// <summary>
        /// Gets the <see cref="NodeArrayCollecion"/>.
        /// </summary>
        /// <returns>An <see cref="NodeArrayCollecion"/> instance that problem owns.</returns>
        /// <exception cref="ObjectDisposedException">Cannot access a disposed object.</exception>
        public NodeArrayCollecion X
        {
            get
            {
                this.ThrowIfDisposed();
                return this._X;
            }
        }

        private readonly double[] _Y;

        /// <summary>
        /// Gets an array of the labels for this problem.
        /// </summary>
        /// <returns>An array of the labels for this problem</returns>
        /// <exception cref="ObjectDisposedException">Cannot access a disposed object.</exception>
        public double[] Y
        {
            get
            {
                this.ThrowIfDisposed();
                return this._Y;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="Problem"/> from the specified file.
        /// </summary>
        /// <param name="path">The LIBSVM format file name and path.</param>
        /// <returns>This method returns a new <see cref="Problem"/> for the specified file.</returns>
        /// <exception cref="ArgumentException">The specified path is null or whitespace.</exception>
        /// <exception cref="FileNotFoundException">The specified file is not found.</exception>
        /// <exception cref="FormatException">The specified file is invalid format.</exception>
        public static Problem FromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The specified path is null or whitespace.");

            if (!File.Exists(path))
                throw new FileNotFoundException("The specified file is not found.");

            var x = new List<Node[]>();
            var y = new List<double>();
            var lines = File.ReadAllLines(path);

            try
            {
                foreach (var tokens in lines.Select(line => line.Split(" \t\n\r\f".ToCharArray())
                                            .Where(c => c != string.Empty).ToArray()))
                {
                    y.Add(double.Parse(tokens[0]));

                    var nodes = new List<Node>();
                    for (var i = 1; i <= tokens.Length - 1; i++)
                    {
                        var token = tokens[i].Trim().Split(':');
                        nodes.Add(new Node
                        {
                            Index = int.Parse(token[0]),
                            Value = double.Parse(token[1])
                        });
                    }

                    x.Add(nodes.ToArray());
                }

                return new Problem(x, y.ToArray());

            }
            catch (FormatException fe)
            {
                throw new FormatException("The specified file is invalid format.", fe);
            }
        }

        #region Overrides

        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            unsafe
            {
                var problem = (NativeMethods.svm_problem*)this.NativePtr;
                var len = problem->l;
                for (var i = 0; i < len; i++)
                    Marshal.FreeCoTaskMem((IntPtr)problem->x[i]);

                Marshal.FreeCoTaskMem((IntPtr)problem->x);
                Marshal.FreeCoTaskMem((IntPtr)problem->y);
            }
        }

        #endregion

        #endregion

    }

}