🧾 NORMAS DE ORGANIZACIÓN PARA SHADERS, MATERIALES Y TEXTURAS

🎨 SHADERS
- Todos los shaders deben crearse dentro de la carpeta principal destinada a Shaders (sin subcarpetas).
- Utilizar el prefijo 'S_' seguido del nombre representativo del shader.
  Ejemplo: S_TechGlow, S_HexTiles.
- No utilizar subcarpetas para categorizar shaders.

🧱 MATERIALES
- Cada shader debe tener un material asociado.
- Los materiales deben crearse a partir del shader correspondiente y ser renombrados usando el prefijo 'M_'.
  Ejemplo: M_TechGlow, M_HexTiles.
- Una vez creado el material, cortar (Ctrl+X) y pegarlo en la carpeta principal de materiales.
  ❗ No se deben crear subcarpetas dentro de la carpeta de materiales.

🧵 TEXTURAS
- Todas las texturas deben seguir el formato de nombre: 'Text_' + NombreDescriptivo.
  Ejemplo: Text_ConcreteWall, Text_GlowMap.
- Se permiten únicamente dos subcarpetas dentro de la carpeta de Texturas:
  - Futuro
  - Pasado
- Clasificar las texturas según su ambientación temática dentro de estas dos únicas carpetas.

✅ RESUMEN RÁPIDO
| Tipo de Archivo | Prefijo  | Ubicación                        | Subcarpetas Permitidas |
|-----------------|----------|----------------------------------|-------------------------|
| Shader          | S_       | Carpeta principal de Shaders     | ❌ No                  |
| Material        | M_       | Carpeta principal de Materiales  | ❌ No                  |
| Textura         | Text_    | Carpeta Texturas/Futuro o Pasado | ✅ Solo 2              |
