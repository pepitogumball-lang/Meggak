# ShadeWarpOnMap

Mod para Hollow Knight Mobile que intenta teletransportar al jugador a la posición guardada de la Sombra al abrir el mapa.

## Qué necesita

- `Managed.zip` o carpeta `Managed/` con los DLLs del juego en la raíz del repositorio.
- GitHub Actions para compilar el DLL.

## Flujo de compilación

1. Subir el repositorio con `Managed.zip` en la raíz, o ya descomprimido como `Managed/`.
2. El workflow de GitHub Actions descomprime el ZIP si existe.
3. Compila `src/ShadeWarpOnMap/ShadeWarpOnMap.csproj`.
4. Publica `ShadeWarpOnMap.dll` como artefacto.

## Instalación

Copia `ShadeWarpOnMap.dll` al cargador de mods que use tu APK/modded build.
