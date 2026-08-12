using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace WgMod.Common;

public class Shaders : ILoadable
{
    public static Asset<Effect> FatArmor { get; private set; }
    public static Asset<Effect> FatArmorSoften { get; private set; }
    public static Asset<Effect> Stone { get; private set; }
    static Asset<Texture2D> _stonePattern;

    public void Load(Mod mod)
    {
        if (Main.dedServ)
            return;
        FatArmor = mod.Assets.Request<Effect>("Assets/Effects/FatArmor");
        FatArmorSoften = mod.Assets.Request<Effect>("Assets/Effects/FatArmorSoften");
        Stone = mod.Assets.Request<Effect>("Assets/Effects/Stone");
        _stonePattern = mod.Assets.Request<Texture2D>("Assets/Textures/StonePattern");
    }

    public void Unload()
    {
    }

    public static void ApplyStone(Camera camera, float opacity = 0.5f)
    {
        Stone.Value.CurrentTechnique.Passes[0].Apply();
        camera.SpriteBatch.GraphicsDevice.Textures[1] = _stonePattern.Value;
        Stone.Value.Parameters["uOpacity"].SetValue(opacity);
        Stone.Value.Parameters["uImageSize1"].SetValue(_stonePattern.Size());
        Stone.Value.Parameters["uMatrix"].SetValue(camera.GameViewMatrix.NormalizedTransformationmatrix);
    }
}
