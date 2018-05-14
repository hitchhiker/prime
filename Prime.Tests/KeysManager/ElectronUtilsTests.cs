using Prime.Manager.Utils;
using Xunit;

namespace Prime.Tests.KeysManager
{
    public class ElectronUtilsTests
    {
        [Theory]
        [InlineData("Electron", "/Users/alexander/Projects/GitHub/prime/Prime.KeysManager/bin/Debug/netcoreapp2.0", "/Users/alexander/Projects/GitHub/prime/Prime.KeysManager/Electron")]
        [InlineData("Electron", "/Users/alexander/Projects/GitHub/prime/Prime.KeysManager/bin/Debug", "/Users/alexander/Projects/GitHub/prime/Prime.KeysManager/Electron")]
        [InlineData("Electron", "/Users/alexander/Projects/GitHub/prime/Prime.KeysManager", "/Users/alexander/Projects/GitHub/prime/Prime.KeysManager/Electron")]
        [InlineData("Electron", "/Users/alexander/Projects/GitHub/prime", null)]
        [InlineData("", "/Users/alexander/Projects/GitHub/prime", null)]
        [InlineData("Electron", "", null)]
        public void ShouldFindFolderInCurrentLevel(string folderName, string rootFolderName, string result)
        {
            var path = ElectronUtils.FindElectronUiDirectory(folderName, rootFolderName);
            
            Assert.Equal(path, result);
        }
    }
}