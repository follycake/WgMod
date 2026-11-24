using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Content.Buffs;
using WgMod.Content.Items.Curses;
using WgMod.Content.TileEntities;

namespace WgMod.Common.Players;

public class CursePlayer : ModPlayer
{
    public bool[] ActiveCurses = new bool[10];

    internal WgStat _stuckageSpeed = new(0.3f, 0.1f);
    internal WgStat _grappleWeaken = new(0.1f, 0.99f);

    internal Vector2 _oldVelocity;

    internal int _absorbantTimer = 0;

    internal int _stormDrainTimer = 0;

    public float _stamina = 100;
    public float _maxStamina = 100;
    internal WgStat _staminaDrain = new(1f, 15f);

    public override void Load()
    {
        On_Player.GetGrapplingForces += GetGrapplingForces;
    }
    public override void Unload()
    {
        On_Player.GetGrapplingForces -= GetGrapplingForces;
    }

    public override void ResetEffects()
    {
        _oldVelocity = Player.velocity;
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (Player.TryGetModPlayer(out BuffHitPlayer plr) && ActiveCurses[(int)CurseIDs.SlimeEater])
        {
            if (plr._slimes.Contains(npc.type))
            {
                TakeBiteOutOfSlime(npc);
            }
        }
    }

    public override void SaveData(TagCompound tag)
    {
        for (int i = 0; i < ActiveCurses.Length; i++)
        {
            tag["ActiveCurse" + i] = ActiveCurses[i];
        }
    }
    public override void LoadData(TagCompound tag)
    {
        for (int i = 0; i < ActiveCurses.Length; i++)
        {
            ActiveCurses[i] = tag.GetBool("ActiveCurse" + i);
        }
    }

    public override void PreUpdateBuffs()
    {
        if (ActiveCurses[(int)CurseIDs.ThirteenDollarStaminaSystem])
            Player.AddBuff(ModContent.BuffType<Stamina>(), 2, true);
    }

    public override void PostUpdateEquips()
    {
        if (ActiveCurses[(int)CurseIDs.SuperSoft])
            SuperSoft();
    }

    public override void PostUpdateMiscEffects()
    {
        if (ActiveCurses[(int)CurseIDs.Absorbant] && Player == Main.LocalPlayer)
            Absorbant();
        if (ActiveCurses[(int)CurseIDs.StormDrain] && Player == Main.LocalPlayer)
            StormDrain();
        if (ActiveCurses[(int)CurseIDs.ThirteenDollarStaminaSystem])
            StaminaUpdate();
    }

    public override void PostUpdateRunSpeeds()
    {
        Stuckage();
    }

    public override void PostUpdate()
    {
        if (ActiveCurses[(int)CurseIDs.WeakPulley])
            if (Player.pulley)
            {
                float weight = Player.Wg().Weight.Mass;

                float downWeigh = weight / 1250;
                if (!Player.controlDown)
                {
                    downWeigh = Math.Max(0, downWeigh - 0.1f);
                }
                if (Player.controlUp)
                {
                    downWeigh = Math.Max(0, downWeigh * 0.67f);
                }
                if (downWeigh > 0f)
                {
                    Player.velocity.Y += downWeigh;
                }
            }
    }

    public void Absorbant()
    {
        _absorbantTimer++;

        if (_absorbantTimer < 6)
            return;
        _absorbantTimer = 0;

        Point16 targetPos = Utils.ToTileCoordinates16(Player.Bottom - new Vector2(0, 2f));

        Tile targetTile = Main.tile[targetPos.X, targetPos.Y];

        if (targetTile.LiquidAmount <= 0)
            return;

        int drankLiquid = targetTile.LiquidType;
        byte liquidLeft = (byte)Math.Max(targetTile.LiquidAmount - 20, 0);
        int totalDrank = 20;
        if (liquidLeft == 0)
            totalDrank -= targetTile.LiquidAmount;

        targetTile.LiquidAmount = liquidLeft;

        TEFeedingTube.FluidInfo fluidInfo = TEFeedingTube.FluidTable[drankLiquid];
        Player.Wg().AddWeight(fluidInfo.Gain * totalDrank / 255f * 20);

        if (liquidLeft == 0)
            targetTile.LiquidType = LiquidID.Water; //i guess this is the default?

        WorldGen.SquareTileFrame(targetPos.X, targetPos.Y, false);

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.sendWater(targetPos.X, targetPos.Y);
        }
        else
        {
            Liquid.AddWater(targetPos.X, targetPos.Y);
        }
    }

    public void StormDrain()
    {
        _stormDrainTimer++;

        if (_stormDrainTimer < 6)
            return;
        _stormDrainTimer = 0;

        if (Main.cloudAlpha > 0 && Player.position.Y > Main.worldSurface)
        {
            Point16 targetPos = Utils.ToTileCoordinates16(Player.Center);
            Tile targetTile = Main.tile[targetPos.X, targetPos.Y];

            if (targetTile.WallType == WallID.None)
            {
                Player.Wg().AddWeight(Main.cloudAlpha * 0.5f);
            }
        }
    }

    public void StaminaUpdate()
    {
        _stamina += 0.05f;

        if ((Player.sitting.isSitting || Player.sleeping.isSleeping) && !(Player.TryGetModPlayer(out TreadmillPlayer tp) && tp._onTreadmill))
            _stamina += 0.1f;

        float lerping = Player.Wg().Weight.GetClampedFactor(Weight.Base, Weight.FromStage(WeightStage.Blob));

        _staminaDrain.Lerp(lerping);

        bool running = false;
        float runningSpeed = 0f;
        if (Player.Grounded() && !Player.mount.Active)
        {
            if (Player.controlRight && Player.velocity.X > 0 || Player.controlLeft && Player.velocity.X < 0)
            {
                running = true;
                runningSpeed = Math.Abs(Player.velocity.X);
            }
        }
        if (running)
        {
            _stamina -= (0.008f + Math.Clamp(runningSpeed / 400, 0, 0.01f)) * _staminaDrain;
        }

        if (Player.Wg().JustJumped)
            _stamina -= 0.5f * _staminaDrain;

        if (Player.ItemAnimationJustStarted)
        {
            _stamina -= 0.07f * _staminaDrain;
        }

        _stamina = Math.Clamp(_stamina, 0, _maxStamina);

        float movementMult = float.Lerp(2f, 0.6f, _stamina / _maxStamina);
        Player.Wg().MovementPenalty *= movementMult;
    }

    public void SuperSoft()
    {
        int extraLife = (int)Math.Floor((Player.Wg().Weight.Mass - 60) / 10f) * 5;
        Player.statLifeMax2 += extraLife;
        Player.Wg().MovementPenalty *= 1.75f;
    }

    public void TakeBiteOutOfSlime(NPC target)
    {
        int stage = Player.Wg().Weight.GetStage();
        NPC.HitInfo info = target.CalculateHitInfo(stage * 10, Player.direction, knockBack: 1, damageVariation: true, luck: Player.luck);
        target.StrikeNPC(info);
        NetMessage.SendStrikeNPC(target, info);

        Player.Wg().AddStomach(info.Damage / 5f);
    }

    public void Stuckage()
    {
        if (!ActiveCurses[(int)CurseIDs.Stuckage])
            return;
        if (Player.Wg().Weight.GetStage() < WeightStage.Heavy)
            return;

        Point16 targetPos = Utils.ToTileCoordinates16(Player.Hitbox.Center.ToVector2());
        if (!TileTypeMatches(targetPos, TileID.OpenDoor))
            return;

        float laaarp = Player.Wg().Weight.GetClampedFactor(Weight.FromStage(WeightStage.Heavy), Weight.FromStage(WeightStage.Immobile));

        _stuckageSpeed.Lerp(laaarp);

        Player.maxRunSpeed *= _stuckageSpeed;
        Player.accRunSpeed *= _stuckageSpeed;
        Player.runSlowdown /= _stuckageSpeed * 3;
    }

    public bool TileTypeMatches(Point16 target, ushort tileID)
    {
        return Main.tile[target.X, target.Y].HasTile && Main.tile[target.X, target.Y].TileType == tileID;
    }

    static void GetGrapplingForces(On_Player.orig_GetGrapplingForces orig, Player self, Vector2 fromPosition, out int? preferredPlayerDirectionToSet, out float preferedPlayerVelocityX, out float preferedPlayerVelocityY)
    {
        if (!self.TryGetModPlayer(out CursePlayer plr))
        {
            orig(self, fromPosition, out preferredPlayerDirectionToSet, out preferedPlayerVelocityX, out preferedPlayerVelocityY);
            return;
        }
        if (!plr.ActiveCurses[(int)CurseIDs.WeakChains])
        {
            orig(self, fromPosition, out preferredPlayerDirectionToSet, out preferedPlayerVelocityX, out preferedPlayerVelocityY);
            return;
        }
        float targetX = 0f;
        float targetY = 0f;
        preferredPlayerDirectionToSet = null;
        int hookCount = 0;
        bool hasAntiGrav = false;
        bool bigEnoughToBeAffected = self.Wg().Weight.GetStage() >= WeightStage.Heavy;
        for (int i = 0; i < self.grapCount; i++)
        {
            Projectile projectile = Main.projectile[self.grappling[i]];
            if (projectile.ai[0] == 2f && !projectile.position.HasNaNs())
            {
                int type = projectile.type;
                bool flag = projectile.ModProjectile != null && projectile.ModProjectile.AIType > ProjectileID.None;
                if (flag)
                {
                    projectile.type = projectile.ModProjectile.AIType;
                }
                targetX += projectile.position.X + (float)(projectile.width / 2);
                targetY += projectile.position.Y + (float)(projectile.height / 2);
                hookCount++;
                if (projectile.type == ProjectileID.AntiGravityHook)
                {
                    hasAntiGrav = true;
                    Vector2 vector = new Vector2((float)(self.controlRight.ToInt() - self.controlLeft.ToInt()), (float)(self.controlDown.ToInt() - self.controlUp.ToInt()) * self.gravDir);
                    if (vector != Vector2.Zero)
                    {
                        vector.Normalize();
                    }
                    vector *= 100f;
                    Vector2 vec = Vector2.Normalize(self.Center - projectile.Center + vector);
                    if (vec.HasNaNs())
                    {
                        vec = -Vector2.UnitY;
                    }
                    float num4 = 200f;
                    targetX += vec.X * num4;
                    targetY += vec.Y * num4;
                }
                else if (projectile.type == ProjectileID.StaticHook)
                {
                    hasAntiGrav = true;
                    Vector2 vector2 = new Vector2((float)(self.controlRight.ToInt() - self.controlLeft.ToInt()), (float)(self.controlDown.ToInt() - self.controlUp.ToInt()) * self.gravDir).SafeNormalize(Vector2.Zero);
                    Vector2 vector3 = projectile.Center - self.Center;
                    Vector2 vector4 = vector3.SafeNormalize(Vector2.Zero);
                    Vector2 value = Vector2.Zero;
                    if (vector2 != Vector2.Zero)
                    {
                        value = vector4 * Vector2.Dot(vector4, vector2);
                    }
                    float num5 = 6f;
                    if (Vector2.Dot(value, vector3) < 0f && vector3.Length() >= 600f)
                    {
                        num5 = 0f;
                    }
                    targetX += 0f - vector3.X + value.X * num5;
                    targetY += 0f - vector3.Y + value.Y * num5;
                }
                else if (projectile.type == ProjectileID.SquirrelHook)
                {
                    Vector2 vector5 = (projectile.rotation - 1.5707964f).ToRotationVector2().SafeNormalize(Vector2.UnitY);
                    Vector2 vector6 = -vector5 * 28f;
                    targetX += vector6.X;
                    targetY += vector6.Y;
                    if (vector5.X != 0f)
                    {
                        preferredPlayerDirectionToSet = new int?(Math.Sign(vector5.X));
                    }
                }
                if (flag)
                {
                    projectile.type = type;
                }
                ProjectileLoader.GrappleTargetPoint(projectile, self, ref targetX, ref targetY);
            }
        }
        if (hookCount == 0)
        {
            preferedPlayerVelocityX = self.velocity.X;
            preferedPlayerVelocityY = self.velocity.Y;
            return;
        }
        float realTargetX = targetX / (float)hookCount;
        float realTargetY = targetY / (float)hookCount;
        preferedPlayerVelocityX = realTargetX - fromPosition.X;
        preferedPlayerVelocityY = realTargetY - fromPosition.Y;
        float num8 = (float)Math.Sqrt((double)(preferedPlayerVelocityX * preferedPlayerVelocityX + preferedPlayerVelocityY * preferedPlayerVelocityY));
        float pullSpeed = 11f;
        if (Main.projectile[self.grappling[0]].type == ProjectileID.BatHook)
        {
            pullSpeed = 14f;
        }
        if (Main.projectile[self.grappling[0]].type == ProjectileID.ThornHook)
        {
            pullSpeed = 12f;
        }
        if (Main.projectile[self.grappling[0]].type >= ProjectileID.LunarHookSolar && Main.projectile[self.grappling[0]].type <= ProjectileID.LunarHookStardust)
        {
            pullSpeed = 16f;
        }
        ProjectileLoader.GrapplePullSpeed(Main.projectile[self.grappling[0]], self, ref pullSpeed);

        float pullPower = 1f;

        float num10 = (num8 <= pullSpeed) ? 1f : (pullSpeed / num8);
        preferedPlayerVelocityX *= num10;
        preferedPlayerVelocityY *= num10;

        if (!hasAntiGrav && bigEnoughToBeAffected)
        {
            float lerp = self.Wg().Weight.GetClampedFactor(Weight.FromStage(WeightStage.Heavy), Weight.FromStage(WeightStage.Blob));

            plr._grappleWeaken.Lerp(lerp);

            pullPower *= Math.Clamp(1 - plr._grappleWeaken / (1 + (float)Math.Log(hookCount) / 10f), 0f, 1f);
        }
        if (pullPower < 1f)
        {
            preferedPlayerVelocityX *= pullPower;
            preferedPlayerVelocityY *= pullPower;

            preferedPlayerVelocityX += plr._oldVelocity.X * (1 - pullPower);
            preferedPlayerVelocityY += plr._oldVelocity.Y * (1 - pullPower) + self.gravity * (1 - pullPower) * self.gravDir;
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket(WgMod.MessageType.CurseSync);
        packet.Write((byte)Player.whoAmI);
        for (int i = 0; i < ActiveCurses.Length; i++)
        {
            packet.Write(ActiveCurses[i]);
        }
        packet.Send(toWho, fromWho);
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        for (int i = 0; i < ActiveCurses.Length; i++)
        {
            ActiveCurses[i] = reader.ReadBoolean();
        }
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        CursePlayer clone = (CursePlayer)targetCopy;
        for (int i = 0; i < ActiveCurses.Length; i++)
        {
            clone.ActiveCurses[i] = ActiveCurses[i];
        }
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        CursePlayer clone = (CursePlayer)clientPlayer;
        bool changed = false;
        for (int i = 0; i < ActiveCurses.Length; i++)
        {
            if (ActiveCurses[i] != clone.ActiveCurses[i])
            {
                changed = true;
                break;
            }
        }
        if (changed)
            SyncPlayer(-1, Main.myPlayer, false);
    }
}
