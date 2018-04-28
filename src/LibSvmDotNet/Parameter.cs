using System;
using System.Runtime.InteropServices;
using LibSvmDotNet.Interop;

namespace LibSvmDotNet
{

    /// <summary>
    /// Represents an parameter for Support Vector Machine.
    /// </summary>
    public sealed class Parameter
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        public Parameter()
        {
        }

        internal unsafe Parameter(NativeMethods.svm_parameter* param)
        {
            this.C = param->C;
            this.CacheSize = param->cache_size;
            this.Coef0 = param->coef0;
            this.Degree = param->degree;
            this.Epsilon = param->eps;
            this.Gamma = param->gamma;
            this.KernelType = (KernelType)param->kernel_type;
            this.LengthOfWeight = param->nr_weight;
            this.Nu = param->nu;
            this.P = param->p;
            this.Probability = param->probability == NativeMethods.True;
            this.SvmType = (SvmType)param->svm_type;
            this.Shrinking = param->shrinking == NativeMethods.True;

            if (this.LengthOfWeight > 0)
            {
                var darray = new double[this.LengthOfWeight];
                Marshal.Copy((IntPtr)param->weight, darray, 0, this.LengthOfWeight);
                this.Weight = darray;

                var iarray = new int[this.LengthOfWeight];
                Marshal.Copy((IntPtr)param->weight_label, iarray, 0, this.LengthOfWeight);
                this.WeightLabel = iarray;
            }
            else
            {
                this.Weight = null;
                this.WeightLabel = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cost parameter.
        /// </summary>
        public double C
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cache memory size in MB.
        /// </summary>
        public double CacheSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the coef0 parameter in kernel function.
        /// </summary>
        public double Coef0
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the degree parameter in kernel function.
        /// </summary>
        public int Degree
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tolerance of termination criterion.
        /// </summary>
        public double Epsilon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gamma parameter in kernel function.
        /// </summary>
        public double Gamma
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of kernel function.
        /// </summary>
        public KernelType KernelType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of elements in the array <see cref="Weight"/> and <see cref="WeightLabel"/>.
        /// </summary>
        public int LengthOfWeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the nu parameter of SvmType.NuSVC, SvmType.OneClass, and SvmType.NuSVR.
        /// </summary>
        public double Nu
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the epsilon in loss function of SvmType.EpsilonSVR.
        /// </summary>
        public double P
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets indicating whether to train a SVC or SVR model for probability estimates.
        /// </summary>
        public bool Probability
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets indicating whether to use the shrinking heuristics.
        /// </summary>
        public bool Shrinking
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets type of Support Vector Machine.
        /// </summary>
        public SvmType SvmType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the array of factors for penalty.
        /// </summary>
        /// <remarks>Each <code>Weight[i]</code> corresponds to <code>WeightLabel[i]</code>.</remarks>
        public double[] Weight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the array of labels for group training data of the same class.
        /// </summary>
        /// <remarks>Each <code>Weight[i]</code> corresponds to <code>WeightLabel[i]</code>.</remarks>
        public int[] WeightLabel
        {
            get;
            set;
        }

        #endregion

    }

}