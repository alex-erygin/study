msbuild /p:Platform="Any CPU" build.msbuild /p:Configuration="Release"
msbuild /p:Platform="Any CPU" UnitTests.msbuild /p:Configuration="Release"
msbuild /p:Platform="Any CPU" IntegrityCheckCreator.msbuild /p:Configuration="Release"