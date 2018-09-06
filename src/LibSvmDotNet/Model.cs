using System;
using System.IO;
using System.Runtime.InteropServices;
using LibSvmDotNet.Interop;

namespace LibSvmDotNet
{

    /// <summary>
    /// Represents an trained model.
    /// </summary>
    public sealed class Model : DisposableObject
    {

        #region Constructors

        internal unsafe Model(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException($"{nameof(ptr)} should not be IntPtr.Zero");

            // NOTE
            // Detail of each member size: https://github.com/cjlin1/libsvm/blob/master/README
            this.NativePtr = ptr;
            var model = (NativeMethods.svm_model*)ptr;

            this.Parameter = new Parameter(&model->param);
            this.Classes = model->nr_class;
            this.L = model->l;

            this.Label = Copy(model->label, this.Classes);
            this.NumberOfSupportVector = Copy(model->nSV, this.Classes);
            this.FreeSupportVector = model->free_sv == NativeMethods.True;

            this.SupportVector = new NodeArrayCollecion(model->SV, this.NumberOfSupportVector);
            if (this.Classes > 0)
            {
                this.SupportVectorCoefficients = new double[this.Classes - 1][];
                for (var index = 0; index < this.SupportVectorCoefficients.Length; index++)
                    this.SupportVectorCoefficients[index] = Copy(model->sv_coef[index], this.L);
            }

            if (model->sv_indices != null)
                this.SupportVectorIndices = Copy(model->sv_indices, this.L);

            var length = (this.Classes - 1) * this.Classes / 2;
            this.Rho = Copy(model->rho, length);

            if ((IntPtr)model->probA != IntPtr.Zero)
                this.ProbabilityA = Copy(model->probA, length);
            if ((IntPtr)model->probB != IntPtr.Zero)
                this.ProbabilityB = Copy(model->probB, length);

            this.IsEstimableProbability = NativeMethods.svm_check_probability_model(model) == NativeMethods.True;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is created by <see cref="Load"/>.
        /// </summary>
        /// <returns>true if this instance is created by <see cref="Load"/>; otherwise, false.</returns>
        public bool FreeSupportVector
        {
            get;
        }

        /// <summary>
        /// Indicates whether this model contains required information to do probability estimates.
        /// </summary>
        /// <returns>true if this model contains required information to do probability estimates; otherwise, false.</returns>
        public bool IsEstimableProbability
        {
            get;
        }

        /// <summary>
        /// Gets the number of support vector.
        /// </summary>
        public int L
        {
            get;
        }

        /// <summary>
        /// Gets the array for label of each class for a classification model.
        /// </summary>
        public int[] Label
        {
            get;
        }

        /// <summary>
        /// Gets the number of classes.
        /// </summary>
        public int Classes
        {
            get;
        }

        /// <summary>
        /// Gets the array contains number of support vector for each class for a classification model.
        /// </summary>
        public int[] NumberOfSupportVector
        {
            get;
        }

        /// <summary>
        /// Gets the parameter of this model.
        /// </summary>
        public Parameter Parameter
        {
            get;
        }

        /// <summary>
        /// Gets the pairwise probability information.
        /// </summary>
        public double[] ProbabilityA
        {
            get;
        }

        /// <summary>
        /// Gets the pairwise probability information.
        /// </summary>
        public double[] ProbabilityB
        {
            get;
        }

        /// <summary>
        /// Gets the constants in decision functions.
        /// </summary>
        public double[] Rho
        {
            get;
        }

        /// <summary>
        /// Gets the support vector.
        /// </summary>
        public NodeArrayCollecion SupportVector
        {
            get;
        }

        /// <summary>
        /// Gets the coefficients for support vector in decision functions.
        /// </summary>
        public double[][] SupportVectorCoefficients
        {
            get;
        }

        /// <summary>
        /// Gets the array to indicate support vector in the training set
        /// </summary>
        public int[] SupportVectorIndices
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads an <see cref="Model"/> given the specified file.
        /// </summary>
        /// <param name="path">The LIBSVM format file name and path.</param>
        /// <returns>This method returns a new <see cref="Model"/> for the specified file.</returns>
        /// <exception cref="ArgumentException">The specified path is null or whitespace.</exception>
        /// <exception cref="FileNotFoundException">The specified file is not found.</exception>
        /// <exception cref="FormatException">The specified file is invalid format.</exception>
        public static Model Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The specified path is null or whitespace.");

            if (!File.Exists(path))
                throw new FileNotFoundException("The specified file is not found.");

            unsafe
            {
                var ret = NativeMethods.svm_load_model(path);
                return new Model((IntPtr)ret);
            }
        }

        /// <summary>
        /// Saves this <see cref="Model"/> to the specified file.
        /// </summary>
        /// <param name="path">The file to write to.</param>
        /// <param name="model">The model to write to the file.</param>
        /// <exception cref="LibSvmException">Failed to save model to the specified file.</exception>
        /// <exception cref="ObjectDisposedException">Cannot access a disposed object.</exception>
        public static void Save(string path, Model model)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The specified path is null or whitespace.");

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.ThrowIfDisposed();

            unsafe
            {
                if (NativeMethods.svm_save_model(path, (NativeMethods.svm_model*)model.NativePtr) != NativeMethods.OK)
                    throw new LibSvmException();
            }
        }

        #region Overrides

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();
            NativeMethods.svm_free_model_content(this.NativePtr);
            NativeMethods.free(this.NativePtr);
        }

        #endregion

        #region Helpers

        private static unsafe double[] Copy(double* src, int len)
        {
            var array = new double[len];
            Marshal.Copy((IntPtr)src, array, 0, len);
            return array;
        }

        private static unsafe int[] Copy(int* src, int len)
        {
            var array = new int[len];
            Marshal.Copy((IntPtr)src, array, 0, len);
            return array;
        }

        #endregion

        #endregion

    }
}
