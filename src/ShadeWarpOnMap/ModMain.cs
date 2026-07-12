using System;
using System.Collections;
using System.Reflection;
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
            return "1.0.1";
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

                // En la versión móvil, los métodos ResetMotion, CancelAttack, etc. son privados.
                // Invocarlos por reflexión para no causar errores de compilación, o ignorarlos si fallan.
                InvokePrivateMethod(hero, "ResetMotion");
                InvokePrivateMethod(hero, "CancelAttack");
                InvokePrivateMethod(hero, "CancelDash");
                InvokePrivateMethod(hero, "CancelRecoilHorizontal");
                InvokePrivateMethod(hero, "CancelDamageRecoil");
                
                // RegainControl sí suele ser público en PC, pero por si acaso lo invocamos por reflexión
                // o comprobamos si es público. Como sabemos que en móvil es privado, usamos reflexión.
                InvokePrivateMethod(hero, "RegainControl");
                InvokePrivateMethod(hero, "ResetLook");

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

        private static void InvokePrivateMethod(object target, string methodName)
        {
            if (target == null) return;
            
            try
            {
                MethodInfo method = target.GetType().GetMethod(
                    methodName, 
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
                if (method != null)
                {
                    method.Invoke(target, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[ShadeWarpOnMap] Could not invoke method " + methodName + ": " + ex.Message);
            }
        }
    }
}
