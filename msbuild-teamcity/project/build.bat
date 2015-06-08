msbuild /p:Platform="x86" build.msbuild /p:Configuration="Release"
msbuild /p:Platform="x86" UnitTests.msbuild /p:Configuration="Release"
msbuild /p:Platform="x86" IntegrityCheckCreator.msbuild /p:Configuration="Release"