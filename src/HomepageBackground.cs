using System.IO;
using SML;
using HarmonyLib;
using Home.HomeScene;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HomepageBackground;

[Mod.SalemMod]
public class HomepageBackground
{
    static Texture2D backgroundTexture = null;

    public static void Start()
    {
        Harmony.CreateAndPatchAll(typeof(HomepageBackground));

        string dir = Path.GetDirectoryName(UnityEngine.Application.dataPath) + "/SalemModLoader/ModFolders/HomepageBackground/";
        byte[] data;

        try
        {
            data = File.ReadAllBytes(dir + "background.png");
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory(dir);
            return;
        }
        catch (FileNotFoundException)
        {
            return;
        }

        backgroundTexture = new Texture2D(2, 2);
        ImageConversion.LoadImage(backgroundTexture, data);
    }

    static void ClearNews(HomeSceneController controller)
    {
        if (ModSettings.GetBool("Hide news tab", "lyricly.homepagebackground"))
        {
            controller.homeNewsTabElementsUI.SetActive(false);
        }
    }

    static void AddBackground(HomeSceneController controller)
    {
        if (!backgroundTexture)
        {
            return;
        }

        var background = new GameObject("HomepageBackground");
        background.transform.SetParent(controller.HudCanvas.transform);
        background.transform.localPosition = Vector3.zero;
        background.transform.localScale = Vector3.one;
        background.transform.SetAsFirstSibling();

        var im = background.AddComponent<RawImage>();
        im.texture = backgroundTexture;
        im.rectTransform.anchorMin = Vector2.zero;
        im.rectTransform.anchorMax = Vector2.one;
        im.rectTransform.sizeDelta = Vector2.zero;
    }

    [HarmonyPatch(typeof(HomeSceneController), "Start")]
    [HarmonyPostfix]
    public static IEnumerator ShowBackground(IEnumerator original, HomeSceneController __instance)
    {
        AddBackground(__instance);

        while (original.MoveNext())
        {
            yield return original.Current;
        }

        ClearNews(__instance);
    }

    [HarmonyPatch(typeof(HomeSceneController), "CloseGameModeSelectionPopup")]
    [HarmonyPostfix]
    public static void KeepNewsHidden(HomeSceneController __instance)
    {
        ClearNews(__instance);
    }
}
