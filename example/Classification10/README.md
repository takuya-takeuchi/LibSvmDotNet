# Classification10
 
The problem is what classify float value 0 or greater but less than 10 into 10 classes.

## How to use?

## 1. Build

1. Open command prompt and change to &lt;Classification10_dir&gt;
1. Type the following command
````
dotnet build -c Release
````
2. Copy ***libsvm.dll*** to output directory; &lt;Classification10_dir&gt;\bin\Release\netcoreapp2.0.

**NOTE**  
If you want to run at Linux, you should build the **LibSvmDotNet** at first.  
Please refer the [Tutorial for Linux](https://github.com/takuya-takeuchi/LibSvmDotNet/wiki/Tutorial-for-Linux).


## 2. Run

1. Open command prompt and change to &lt;Classification10_dir&gt;
1. Type the following command
````
dotnet run -c Release
````
After this, command prompt will show result like the following.
````
Accuracy: 99.5%
````