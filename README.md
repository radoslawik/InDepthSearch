# InDepthSearch
![version](https://img.shields.io/badge/version-0.2.0-blue)

Search for keywords inside the documents on Windows, Linux and macOS!

![image](https://user-images.githubusercontent.com/55437425/118365572-9806af80-b59d-11eb-92c0-655f7824742c.png)
![image](https://user-images.githubusercontent.com/55437425/118365601-b8366e80-b59d-11eb-9932-c2ceacc82a21.png)


## 📖 About
InDepthSearch is a multi-platform desktop search engine to find the keywords inside the documents content, in both text and images. Currently supported platforms:
 - Windows 10 (x86 and x64)
 - Linux (x64)
 - macOS (x64)

**NOTE:** Currently the project is in pre-release state, some functionalities are not implemented yet and some of them work only on a specific platform. For more information see the section below.

## ✔️ Features
The table below lists the features and their compatibility with the supported platforms:

| Name  | Windows | Linux  | macOS |
| ------------- |:---------------:|:---------------:|:---------------:|
| Multiple keywords search<sup>*</sup> | - | -  | - |
| PDF support | + | + | + |
| DOC/DOCX support<sup>**</sup> | + | + | + |
| ODT support<sup>**</sup> | - | - | - |
| Search in subfolders | + | + | + |
| Case-sensitive search | + | + | + |
| Search in images<sup>***</sup> | + | - | - |

<sup>*</sup>  Currently the user is allowed to search only for one word/phrase. Support for multiple search entries is provisioned for next pre-release.

<sup>**</sup>  ODT format is not available yet.

<sup>***</sup>  Optical Character Recognition (OCR) libraries are compatible only with Windows, therefore search in images is disabled on Linux and macOS for the moment.

## 🚀 Quick start!
If you just want to run the application follow the instructions below. If you want to build the project first, go to to **Build & run** section.

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
The code in this repository is licensed under the **Apache License 2.0**

**NOTE:** The project would not exist without following resources:

 - [Avalonia](https://github.com/AvaloniaUI/Avalonia), [ReactiveUI](https://github.com/reactiveui/ReactiveUI), [DocNET](https://github.com/GowenGit/docnet) and [Open-XML-SDK](https://github.com/OfficeDev/Open-XML-SDK) licensed under MIT.
 - [tesseract](https://github.com/charlesw/tesseract) licensed under the Apache License 2.0.


