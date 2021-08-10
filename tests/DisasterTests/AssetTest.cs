using Disaster;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DisasterTests
{
    [TestClass]
    public class AssetTest
    {
        [TestMethod]
        public void AssetPathTest()
        {
            DisasterTests.Init();

            Disaster.Assets.basePath = "C:/disaster5/";
            Disaster.Assets.LoadPath("test_asset.obj", out string AssetPath);

            Assert.IsTrue(AssetPath == "C:/disaster5/test_asset.obj", "The asset path is not correct");
            
            DisasterTests.Quit();
        }

        // LUNA: Engine should be able to recover from this
        [TestMethod]
        public void LoadNonexistentTexture()
        {
            DisasterTests.Init();
            
            Disaster.Assets.basePath = "C:/disaster5/";

            //Disaster.Assets.Texture("Non-existent.png");
            
            DisasterTests.Quit();
        }
    }
}
