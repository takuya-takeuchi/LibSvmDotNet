# Cross Validation

This exsample demonstrates cross validation process of LIBSVM.  
The problem is what classify float value 0 or greater but less than 10 into 10 classes.

## How to use?

## 1. Build

1. Open command prompt and change to &lt;CrossValidation_dir&gt;
1. Type the following command
````
dotnet build -c Release
````
2. Copy ***libsvm.dll*** to output directory; &lt;CrossValidation_dir&gt;\bin\Release\netcoreapp2.0.

**NOTE**  
If you want to run at Linux, you should build the **LibSvmDotNet** at first.  
Please refer the [Tutorial for Linux](https://github.com/takuya-takeuchi/LibSvmDotNet/wiki/Tutorial-for-Linux).


## 2. Run

1. Open command prompt and change to &lt;CrossValidation_dir&gt;
1. Type the following command
````
dotnet run -c Release -- "-f=5"
````
After this, command prompt will show result like the following.
````
Accuracy: 99.14%
````

## 3. Et cetera

This program support the following argument and option.

### Argument

|Argument|Description|
|:-----------|:------------|
|quiet|Supress the LIBSVM output|

### Option

|Option|Short form|Description|
|:-----------|:------------|:------------|
|--fold|-f|Specify K-fold. (An integer of not less than 0)|