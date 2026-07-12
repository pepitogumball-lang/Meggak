using System;
using System.Collections;
using Modding;
using UnityEngine;

namespace ShadeWarpOnMap
{
    public sealed class ShadeWarpOnMap : Mod
    {
        private static bool _hooksInstalled;
        private static bool _warpQueued;

        public override string GetVersion()
        {
            return "1.0.0";
        }

        public override void Initialize()
        {
            if (_hooksInstalled)
            {
                return;
            }

            _hooksInstalled = true;
            On.GameMap.OnEnable += GameMap_OnEnable;
            On.GameMap.OnDisable += GameMap_OnDisable;

            Debug.Log("[ShadeWarpOnMap] Loaded.");
        }

        private void GameMap_OnEnable(On.GameMap.orig_OnEnable orig, GameMap self)
        {
            orig(self);

            if (self == null || _warpQueued)
            {
                return;
            }

            _warpQueued = true;
            self.StartCoroutine(WarpNextFrame());
        }

        private void GameMap_OnDisable(On.GameMap.orig_OnDisable orig, GameMap self)
        {
            orig(self);
            _warpQueued = false;
        }

        private static IEnumerator WarpNextFrame()
        {
            yield return null;
            TryWarpToShade();
        }

        private static void TryWarpToShade()
        {
            try
            {
                PlayerData pd = PlayerData.instance;
                HeroController hero = HeroController.instance;

                if (pd == null || hero == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(pd.shadeScene))
                {
                    return;
                }

                Vector3 target = new Vector3(pd.shadePositionX, pd.shadePositionY, hero.transform.position.z);

                Rigidbody2D body = hero.GetComponent<Rigidbody2D>();
                if (body != null)
                {
                    body.position = target;
                    body.velocity = Vector2.zero;
                    body.angularVelocity = 0f;
                }

                hero.transform.position = target;

                hero.ResetMotion();
                hero.CancelAttack();
                hero.CancelDash();
                hero.CancelRecoilHorizontal();
                hero.CancelDamageRecoil();
                hero.RegainControl();
                hero.ResetLook();

                Debug.Log(string.Format(
                    "[ShadeWarpOnMap] Warped to shade at ({0:0.00}, {1:0.00}) in scene '{2}'.",
                    pd.shadePositionX,
                    pd.shadePositionY,
                    pd.shadeScene));
            }
            catch (Exception ex)
            {
                Debug.LogError("[ShadeWarpOnMap] Failed to warp to shade: " + ex);
            }
        }
    }
}
