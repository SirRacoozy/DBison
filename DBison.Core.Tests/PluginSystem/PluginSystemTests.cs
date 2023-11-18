using DBison.Core.PluginSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DBison.Core.Tests.PluginSystem;

[TestClass]
public class PluginSystemTests
{

    [TestMethod]
    public void TestOfLoadPlugins()
    {
        var path = @"..\..\..\TestData\PluginSystem\";
        var loader = new PluginLoader(path);

        Assert.AreEqual(1, loader.SearchParsingPlugins.Count, "0x00");
        Assert.AreEqual(1, loader.ContextMenuPlugins.Count, "0x01");
        Assert.AreEqual("MyExampleSearchParsingPlugin", loader.SearchParsingPlugins.First().ParseSearchInput(null).Name, "0x02");
        Assert.AreEqual("MyExampleContextMenuPlugin", loader.ContextMenuPlugins.First().Execute(null).Message, "0x03");
    }

}
