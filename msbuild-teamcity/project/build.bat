msbuild /p:Platform="x86" build.msbuild /p:Configuration="Release"
msbuild /p:Platform="x86" UnitTests.msbuild /p:Configuration="Release"
msbuild /p:Platform="x86" IntegrityCheckCreator.msbuild /p:Configuration="Release"
msbuild /p:Platform="x86" PrepareSetup.msbuild /p:Configuration="Release"
msbuild /p:Platform="x86" BuildSetup.msbuild /p:Configuration="Release"