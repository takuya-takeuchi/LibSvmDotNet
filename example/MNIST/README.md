# MNIST

## How to use?

## 1. Preparation

This sample requires MNIST train and test data set.  
Please download the following files.

* https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass/mnist.bz2
* https://www.csie.ntu.edu.tw/~cjlin/libsvmtools/datasets/multiclass/mnist.t.bz2

Then, extracts the above files.  
At last, deploy **mnist** and **mnist.t** to &lt;MNIST_dir&gt;.

## 2. Build

1. Open command prompt and change to &lt;MNIST_dir&gt;
1. Type the following command
````
dotnet build -c Release
````
3. Copy ***libsvm.dll*** to output directory; &lt;MNIST_dir&gt;\bin\Release\netcoreapp2.0.

**NOTE**  
If you want to run at Linux, you should build the **LibSvmDotNet** at first.  
Please refer the [Tutorial for Linux](https://github.com/takuya-takeuchi/LibSvmDotNet/wiki/Tutorial-for-Linux).


## 3. Run

1. Open command prompt and change to &lt;MNIST_dir&gt;
1. Type the following command
````
dotnet run -c Release -- "-o=trained.model"
````
After this, command prompt will show result like the following.
````
Accuracy: 83.97%, Elapsed: 34710s
````

## 4. Et cetera

This program support the following argument and option.

### Argument

|Argument|Description|
|:-----------|:------------|
|quiet|Supress the LIBSVM output|

### Option

|Option|Short form|Description|
|:-----------|:------------|:------------|
|--output|-o|Output trained model to specified file path|