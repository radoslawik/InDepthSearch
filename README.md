<img src="https://raw.githubusercontent.com/radoslawik/InDepthSearch/master/InDepthSearch.UI/Assets/ids-icon.ico" width="200">

# InDepthSearch
Search for keywords inside the documents on Windows, Linux and macOS!

![image](https://user-images.githubusercontent.com/55437425/115236262-6ffd6b00-a11b-11eb-8ca5-43a1542e05c5.png)

## 📖 About
InDepthSearch is a multi-platform desktop search engine to find the keywords inside the documents content, in both text and images. Currently supported platforms:
 - Windows 10 (x86 and x64)
 - Linux (x64)
 - macOS (x64)

**NOTE:** Currently the project is in pre-release state, some functionalities are not implemented yet and some of them work only on a specific platform.

## ✔️ Features
The table below lists the features and their compatibility with the supported platforms:

| Name  | Windows | Linux  | macOS |
| ------------- |:---------------:|:---------------:|:---------------:|
| Multiple keywords search<sup>*</sup> | - | -  | - |
| PDF support | + | + | + |
| DOC/DOCX support<sup>**</sup> | - | - | - |
| ODT support<sup>**</sup> | - | - | - |
| Search in subfolders | + | + | + |
| Case-sensitive search | + | + | + |
| Search in images<sup>***</sup> | + | - | - |

<sup>*</sup>  Currently the user is allowed to search only for one word/phrase. Support for multiple search entries is provisioned for next pre-release.

<sup>**</sup>  Only PDF format is supported at the moment, but in the future it is planned to extend it to DOC, DOCX and ODT.

<sup>**</sup>  Optical Character Recognition (OCR) libraries are compatible only with Windows, therefore search in images is disabled on Linux and macOS for the moment.

## 🚀 Quick start!
~~If you just want to run the application follow the instructions below. If you want to build the project first, go to to **Build & run** section.~~

**NOTE:** Quick start will be available starting with `0.2.0` version.

**Quick start** steps:
 - Make sure you have [**.NET Core 5.0 runtime**](https://dotnet.microsoft.com/download/dotnet/5.0/runtime "**.NET Core 5.0 runtime**") installed
 - Go to [releases](https://github.com/radoslawik/InDepthSearch/releases "releases") and download the latest archive asset for your platform
 - Unpack the content of the archive on your disc
 - Run **InDepthSearch** executable!

## ⛏️ Build & run!
In order to build and run the project you need to:
- Make sure you have[ **.NET Core 5.0 runtime and SDK**](https://dotnet.microsoft.com/download " **.NET Core 5.0 runtime and SDK**") installed
- Download the code using `git clone` or ZIP archive
- Navigate to the root repository folder
- Build the project with the terminal command
`dotnet run --project ./InDepthSearch.UI/InDepthSearch.UI.csproj` .
Alternatively you can also open the `.sln` file in Visual Studio 2019 and build `InDepthSearch.UI` project. Voilà!

## ⚖️ License
The code in this repository is licensed under the **Apache-2.0 License**

**NOTE:** [tesseract-ocr/tesseract](https://github.com/tesseract-ocr/tesseract/blob/master/LICENSE "tesseract-ocr/tesseract") (project dependency) is licensed under the Apache License 2.0.
