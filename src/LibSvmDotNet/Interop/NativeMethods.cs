using System;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace LibSvmDotNet.Interop
{

    internal static unsafe class NativeMethods
    {

#if LINUX
        public const string CLibrary = "glibc.so";

        public const string NativeLibrary = "libsvm.so";

        public const CallingConvention CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl;
#else
        public const string CLibrary = "msvcrt.dll";

        public const string NativeLibrary = "libsvm.dll";

        public const CallingConvention CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl;
#endif

        #region Constants

        public const int OK = 0;

        public const int Error = -1;

        public const int True = 1;

        public const int False = 0;

        #endregion

        #region Methods

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr svm_check_parameter(svm_problem* prob, svm_parameter* param);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int svm_check_probability_model(svm_model* model);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void svm_cross_validation(svm_problem* prob, svm_parameter* param, int nr_fold, double[] target);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern svm_model* svm_load_model([MarshalAs(UnmanagedType.LPStr)] string model_file_name);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern double svm_predict(IntPtr model, svm_node* x);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern double svm_predict_probability(IntPtr model, svm_node* x, double[] prob_estimates);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern double svm_predict_values(IntPtr model, svm_node* x, double[] dec_values);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int svm_save_model([MarshalAs(UnmanagedType.LPStr)]string model_file_name, svm_model* model);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern svm_model* svm_train(svm_problem* prob, svm_parameter* param);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void svm_set_print_string_function(IntPtr func);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(CLibrary, EntryPoint = "memcpy", CallingConvention = CallingConvention)]
        public static extern IntPtr memcpy(int* dest, int* src, int count);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(CLibrary, EntryPoint = "memcpy", CallingConvention = CallingConvention)]
        public static extern IntPtr memcpy(double* dest, double* src, int count);

        public static void free(IntPtr dest)
        {
            Marshal.FreeCoTaskMem(dest);
        }

        public static IntPtr malloc(int size, int length)
        {
            return Marshal.AllocCoTaskMem(size * length);
        }

        #region svm_model

        [StructLayout(LayoutKind.Explicit, Size = 184)]
        internal struct svm_model
        {

            #region Fields

            [FieldOffset(0)]
            public svm_parameter param;	/* parameter */

            [FieldOffset(104)]
            public int nr_class;        /* number of classes, = 2 in regression/one class svm */

            [FieldOffset(108)]
            public int l;               /* total #SV */

            [FieldOffset(112)]
            public svm_node** SV;       /* SVs (SV[l]) */

            [FieldOffset(120)]
            public double** sv_coef;    /* coefficients for SVs in decision functions (sv_coef[k-1][l]) */

            [FieldOffset(128)]
            public double* rho;         /* constants in decision functions (rho[k*(k-1)/2]) */

            [FieldOffset(136)]
            public double* probA;       /* pariwise probability information */

            [FieldOffset(144)]
            public double* probB;

            [FieldOffset(152)]
            public int* sv_indices;     /* sv_indices[0,...,nSV-1] are values in [1,...,num_traning_data] to indicate SVs in the training set */

            /* for classification only */

            [FieldOffset(160)]
            public int* label;     /* label of each class (label[k]) */

            [FieldOffset(168)]
            public int* nSV;       /* number of SVs for each class (nSV[k]) */
                                   /* nSV[0] + nSV[1] + ... + nSV[k-1] = l */
                                   /* XXX */

            [FieldOffset(176)]
            public int free_sv;    /* 1 if svm_model is created by svm_load_model*/
            　　　　　　　　　　　 /* 0 if svm_model is created by svm_train */

            #endregion

        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void svm_free_model_content(IntPtr model);

        #endregion

        #region svm_node

        [StructLayout(LayoutKind.Sequential, Size = 16)]
        internal struct svm_node
        {

            #region Fields

            public int index;

            public double value;

            #endregion

        }

        #endregion

        #region svm_parameter

        [StructLayout(LayoutKind.Explicit, Size = 104)]
        internal struct svm_parameter
        {

            #region Fields

            [FieldOffset(0)]
            public int svm_type;

            [FieldOffset(4)]
            public int kernel_type;

            [FieldOffset(8)]
            public int degree; /* for poly */

            [FieldOffset(16)]
            public double gamma;   /* for poly/rbf/sigmoid */

            [FieldOffset(24)]
            public double coef0;   /* for poly/sigmoid */

            /* these are for training only */

            [FieldOffset(32)]
            public double cache_size; /* in MB */

            [FieldOffset(40)]
            public double eps; /* stopping criteria */

            [FieldOffset(48)]
            public double C;   /* for C_SVC, EPSILON_SVR and NU_SVR */

            [FieldOffset(56)]
            public int nr_weight;      /* for C_SVC */

            [FieldOffset(64)]
            public int* weight_label;  /* for C_SVC */

            [FieldOffset(72)]
            public double* weight;     /* for C_SVC */

            [FieldOffset(80)]
            public double nu;  /* for NU_SVC, ONE_CLASS, and NU_SVR */

            [FieldOffset(88)]
            public double p;   /* for EPSILON_SVR */

            [FieldOffset(96)]
            public int shrinking;  /* use the shrinking heuristics */

            [FieldOffset(100)]
            public int probability; /* do probability estimates */

            #endregion

        }

        #endregion

        #region svm_problem

        [StructLayout(LayoutKind.Sequential, Size = 24)]
        internal struct svm_problem
        {

            #region Fields

            public int l;

            public double* y;

            public svm_node** x;

            #endregion

        }

        #endregion

        #endregion

    }

}
