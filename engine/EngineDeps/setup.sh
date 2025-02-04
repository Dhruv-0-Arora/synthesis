#!/bin/bash

text_highlight() {
  local text="$1"
  echo -e "\033[0;32m$text\033[0m"
}

remove_package() {
  rm -v ../Assets/Packages/$1
}

text_highlight "\nRemoving Existing DLLs...\n"

remove_package "*.dll"

text_highlight "\nBuilding Solution..."

dotnet build

text_highlight "\nInstalling Dependencies...\n"

cp -v ~/.nuget/packages/google.protobuf/3.23.3/lib/netstandard2.0/Google.Protobuf.dll ../Assets/Packages/Google.Protobuf.dll
cp -v ~/.nuget/packages/mathnet.numerics/4.15.0/lib/netstandard2.0/MathNet.Numerics.dll ../Assets/Packages/MathNet.Numerics.dll
cp -v ~/.nuget/packages/mathnet.spatial/0.6.0/lib/netstandard2.0/MathNet.Spatial.dll ../Assets/Packages/MathNet.Spatial.dll
cp -v ~/.nuget/packages/system.buffers/4.4.0/lib/netstandard2.0/System.Buffers.dll ../Assets/Packages/System.Buffers.dll
cp -v ~/.nuget/packages/system.memory/4.5.3/lib/netstandard2.0/System.Memory.dll ../Assets/Packages/System.Memory.dll
cp -v ~/.nuget/packages/system.runtime.compilerservices.unsafe/6.0.0/lib/netstandard2.0/System.Runtime.CompilerServices.Unsafe.dll ../Assets/Packages/System.Runtime.CompilerServices.Unsafe.dll
cp -v ~/.nuget/packages/ionic.zip/1.9.1.8/lib/Ionic.Zip.dll ../Assets/Packages/Ionic.Zip.dll
cp -v ~/.nuget/packages/portable.bouncycastle/1.9.0/lib/netstandard2.0/BouncyCastle.Crypto.dll ../Assets/Packages/BouncyCastle.Crypto.dll

text_highlight "\n!!! Finished Copying Dependencies !!!\n"
