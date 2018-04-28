// ReSharper disable InconsistentNaming
namespace LibSvmDotNet
{

    /// <summary>
    /// The SvmType enumeration specifies the type of Support Vector Machine.
    /// </summary>
    public enum SvmType
    {

        /// <summary>
        /// C-Support Vector Classification.
        /// </summary>
        CSVC,

        /// <summary>
        /// nu-Support Vector Classification.
        /// </summary>
        NuSVC,

        /// <summary>
        /// One-class Support Vector Machine.
        /// </summary>
        OneClass,

        /// <summary>
        /// epsilon-Support Vector Regression.
        /// </summary>
        EpsilonSVR,

        /// <summary>
        /// nu-Support Vector Regression.
        /// </summary>
        NuSVR

    }

}