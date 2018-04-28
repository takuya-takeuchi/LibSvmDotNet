using System;
using System.IO;
using System.Text;
using LibSvmDotNet;
using Microsoft.Extensions.CommandLineUtils;

namespace CrossValidation
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            var app = new CommandLineApplication(false);
            app.Name = nameof(CrossValidation);
            app.Description = "The exsample program for cross validation";
            app.HelpOption("-h|--help");

            var quietArgument = app.Argument("quiet", "Suppress output of LIBSVM");
            var foldOption = app.Option("-f|--fold", "K-fold. (An integer of not less than 0)", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (quietArgument.Value != null)
                    LibSvm.SetPrintFunction(null);

                if (!foldOption.HasValue())
                {
                    app.ShowHelp();
                    return -1;
                }

                if (!int.TryParse(foldOption.Value(), out var fold))
                {
                    app.ShowHelp();
                    return -1;
                }

                // Create random data
                var r = new Random();
                const int trainCount = 500;

                var temp = Path.GetTempPath();
                var trainDic = new StringBuilder();

                for (var l = 0; l < 10; l++)
                {
                    for (var i = 0; i < trainCount; i++)
                    {
                        // Create decimal value 0 or greater but less than 10
                        var v = r.NextDouble() + l;
                        trainDic.AppendLine($"{l} 1:{v}");
                    }
                }

                var tempTrainPath = Path.Combine(temp, "train");

                using (var fs = new FileStream(tempTrainPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                using (var sw = new StreamWriter(fs, Encoding.ASCII))
                    sw.Write(trainDic.ToString());

                // Load training data and test data set
                using (var train = Problem.FromFile(tempTrainPath))
                {
                    // Configure parameter
                    var param = new Parameter
                    {
                        SvmType = SvmType.CSVC,
                        KernelType = KernelType.RBF,
                        Gamma = 0.05d,
                        C = 5d,
                        CacheSize = 100,
                        Degree = 3,
                        Coef0 = 0,
                        Nu = 0.5,
                        Epsilon = 1e-3,
                        P = 0.1,
                        Shrinking = true,
                        Probability = false,
                        WeightLabel = new int[0],
                        Weight = new double[0]
                    };

                    var message = LibSvm.CheckParameter(train, param);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Console.WriteLine($"Error: {message} for train problem");
                        return -1;
                    }

                    // Do cross validation
                    LibSvm.CrossValidation(train, param, fold, out var target);

                    var correct = 0;
                    var total = 0;
                    var y = train.Y;
                    for (var i = 0; i < train.Length; i++)
                    {
                        // Compare predict result and train data label
                        if ((int)y[i] == (int)target[i])
                            correct++;

                        total++;
                    }

                    Console.WriteLine($"Accuracy: {correct / (double)total * 100}%");
                }

                return 0;
            });

            app.Execute(args);
        }

    }

}
