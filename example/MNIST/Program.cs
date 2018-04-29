using System;
using System.Diagnostics;
using LibSvmDotNet;
using Microsoft.Extensions.CommandLineUtils;

namespace MNIST
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            var app = new CommandLineApplication(false);
            app.Name = nameof(MNIST);
            app.Description = "The exsample program for MNIST";
            app.HelpOption("-h|--help");

            var quietArgument = app.Argument("quiet", "Suppress output of LIBSVM");
            var outputOption = app.Option("-o|--output", "output path for trained model", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (quietArgument.Value != null)
                    LibSvm.SetPrintFunction(null);

                var output = outputOption.Value();

                // Load training data and test data set
                using (var train = Problem.FromFile("mnist"))
                using (var test = Problem.FromFile("mnist.t"))
                {
                    // Configure parameter
                    var param = new Parameter
                    {
                        SvmType = SvmType.CSVC,
                        KernelType = KernelType.Linear,
                        Gamma = 0.05d,
                        C = 5d,
                        CacheSize = 1000,
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

                    // Train training data
                    var sw = new Stopwatch();
                    sw.Start();
                    using (var model = LibSvm.Train(train, param))
                    {
                        sw.Stop();

                        if(!string.IsNullOrWhiteSpace(output))
                            Model.Save(output, model);

                        var correct = 0;
                        var total = 0;
                        var x = test.X;
                        var y = test.Y;
                        for (var i = 0; i < test.Length; i++)
                        {
                            // Get vector from test data
                            var array = x[i];

                            // Get classification result (returns label)
                            var ret1 = (int)LibSvm.Predict(model, array);
                            if (ret1 == (int)y[i])
                                correct++;

                            total++;
                        }

                        Console.WriteLine($"Accuracy: {correct / (double)total * 100}%, Elapsed: {sw.ElapsedMilliseconds / 1000}s");
                    }
                }

                return 0;
            });

            app.Execute(args);
        }

    }

}
