using System.Collections.Generic;
using Microsoft.Xna.Framework;
using tContentPatch;
using tContentPatch.Content.UI;
using Terraria;
using Terraria.ID;
using Terraria.GameInput;

namespace tContentPatch.Content.ChainCollection
{
    public class ChainCollectionWorld : PatchWorldGen
    {
        public static bool Enabled = false;
        private static bool IsMining = false;

        public override void KillTile(int i, int j, bool fail, bool effectOnly, bool noItem)
        {
            if (!Enabled || IsMining || fail || effectOnly || noItem) return;

            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.HasTile) return;

            IsMining = true;
            try
            {
                int type = tile.TileType;
                Mine(i, j, type);
            }
            finally
            {
                IsMining = false;
            }
        }

        private void Mine(int i, int j, int type)
        {
            int maxBlocks = 500; // Limit to prevent lag
            Queue<Point> q = new Queue<Point>();
            q.Enqueue(new Point(i, j));
            HashSet<Point> visited = new HashSet<Point>();
            visited.Add(new Point(i, j));

            while(q.Count > 0 && maxBlocks > 0)
            {
                Point p = q.Dequeue();
                
                // Skip the starting block as it's already being mined by the game
                if (p.X != i || p.Y != j)
                {
                    // Check if tile is still valid and same type
                    Tile t = Main.tile[p.X, p.Y];
                    if (t != null && t.HasTile && t.TileType == type)
                    {
                        WorldGen.KillTile(p.X, p.Y);
                        maxBlocks--;
                    }
                }

                // Check neighbors
                Point[] dirs = { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };
                foreach(var dir in dirs)
                {
                    Point next = new Point(p.X + dir.X, p.Y + dir.Y);
                    
                    // Bounds check
                    if (next.X < 0 || next.X >= Main.maxTilesX || next.Y < 0 || next.Y >= Main.maxTilesY) continue;

                    if (!visited.Contains(next))
                    {
                        Tile t = Main.tile[next.X, next.Y];
                        if (t != null && t.HasTile && t.TileType == type)
                        {
                            visited.Add(next);
                            q.Enqueue(next);
                        }
                    }
                }
            }
        }
    }

    public class ChainCollectionPlayer : PatchPlayer
    {
        private ChainCollectionUI ui;

        public override void Initialize()
        {
            ui = new ChainCollectionUI();
        }

        public override void UpdatePrefix(Player This, int playerI)
        {
            if (Main.myPlayer != playerI) return;

            // Simple toggle key 'O'
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.O) && Main.oldKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.O))
            {
                if (ui.IsOpen)
                {
                    ui.Close();
                }
                else
                {
                    if (Main.InGameUI.CurrentState != null)
                    {
                         ui.Open(Main.InGameUI.CurrentState);
                    }
                }
            }
        }
    }
}
