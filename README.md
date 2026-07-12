# ShadeWarpOnMap

Mod para Hollow Knight Mobile que intenta teletransportar al jugador a la posición guardada de la Sombra al abrir el mapa.

## Qué necesita

- `Managed.zip` en la raíz del repositorio, o la carpeta `Managed/` ya descomprimida.
- Dentro de `Managed/` deben estar `Assembly-CSharp.dll`, `MMHOOK_Assembly-CSharp.dll`, `UnityEngine.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.Physics2DModule.dll` y demás DLLs del juego.
- GitHub Actions para compilar el DLL.

## Flujo de compilación

1. Subir el repositorio con `Managed.zip` en la raíz, o ya descomprimido como `Managed/`.
2. El workflow descomprime el ZIP si existe.
3. Si los DLL quedaron en otra carpeta, el workflow los busca y los normaliza a `Managed/`.
4. Compila `src/ShadeWarpOnMap/ShadeWarpOnMap.csproj`.
5. Publica `ShadeWarpOnMap.dll` como artefacto.

## Instalación

Copia `ShadeWarpOnMap.dll` al cargador de mods que use tu APK/modded build.
