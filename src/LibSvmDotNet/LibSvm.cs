using System;
using System.Runtime.InteropServices;
using LibSvmDotNet.Interop;

namespace LibSvmDotNet
{

    /// <summary>
    /// Provides functions of LIBSVM.
    /// </summary>
    public static class LibSvm
    {

        #region Callback

        /// <summary>
        /// Encapsulates a method that has a string parameter and does not return a value.
        /// </summary>
        /// <param name="message">The message given from LIBSVM.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PrintFunc(string message);

        #endregion

        #region Fields

        private static GCHandle PrintFuncHandle;

        #endregion

        #region Methods

        /// <summary>
        /// Checks whether the parameters are within the feasible range of the problem.
        /// </summary>
        /// <param name="problem"><see cref="Problem"/>.</param>
        /// <param name="parameter"><see cref="Parameter"/>.</param>
        /// <returns>It returns null if the parameters are feasible, otherwise an error message is returned.</returns>
        public static string CheckParameter(Problem problem, Parameter parameter)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            problem.ThrowIfDisposed();

            unsafe
            {
                var param = new NativeMethods.svm_parameter();

                try
                {
                    // This method throw exception when there is some errors.
                    // This error check is not official's.
                    param = parameter.ToNative();
                }
                catch (LibSvmException lle)
                {
                    return lle.Message;
                }

                try
                {
                    var ret = NativeMethods.svm_check_parameter((NativeMethods.svm_problem*)problem.NativePtr, &param);
                    return Marshal.PtrToStringAnsi(ret);
                }
                finally
                {
                    Free(&param);
                }
            }
        }

        /// <summary>
        /// Conducts cross validation.
        /// </summary>
        /// <param name="problem"><see cref="Problem"/>.</param>
        /// <param name="parameter"><see cref="Parameter"/>.</param>
        /// <param name="fold">The number of division for samples.</param>
        /// <param name="target">The predicted labels (of all prob's instances) in the validation process will be stored.</param>
        public static void CrossValidation(Problem problem, Parameter parameter, int fold, out double[] target)
        {
            target = null;

            if (problem == null)
                throw new ArgumentNullException(nameof(problem));
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            problem.ThrowIfDisposed();

            unsafe
            {
                var param = parameter.ToNative();

                try
                {
                    target = new double[problem.Length];
                    var prob = (NativeMethods.svm_problem*)problem.NativePtr;
                    NativeMethods.svm_cross_validation(prob, &param, fold, target);
                }
                finally
                {
                    Free(&param);
                }
            }
        }

        /// <summary>
        /// Does classification or regression on a test vector x given a model.
        /// </summary>
        /// <param name="model"><see cref="Model"/>.</param>
        /// <param name="x">The test vector.</param>
        /// <returns>
        /// <para>For a classification model, the predicted class for x is returned.</para>
        /// <para>For a regression model, the function value of x calculated using the model is returned. For an one-class model, +1 or -1 is returned.</para>
        /// </returns>
        public static double Predict(Model model, NodeArray x)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            unsafe
            {
                var m = model.NativePtr;
                return NativeMethods.svm_predict(m, (NativeMethods.svm_node*)x.NativePtr);
            }
        }

        /// <summary>
        /// Does classification or regression on a test vector x given a model.
        /// </summary>
        /// <param name="model"><see cref="Model"/>.</param>
        /// <param name="x">The test vector.</param>
        /// <param name="probability">When this method returns, contains probability estimates if succeeded, or null if failed.</param>
        /// <returns>
        /// <para>For a classification model, the predicted class for x is returned.</para>
        /// <para>For a regression model, the function value of x calculated using the model is returned. For an one-class model, +1 or -1 is returned.</para>
        /// </returns>
        /// <remarks>This methods returns valid probability when <see cref="Model.ProbabilityA"/> and <see cref="Model.ProbabilityB"/> are not null.</remarks>
        public static double Predict(Model model, NodeArray x, out double[] probability)
        {
            probability = null;

            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            unsafe
            {
                var m = model.NativePtr;
                probability = new double[model.Classes];
                return NativeMethods.svm_predict_probability(m, (NativeMethods.svm_node*)x.NativePtr, probability);
            }
        }

        /// <summary>
        /// Gives decision values on a test vector x given a model, and return the predicted label(classification) or the function value(regression).
        /// </summary>
        /// <param name="model"><see cref="Model"/>.</param>
        /// <param name="x">The test vector.</param>
        /// <param name="decisionValues">When this method returns, contains decision values if succeeded, or null if failed.</param>
        /// <returns>
        /// <para>For a classification model, the predicted class for x and decision values.</para>
        /// <para>For a regression model, <code>decisionValues[0]</code> and the returned value are both the function value of x calculated using the model. </para>
        /// <para>For a one-class model, <code>decisionValues[0]</code> is the decision value of x, while the returned value is +1/-1.</para>
        /// </returns>
        public static double PredictValues(Model model, NodeArray x, out double[] decisionValues)
        {
            decisionValues = null;

            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            unsafe
            {
                var m = model.NativePtr;
                switch (model.Parameter.SvmType)
                {
                    case SvmType.CSVC:
                    case SvmType.NuSVC:
                        var length = model.Classes * (model.Classes - 1) / 2;
                        decisionValues = new double[length];
                        return NativeMethods.svm_predict_values(m, (NativeMethods.svm_node*)x.NativePtr, decisionValues);
                    default:
                        decisionValues = new double[1];
                        return NativeMethods.svm_predict_values(m, (NativeMethods.svm_node*)x.NativePtr, decisionValues);
                }
            }
        }

        /// <summary>
        /// Specify output fpr LIBSVM.
        /// </summary>
        /// <param name="printFunc">The callback to receive the output and process.</param>
        /// <remarks>If specify null, it suppress output from LIBSVM.</remarks>
        public static void SetPrintFunction(PrintFunc printFunc)
        {
            if (printFunc == null)
                printFunc = PrintNull;

            var fp = new PrintFunc(printFunc);
            var handle = GCHandle.Alloc(fp);
            var ip = Marshal.GetFunctionPointerForDelegate(fp);

            NativeMethods.svm_set_print_string_function(ip);

            if (PrintFuncHandle.IsAllocated)
                PrintFuncHandle.Free();

            PrintFuncHandle = handle;
        }

        /// <summary>
        /// Does constructs and returns an Support Vector Machine model according to the given training data and parameters.
        /// </summary>
        /// <param name="problem"><see cref="Problem"/>.</param>
        /// <param name="parameter"><see cref="Parameter"/>.</param>
        /// <returns>This method returns a new <see cref="Model"/>.</returns>
        public static Model Train(Problem problem, Parameter parameter)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            problem.ThrowIfDisposed();

            unsafe
            {
                var param = parameter.ToNative();

                try
                {
                    var ret = NativeMethods.svm_train((NativeMethods.svm_problem*)problem.NativePtr, &param);
                    if ((IntPtr)ret == IntPtr.Zero)
                        throw new LibSvmException("svm_train returns null");

                    return new Model((IntPtr)ret);
                }
                finally
                {
                    Free(&param);
                }
            }
        }

        #region Helpers

        internal static unsafe void Free(NativeMethods.svm_parameter* parameter)
        {
            if ((IntPtr)parameter->weight != IntPtr.Zero)
                NativeMethods.free((IntPtr)parameter->weight);

            if ((IntPtr)parameter->weight_label != IntPtr.Zero)
                NativeMethods.free((IntPtr)parameter->weight_label);

            parameter->weight = (double*)IntPtr.Zero;
            parameter->weight_label = (int*)IntPtr.Zero;
        }

        private static void PrintNull(string message)
        {

        }

        private static unsafe NativeMethods.svm_parameter ToNative(this Parameter parameter)
        {
            var ret = new NativeMethods.svm_parameter
            {
                svm_type = (int)parameter.SvmType,
                kernel_type = (int)parameter.KernelType,
                degree = parameter.Degree,
                gamma = parameter.Gamma,
                coef0 = parameter.Coef0,
                cache_size = parameter.CacheSize,
                eps = parameter.Epsilon,
                C = parameter.C,
                nr_weight = parameter.LengthOfWeight,
                nu = parameter.Nu,
                p = parameter.P,
                shrinking = parameter.Shrinking ? NativeMethods.True : NativeMethods.False,
                probability = parameter.Probability ? NativeMethods.True : NativeMethods.False
            };

            if (ret.nr_weight > 0)
            {
                if (parameter.WeightLabel == null)
                    throw new LibSvmException("Parameter.WeightLabel is null although Parameter.LengthOfWeight is over 0.");
                if (parameter.Weight == null)
                    throw new LibSvmException("Parameter.Weight is null although Parameter.LengthOfWeight is over 0.");

                var len1 = parameter.WeightLabel.Length;
                var len2 = parameter.Weight.Length;
                if(len1 != ret.nr_weight || len2 != ret.nr_weight)
                    throw new LibSvmException("Parameter.WeightLabel.Length does not match Parameter.Weight.Length");
            }

            var failAlloc = false;

            try
            {
                if (ret.nr_weight > 0)
                {
                    ret.weight_label = (int*)NativeMethods.malloc(sizeof(int), ret.nr_weight);
                    fixed (int* p = &parameter.WeightLabel[0])
                        NativeMethods.memcpy(ret.weight_label, p, ret.nr_weight * sizeof(int));

                    ret.weight = (double*)NativeMethods.malloc(sizeof(double), ret.nr_weight);
                    fixed (double* p = &parameter.Weight[0])
                        NativeMethods.memcpy(ret.weight, p, ret.nr_weight * sizeof(double));
                }

                failAlloc = true;
            }
            finally
            {
                if (!failAlloc)
                {
                    NativeMethods.free((IntPtr)ret.weight_label);
                    NativeMethods.free((IntPtr)ret.weight);
                }
            }

            return ret;
        }

        internal static unsafe NativeMethods.svm_node* ToNative(this Node[] node)
        {
            var len = node.Length;
            var ptr = (NativeMethods.svm_node*)NativeMethods.malloc(sizeof(NativeMethods.svm_node), (node.Length + 1));
            fixed (Node* pX = &node[0])
            {
                for (var j = 0; j < len; j++)
                    ptr[j] = new NativeMethods.svm_node
                    {
                        index = pX[j].Index,
                        value = pX[j].Value
                    };

                // delimiter
                ptr[len].index = -1;
                ptr[len].value = 0;
            }

            return ptr;
        }

        #endregion

        #endregion

    }

}

