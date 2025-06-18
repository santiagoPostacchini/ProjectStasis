using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DualSurfaceShaderEditor : ShaderGUI
{
    public bool uniform;
    public bool uniform2;
    public bool uniform3;
    public bool uniform4;
    public bool uniform5;
    public bool uniform6;

    public int copy1;
    public int copy2;
    public int colcopy1;
    public int colcopy2;
    public bool clampshow;
    public bool boxedit;
    public bool gradedit;
    public bool once;
    public bool onceagain;
    public bool place;
    public Vector3 mousePosition;
    public Vector3 tryplace;
    public float savedoffsetx;
    public float savedoffsety;

    Color line = new Color(0.5f, 0.5f, 0.5f, 1);
    Color darkline = new Color(0.1f, 0.1f, 0.1f, 1);
    public int active = -1;

    Object target;

    public SceneEdit pls;

    private static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(35), GUILayout.Height(35));
        GUILayout.EndVertical();
        return result;
    }

    public override void OnClosed(Material material)
    {
        Debug.Log("WEWEWEWE");
    }

    public static void DrawUILine(Color color, int thickness = 1, int padding = 10, int width = 2)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= width / 2;
        r.width += width * 2;
        EditorGUI.DrawRect(r, color);
    }

    Vector3 scale(bool uni, Vector3 start, bool noclamp = false)
    {
        Vector3 end;
        Undo.RecordObject(target, "Changed Value");
        end = EditorGUILayout.Vector3Field("", start);
        if(!noclamp)
        {
            end.x = Mathf.Clamp(end.x, 0.0001f, end.x);
            end.y = Mathf.Clamp(end.y, 0.0001f, end.y);
            end.z = Mathf.Clamp(end.z, 0.0001f, end.z);
        }
        if (uni)
        {
            float wow = Mathf.Max(end.x / start.x, Mathf.Max(end.y / start.y, end.z / start.z));
            if (wow == 1)
                wow = Mathf.Min(end.x / start.x, Mathf.Min(end.y / start.y, end.z / start.z));
            if (!float.IsNaN(wow))
            {
                end = start * (wow);
            }
            else
            {
                wow = Mathf.Max(end.x, Mathf.Max(end.y, end.z));
                if (wow == 0)
                    wow = Mathf.Min(end.x, Mathf.Max(end.y, end.z));
                end = Vector3.one * wow;
            }
                

            

            
            if (float.IsNaN(Vector3.Magnitude(end)))
                end = Vector3.zero;
        }
        return end;
    }


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        target = materialEditor.target;
        if (!pls && Selection.activeGameObject != null)
        {
            pls = ScriptableObject.CreateInstance<SceneEdit>();
            pls.DSSE = this;
            pls.Tar = target;
        }

        Material T = materialEditor.target as Material;
        var header = new GUIStyle(GUI.skin.label);
        header.alignment = TextAnchor.UpperCenter;
        header.fontSize = 15;
        var Hheader = new GUIStyle(GUI.skin.label);
        Hheader.alignment = TextAnchor.UpperCenter;
        Hheader.fontSize = 20;
        var sideb = new GUIStyle(GUI.skin.label);
        sideb.alignment = TextAnchor.UpperRight;

        bool norm = T.HasTexture("_BumpMap");
        bool dual = T.HasTexture("_UnderTex");

        float setsnap(int type, float inp, bool left = true)
        {
            


            float x = 0;
            switch (type)
            {
                case 0:
                    x = inp;
                    break;
                case 1:
                    x = T.GetFloat("_MinNoise");
                    break;
                case 2:
                    if (left)
                        x = Mathf.Clamp01(T.GetFloat("_MinNoise") - savedoffsetx);
                    else
                        x = Mathf.Clamp01(T.GetFloat("_MinNoise") + savedoffsety);
                    break;
                case 3:
                    x = T.GetFloat("_MaxNoise");
                    break;
                case 4:
                    if (left)
                        x = Mathf.Clamp01(T.GetFloat("_MaxNoise") - savedoffsetx);
                    else
                        x = Mathf.Clamp01(T.GetFloat("_MaxNoise") + savedoffsety);
                    break;
                case 5:
                    x = T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth");
                    break;
                case 6:
                    if (left)
                        x = Mathf.Clamp01(T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth") - savedoffsetx);
                    else
                        x = Mathf.Clamp01(T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth") + savedoffsety);
                    break;
                case 7:
                    x = T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth");
                    break;
                case 8:
                    if (left)
                        x = Mathf.Clamp01(T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth") - savedoffsetx);
                    else
                        x = Mathf.Clamp01(T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth") + savedoffsety);
                    break;
                
            }



            return x;
        }






        GUILayout.Label("Main Textures", Hheader);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Top Texture:", GUILayout.Width(120));
        Undo.RecordObject(target, "Changed Texture");
        T.SetTexture("_MainTex", TextureField("Albedo", (Texture2D)T.GetTexture("_MainTex")));
        if(norm)
        {
            Undo.RecordObject(target, "Changed Texture");
            T.SetTexture("_BumpMap", TextureField("Normal", (Texture2D)T.GetTexture("_BumpMap")));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Offset:");
        GUILayout.Label("      ");
        Vector3 st = T.GetVector("_MainOffsetFactor");
        Vector3 e;
        Undo.RecordObject(target, "Changed Value");
        e = EditorGUILayout.Vector3Field("", st);
        T.SetVector("_MainOffsetFactor", e);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Scale:");
        uniform = GUILayout.Toggle(uniform, "U");
        Undo.RecordObject(target, "Changed Value");
        T.SetVector("_MainScaleFactor", scale(uniform, T.GetVector("_MainScaleFactor")));
        EditorGUILayout.EndHorizontal();

        if(dual)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Bottom Texture:", GUILayout.Width(120));
            Undo.RecordObject(target, "Changed Texture");
            T.SetTexture("_UnderTex", TextureField("Albedo", (Texture2D)T.GetTexture("_UnderTex")));
            if (norm)
            {
                Undo.RecordObject(target, "Changed Texture");
                T.SetTexture("_BumpMapUnder", TextureField("Normal", (Texture2D)T.GetTexture("_BumpMapUnder")));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Offset:");
            GUILayout.Label("      ");
            Vector3 st2 = T.GetVector("_SecOffsetFactor");
            Vector3 e2;
            Undo.RecordObject(target, "Changed Value");
            e2 = EditorGUILayout.Vector3Field("", st2);
            T.SetVector("_SecOffsetFactor", e2);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scale:");
            uniform2 = GUILayout.Toggle(uniform2, "U");
            Undo.RecordObject(target, "Changed Value");
            T.SetVector("_SecScaleFactor", scale(uniform2, T.GetVector("_SecScaleFactor")));
            EditorGUILayout.EndHorizontal();
        }
        

        if(T.HasFloat("_Blending"))
        {
            GUILayout.Label("Triplanar Blend           ", header);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Sharp");
            GUILayout.Label("Mixed                 ", sideb);
            EditorGUILayout.EndHorizontal();
            Undo.RecordObject(target, "Changed Value");
            T.SetFloat("_Blending", EditorGUILayout.Slider(T.GetFloat("_Blending"), 0, 1));
        }
        




        DrawUILine(darkline, 1, 20, 1000);


        GUILayout.Label("Interpolator Textures", Hheader);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("First Interpolator:", GUILayout.Width(120));
        Undo.RecordObject(target, "Changed Texture");
        T.SetTexture("_InterpBump", TextureField("Heightmap", (Texture2D)T.GetTexture("_InterpBump")));
        if(norm)
        {
            Undo.RecordObject(target, "Changed Texture");
            T.SetTexture("_InterpNormal", TextureField("Normal", (Texture2D)T.GetTexture("_InterpNormal")));
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Offset:");
        GUILayout.Label("      ");
        Vector3 st3 = T.GetVector("_IntOffsetFactor");
        Vector3 e3;
        Undo.RecordObject(target, "Changed Value");
        e3 = EditorGUILayout.Vector3Field("", st3);
        T.SetVector("_IntOffsetFactor", e3);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Scale:");
        uniform3 = GUILayout.Toggle(uniform3, "U");
        Undo.RecordObject(target, "Changed Value");
        T.SetVector("_IntScaleFactor", scale(uniform3, T.GetVector("_IntScaleFactor")));
        EditorGUILayout.EndHorizontal();
        Vector3 sss = T.GetVector("_Sin");
        sss = new Vector3(Mathf.Rad2Deg * Mathf.Asin(sss.x), Mathf.Rad2Deg * Mathf.Asin(sss.y), Mathf.Rad2Deg * Mathf.Asin(sss.z));
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Rotation:");
        GUILayout.Label("  ");
        Undo.RecordObject(target, "Changed Value");
        sss = EditorGUILayout.Vector3Field("", sss);
        EditorGUILayout.EndHorizontal();
        Vector3 ssin = new Vector3(Mathf.Sin(Mathf.Deg2Rad * sss.x), Mathf.Sin(Mathf.Deg2Rad * sss.y), Mathf.Sin(Mathf.Deg2Rad * sss.z));
        Vector3 ccos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * sss.x), Mathf.Cos(Mathf.Deg2Rad * sss.y), Mathf.Cos(Mathf.Deg2Rad * sss.z));
        T.SetVector("_Sin", ssin);
        T.SetVector("_Cos", ccos);
        if(dual)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Second Interpolator:", GUILayout.Width(120));
            Undo.RecordObject(target, "Changed Texture");
            T.SetTexture("_SecInterpBump", TextureField("Heightmap", (Texture2D)T.GetTexture("_SecInterpBump")));
            if (norm)
            {
                Undo.RecordObject(target, "Changed Texture");
                T.SetTexture("_SecInterpNormal", TextureField("Normal", (Texture2D)T.GetTexture("_SecInterpNormal")));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Offset:");
            GUILayout.Label("      ");
            Vector3 st4 = T.GetVector("_SecIntOffsetFactor");
            Vector3 e4;
            Undo.RecordObject(target, "Changed Value");
            e4 = EditorGUILayout.Vector3Field("", st4);
            T.SetVector("_SecIntOffsetFactor", e4);
            EditorGUILayout.EndHorizontal();



            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scale:");
            uniform4 = GUILayout.Toggle(uniform4, "U");
            Undo.RecordObject(target, "Changed Value");
            T.SetVector("_SecIntScaleFactor", scale(uniform4, T.GetVector("_SecIntScaleFactor")));
            EditorGUILayout.EndHorizontal();

            sss = T.GetVector("_SecSin");
            sss = new Vector3(Mathf.Rad2Deg * Mathf.Asin(sss.x), Mathf.Rad2Deg * Mathf.Asin(sss.y), Mathf.Rad2Deg * Mathf.Asin(sss.z));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Rotation:");
            GUILayout.Label("  ");
            Undo.RecordObject(target, "Changed Value");
            sss = EditorGUILayout.Vector3Field("", sss);
            EditorGUILayout.EndHorizontal();
            ssin = new Vector3(Mathf.Sin(Mathf.Deg2Rad * sss.x), Mathf.Sin(Mathf.Deg2Rad * sss.y), Mathf.Sin(Mathf.Deg2Rad * sss.z));
            ccos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * sss.x), Mathf.Cos(Mathf.Deg2Rad * sss.y), Mathf.Cos(Mathf.Deg2Rad * sss.z));
            T.SetVector("_SecSin", ssin);
            T.SetVector("_SecCos", ccos);




            GUILayout.Label("Interpolator Blend           ", header);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Int 1");
            GUILayout.Label("Int 2                 ", sideb);
            EditorGUILayout.EndHorizontal();
            Undo.RecordObject(target, "Changed Value");
            T.SetFloat("_IntBlend", EditorGUILayout.Slider(T.GetFloat("_IntBlend"), 0, 1));
        }
        


        GUILayout.Label("Interpolator Thresholds", header);
        float s = T.GetFloat("_MinNoise");
        float f = T.GetFloat("_MaxNoise");
        Undo.RecordObject(target, "Changed Values");
        EditorGUILayout.MinMaxSlider(ref s, ref f, 0.01f, 0.99f);
        T.SetFloat("_MinNoise", Mathf.Clamp(s, 0.01f, f - 0.001f));
        T.SetFloat("_MaxNoise", Mathf.Clamp(f, s + 0.001f, 0.99f));

        if(norm)
        {
            GUILayout.Label("Interpolator Strength           ", header);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("-1");
            GUILayout.Label("1                 ", sideb);
            EditorGUILayout.EndHorizontal();
            Undo.RecordObject(target, "Changed Value");
            T.SetFloat("_NoiseBumpStrength", EditorGUILayout.Slider(T.GetFloat("_NoiseBumpStrength"), -1, 1));
        }

        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_IntPosInf", System.Convert.ToSingle(GUILayout.Toggle(System.Convert.ToBoolean(T.GetFloat("_IntPosInf")), "Use Interpolator Clamp")));

        clampshow = GUILayout.Toggle(clampshow, "Hide Interpolator Clamp");

        if (T.GetFloat("_IntPosInf") == 1)
        {
            GUIStyle bigbutt = new GUIStyle(GUI.skin.button);
            bigbutt.fontSize = 15;

            if (GUILayout.Button(place == false ? "Pick Interpolator Clamp Origin" : "Click On World To Place",bigbutt, GUILayout.Height(30)))
            {
                place = !place;
                boxedit = false;
                gradedit = false;
            }
            if (GUILayout.Button(boxedit == false? "Edit Interpolator Clamp Boundaries" : "Click To Exit Edit", bigbutt, GUILayout.Height(50)))
            {
                boxedit = !boxedit;
                place = false;
                gradedit = false;
                onceagain = true;
                if (boxedit)
                    active = -1;
            }
            if (GUILayout.Button(gradedit == false ? "Edit Interpolator Gradient Boundaries" : "Click To Exit Edit", bigbutt, GUILayout.Height(30)))
            {
                gradedit = !gradedit;
                boxedit = false;
                place = false;
                onceagain = true;
                if (gradedit)
                    active = -1;
            }


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Pos:         ");
            GUILayout.Label("      ");
            Undo.RecordObject(target, "Changed Value");
            T.SetVector("_IntClampPos", EditorGUILayout.Vector3Field("", T.GetVector("_IntClampPos")));
            Undo.RecordObject(target, "Changed Value");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Width:     ");
            uniform6 = GUILayout.Toggle(uniform6, "U");
            Vector3 mhm = scale(uniform6, T.GetVector("_IntClampWidth"), true);
            mhm = new Vector3(Mathf.Clamp(mhm.x, 0, mhm.x), Mathf.Clamp(mhm.y, 0, mhm.y), Mathf.Clamp(mhm.z, 0, mhm.z));
            Undo.RecordObject(target, "Changed Value");
            T.SetVector("_IntClampWidth", mhm);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Gradient:");
            uniform5 = GUILayout.Toggle(uniform5, "U");
            Undo.RecordObject(target, "Changed Value");
            T.SetVector("_IntPosGrad", scale(uniform5, T.GetVector("_IntPosGrad"), true));
            EditorGUILayout.EndHorizontal();


        }

        

        



        DrawUILine(darkline, 1, 20, 1000);

        GUILayout.Label("Colour", Hheader);
        Undo.RecordObject(target, "Changed Value");
        T.SetColor("_MainColor", EditorGUILayout.ColorField("Primary Top Color", T.GetColor("_MainColor")));
        Undo.RecordObject(target, "Changed Value");
        T.SetColor("_SecColor", EditorGUILayout.ColorField("Primary Bottom Color", T.GetColor("_SecColor")));

        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_CChangeHeight", EditorGUILayout.FloatField("Top and Bottom Cutoff Height", T.GetFloat("_CChangeHeight")));
        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_CChangeGrad", EditorGUILayout.FloatField("Top and Bottom Gradient", T.GetFloat("_CChangeGrad")));

        s = T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth");
        f = T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth");
        GUILayout.Label("Primary Colour Thresholds", header);
        Undo.RecordObject(target, "Changed Values");
        EditorGUILayout.MinMaxSlider(ref s, ref f, 0, 1);

        float qs = setsnap(colcopy1,s);
        float qf = setsnap(colcopy2, f, false);

        if (qs > qf)
        {
            int sa = copy1;
            copy1 = copy2;
            copy2 = sa;
            float ssa = qs;
            qs = qf;
            qf = ssa;
            ssa = savedoffsetx;
            savedoffsetx = -savedoffsety;
            savedoffsety = -ssa;

        }


        s = qs;
        f = qf;

        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_CChangePenPos", (s + f) / 2);
        T.SetFloat("_CChangePenWidth", (f - s) / 2);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("First Threshold Snap Type:");
        float old = colcopy1;
        colcopy1 = EditorGUILayout.Popup(colcopy1, new string[] { "Free", "Copy First Interpolator Threshold", "Follow First Interpolator Threshold", "Copy Last Interpolator Threshold", "Follow Last Interpolator Threshold"});
        if(old != colcopy1)
        {
            if(colcopy1 == 2)
            {
                savedoffsetx = T.GetFloat("_MinNoise") - (T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth"));
            }
            if (colcopy1 == 4)
            {
                savedoffsetx = T.GetFloat("_MaxNoise") - (T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth"));
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Last Threshold Snap Type:");
        old = colcopy2;
        colcopy2 = EditorGUILayout.Popup(colcopy2, new string[] { "Free", "Copy First Interpolator Threshold", "Follow First Interpolator Threshold", "Copy Last Interpolator Threshold", "Follow Last Interpolator Threshold" });
        if (old != colcopy2)
        {
            if (colcopy2 == 2)
            {
                savedoffsety = T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth") - T.GetFloat("_MinNoise");
            }
            if (colcopy2 == 4)
            {
                savedoffsety = T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth") - T.GetFloat("_MaxNoise");
            }
        }

        EditorGUILayout.EndHorizontal();

        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_CChangePenGrad", EditorGUILayout.Slider("Threshold Gradient", T.GetFloat("_CChangePenGrad"), 0, 1));
        Undo.RecordObject(target, "Changed Value");
        T.SetColor("_CChangeAmbCol", EditorGUILayout.ColorField("Secondary Color", T.GetColor("_CChangeAmbCol")));

        DrawUILine(darkline, 1, 20, 1000);

        GUILayout.Label("Smoothness and Metallic", Hheader);

        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_Glossiness", EditorGUILayout.Slider("Smoothness", T.GetFloat("_Glossiness"), 0, 1));
        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_Metallic", EditorGUILayout.Slider("Metallic", T.GetFloat("_Metallic"), 0, 1));
        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_SecGlossiness", EditorGUILayout.Slider("Secondary Smooth",T.GetFloat("_SecGlossiness"), 0, 1));
        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_SecMetallic", EditorGUILayout.Slider("Secondary Metal", T.GetFloat("_SecMetallic"), 0, 1));
        
        s = T.GetFloat("_SmoothStart") - T.GetFloat("_SmoothEnd");
        f = T.GetFloat("_SmoothStart") + T.GetFloat("_SmoothEnd");
        GUILayout.Label("Primary Smooth Thresholds", header);
        Undo.RecordObject(target, "Changed Values");
        EditorGUILayout.MinMaxSlider(ref s, ref f, 0, 1);
        s = Mathf.Clamp(s, 0, f - 0.001f);
        f = Mathf.Clamp(f, s + 0.001f, 1);


        qs = setsnap(copy1, s);
        qf = setsnap(copy2, f, false);
        


        if (qs > qf | (copy1 == 3 && copy2 == 1) | (copy1 == 7 && copy2 == 5))
        {
            

            int sa = copy1;
            copy1 = copy2;
            copy2 = sa;
            float ssa = qs;
            qs = qf;
            qf = ssa;
            ssa = savedoffsetx;
            savedoffsetx = -savedoffsety;
            savedoffsety = -ssa;

        }

        s = qs;
        f = qf;

        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_SmoothStart", (s + f) / 2);
        T.SetFloat("_SmoothEnd", (f - s) / 2);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("First Threshold Snap Type:");
        old = copy1;
        copy1 = EditorGUILayout.Popup(copy1, new string[] { "Free", "Copy First Interpolator Threshold", "Follow First Interpolator Threshold", "Copy Last Interpolator Threshold", "Follow Last Interpolator Threshold", "Copy First Color Threshold", "Follow First Color Threshold", "Copy Last Color Threshold", "Follow Last Color Threshold" });
        if (old != copy1)
        {
            if (copy1 == 2)
            {
                savedoffsetx = T.GetFloat("_MinNoise") - (T.GetFloat("_SmoothStart") - T.GetFloat("_SmoothEnd"));
            }
            if (copy1 == 4)
            {
                savedoffsetx = T.GetFloat("_MaxNoise") - (T.GetFloat("_SmoothStart") - T.GetFloat("_SmoothEnd"));
            }

                        
            if(copy1 == 6)
            {
                savedoffsetx = (T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth")) - (T.GetFloat("_SmoothStart") - T.GetFloat("_SmoothEnd"));
            }
            if (copy1 == 8)
            {
                savedoffsetx = (T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth")) - (T.GetFloat("_SmoothStart") - T.GetFloat("_SmoothEnd"));
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Last Threshold Snap Type:");
        old = copy2;
        copy2 = EditorGUILayout.Popup(copy2, new string[] { "Free", "Copy First Interpolator Threshold", "Follow First Interpolator Threshold", "Copy Last Interpolator Threshold", "Follow Last Interpolator Threshold", "Copy First Color Threshold", "Follow First Color Threshold", "Copy Last Color Threshold", "Follow Last Color Threshold" });
        if (old != copy2)
        {
            if (copy2 == 2)
            {
                savedoffsety = T.GetFloat("_SmoothStart") + T.GetFloat("_SmoothEnd") - T.GetFloat("_MinNoise");
            }
            if (copy2 == 4)
            {
                savedoffsety = T.GetFloat("_SmoothStart") + T.GetFloat("_SmoothEnd") - T.GetFloat("_MaxNoise");
            }


            
            if (copy2 == 6)
            {
                savedoffsety = T.GetFloat("_SmoothStart") + T.GetFloat("_SmoothEnd") - (T.GetFloat("_CChangePenPos") - T.GetFloat("_CChangePenWidth"));
            }
            if (copy2 == 8)
            {
                savedoffsety = T.GetFloat("_SmoothStart") + T.GetFloat("_SmoothEnd") - (T.GetFloat("_CChangePenPos") + T.GetFloat("_CChangePenWidth"));
            }
        }
        EditorGUILayout.EndHorizontal();
        Undo.RecordObject(target, "Changed Value");
        T.SetFloat("_SmoothSmooth", EditorGUILayout.Slider("Threshold Gradient", T.GetFloat("_SmoothSmooth"), 0, 1));

        DrawUILine(darkline, 1, 20, 1000);
        T.enableInstancing = GUILayout.Toggle(T.enableInstancing, "Enable GPU Instancing");

    }

    
}


[CustomEditor(typeof(DualSurfaceShaderEditor))]
[CanEditMultipleObjects]
public class SceneEdit : Editor
{
    public DualSurfaceShaderEditor DSSE;
    public Object Tar;
    public Object check;
    private void OnEnable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }


    public void OnSceneGUI(SceneView sceneView)
    {
        if (!Tar)
            return;

        if (Selection.activeGameObject == null | Selection.activeGameObject != check)
        {
            if (!(check == null && Selection.activeGameObject != null))
            {
                Tar = null;
                SceneView.duringSceneGui -= OnSceneGUI;
                ScriptableObject.DestroyImmediate(DSSE.pls);
                return;
            }
        }


        check = Selection.activeObject;



        if (EditorWindow.mouseOverWindow != null && DSSE.boxedit && DSSE.onceagain)
        {
            if (EditorWindow.mouseOverWindow.ToString() != " (UnityEditor.SceneView)")
            {
                HandleUtility.Repaint();
                DSSE.onceagain = false;
            }
        }

        var t = Tar as Material;

        if (t.GetFloat("_IntPosInf") == 1 && !DSSE.clampshow)
        {
            Vector3 pos = t.GetVector("_IntClampPos");
            Vector3 wid = t.GetVector("_IntClampWidth");
            Vector3 grad = t.GetVector("_IntPosGrad");

            Handles.color = Color.green;
            Handles.DrawWireCube(pos, wid * 2);
            if (grad != Vector3.zero | DSSE.gradedit)
                Handles.color = Color.blue;
            Handles.DrawWireCube(pos, wid * 2 + grad * 2);
            Handles.color = Color.green;
            var e = Event.current;
            if (EditorWindow.mouseOverWindow == null | e.alt)
                return;




            if (e.keyCode == KeyCode.Escape)
            {
                DSSE.place = false;
                DSSE.boxedit = false;
                DSSE.gradedit = false;
            }


            if (DSSE.place)
            {
                int id = GUIUtility.GetControlID(FocusType.Passive);
                HandleUtility.AddDefaultControl(id);
                Handles.DrawLine(DSSE.tryplace, pos, 0.1f);
                if (Camera.current != null)
                    Handles.DrawSolidDisc(DSSE.tryplace, Camera.current.transform.forward, 0.1f);

                RaycastHit hit;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);




                if (Physics.Raycast(ray.origin, ray.direction, hitInfo: out hit, 2000))
                {
                    if (e.type == EventType.MouseMove)
                    {
                        HandleUtility.Repaint();
                        DSSE.tryplace = hit.point;
                    }


                    if (e.type == EventType.MouseDown)
                    {

                        pos = hit.point;
                        Undo.RecordObject(Tar, "Changed Values");
                        t.SetVector("_IntClampPos", pos);
                        DSSE.place = false;

                    }
                }


            }



            if (DSSE.boxedit | DSSE.gradedit)
            {
                Vector3 wwid = wid;
                if (DSSE.gradedit)
                    wwid = wwid + grad;
                List<float> dir = new List<float>();
                if (DSSE.gradedit)
                {
                    dir.Add(Mathf.Sign(wid.y + grad.y));
                    dir.Add(Mathf.Sign(wid.y + grad.y));
                    dir.Add(Mathf.Sign(wid.x + grad.x));
                    dir.Add(Mathf.Sign(wid.x + grad.x));
                    dir.Add(Mathf.Sign(wid.z + grad.z));
                    dir.Add(Mathf.Sign(wid.z + grad.z));
                }

                //wwid = new Vector3(Mathf.Abs(wwid.x), Mathf.Abs(wwid.y), Mathf.Abs(wwid.z));

                SceneView cam = EditorWindow.GetWindow<SceneView>();
                DSSE.mousePosition = Event.current.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                DSSE.mousePosition.y = cam.camera.pixelHeight - DSSE.mousePosition.y * ppp;
                DSSE.mousePosition.x *= ppp;
                float dist = 10000;
                int currentclosest = 0;
                float distCheck = 15;
                int id = GUIUtility.GetControlID(FocusType.Passive);
                HandleUtility.AddDefaultControl(id);
                List<Color> cols = new List<Color>();
                List<Vector3> points = new List<Vector3>();
                points.Add(pos + Vector3.up * wwid.y);
                cols.Add(Color.yellow);
                points.Add(pos - Vector3.up * wwid.y);
                cols.Add(Color.yellow);
                points.Add(pos + Vector3.right * wwid.x);
                cols.Add(Color.red);
                points.Add(pos - Vector3.right * wwid.x);
                cols.Add(Color.red);
                points.Add(pos + Vector3.forward * wwid.z);
                cols.Add(Color.blue);
                points.Add(pos - Vector3.forward * wwid.z);
                cols.Add(Color.blue);
                points.Add(pos);


                float ugh = Mathf.Min(Mathf.Abs(wwid.x), Mathf.Min(Mathf.Abs(wwid.y), Mathf.Abs(wwid.z)));
                bool odd = false;

                for (int i = 0; i < points.Count - 1; i++)
                {

                    Handles.color = cols[i];
                    if (i != DSSE.active)
                        Handles.DrawWireDisc(points[i], (points[i] - pos).normalized, Mathf.Min(0.3f, ugh));
                    else
                    {
                        Handles.DrawSolidDisc(points[i], (points[i] - pos).normalized, Mathf.Min(0.3f, ugh));
                        if (!e.shift)
                        {
                            if (DSSE.boxedit)
                                points[i] = Handles.Slider(points[i], (points[i] - pos).normalized);
                            else
                            {
                                if (DSSE.gradedit && i % 2 != 0)
                                    odd = true;
                                Vector3 chhk = Handles.Slider(points[i], (points[i] - pos).normalized * dir[i]);
                                points[i] = chhk;
                            }
                        }


                    }



                }

                if (DSSE.active != -1)
                {
                    if (DSSE.active % 2 == 0)
                    {
                        pos = (points[DSSE.active] + points[DSSE.active + 1]) / 2;
                    }
                    else
                    {
                        pos = (points[DSSE.active] + points[DSSE.active - 1]) / 2;
                    }
                }

                Vector3 wwwid = new Vector3(Vector3.Distance(points[2], points[3]) / 2, Vector3.Distance(points[0], points[1]) / 2, Vector3.Distance(points[4], points[5]) / 2);
                if (DSSE.gradedit)
                {
                    if (!odd)
                        wwwid = new Vector3(points[2].x - wid.x - pos.x, points[0].y - wid.y - pos.y, points[4].z - wid.z - pos.z);
                    else
                        wwwid = new Vector3(pos.x - points[3].x - wid.x, pos.y - points[1].y - wid.y, pos.z - points[5].z - wid.z);
                }


                wwwid = new Vector3(Mathf.Round(wwwid.x * 100) / 100, Mathf.Round(wwwid.y * 100) / 100, Mathf.Round(wwwid.z * 100) / 100);    //despite rounding to 0.01, still randomly chooses .z to be "negative", like negative zero exists
                if (wwwid.x == 0)
                    wwwid.x = 0.000001f;         //setting it to a low pos lmao;
                if (wwwid.x == 0.000001f)           //setting it to zero again, seems to behave after that;
                    wwwid.x = 0;
                if (wwwid.y == 0)
                    wwwid.y = 0.000001f;         // covering my bases if it happens to other directions;
                if (wwwid.y == 0.000001f)
                    wwwid.y = 0;
                if (wwwid.z == 0)
                    wwwid.z = 0.000001f;
                if (wwwid.z == 0.000001f)
                    wwwid.z = 0;

                if (DSSE.boxedit)
                {
                    Undo.RecordObject(Tar, "Changed Values");
                    t.SetVector("_IntClampPos", pos);
                    t.SetVector("_IntClampWidth", wwwid);
                }
                else
                {
                    Undo.RecordObject(Tar, "Changed Values");
                    t.SetVector("_IntPosGrad", wwwid);
                }


                Handles.color = Color.green;
                if (Camera.current != null && DSSE.boxedit)
                {
                    if (DSSE.active != -1)
                        Handles.DrawWireDisc(pos, Camera.current.transform.forward, Mathf.Min(0.3f, ugh));
                    else
                    {
                        Handles.DrawSolidDisc(pos, Camera.current.transform.forward, Mathf.Min(0.3f, ugh));
                        Undo.RecordObject(t, "Changed pos");
                        if (!e.shift)
                        {
                            pos = Handles.PositionHandle(pos, Quaternion.identity);
                            t.SetVector("_IntClampPos", pos);
                        }

                    }
                }




                if (e.button == 0 && e.isMouse && !DSSE.once && e.type == EventType.MouseDown)
                {



                    DSSE.once = true;

                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        Vector3 screenPos = cam.camera.WorldToScreenPoint(points[i]);
                        float curDist = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), new Vector2(DSSE.mousePosition.x, DSSE.mousePosition.y));
                        if (curDist < dist && curDist < distCheck)
                        {

                            currentclosest = i;
                            dist = curDist;
                        }


                    }


                    if (dist != 10000)
                    {
                        DSSE.active = currentclosest;
                    }
                    else
                    {
                        DSSE.active = -1;
                    }

                }

                if (e.button == 0 && e.isMouse && DSSE.once && e.type == EventType.MouseUp)
                    DSSE.once = false;
            }
        }
    }
}