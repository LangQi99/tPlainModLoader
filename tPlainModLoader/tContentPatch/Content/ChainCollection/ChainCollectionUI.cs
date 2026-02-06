using tContentPatch.Content.UI;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace tContentPatch.Content.ChainCollection
{
    public class ChainCollectionUI : UIWindow
    {
        public ChainCollectionUI() : base("连锁采集", 200, 100)
        {
            UISwitch sw = new UISwitch();
            sw.HAlign = 0.5f;
            sw.VAlign = 0.5f;
            sw.OnValUpdate = (v) =>
            {
                ChainCollectionWorld.Enabled = v;
                Main.NewText($"连锁采集: {(v ? "开启" : "关闭")}");
            };
            sw.SetVal(ChainCollectionWorld.Enabled);

            Child.Append(sw);
        }
    }
}
