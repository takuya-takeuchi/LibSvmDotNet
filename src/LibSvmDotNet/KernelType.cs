// ReSharper disable InconsistentNaming
namespace LibSvmDotNet
{

    /// <summary>
    /// The KernelType enumeration specifies the kernel type for Support Vector Machine.
    /// </summary>
    public enum KernelType
    {

        /// <summary>
        /// Linear kernel.
        /// </summary>
        Linear,

        /// <summary>
        /// Polynomial kernel.
        /// </summary>
        Polynomial,

        /// <summary>
        /// Radial Basis Function kernel.
        /// </summary>
        RBF,

        /// <summary>
        /// Sigmoid kernel.
        /// </summary>
        Sigmoid,

        /// <summary>
        /// Precomputed kernel.
        /// </summary>
        Precomputed

    }

}