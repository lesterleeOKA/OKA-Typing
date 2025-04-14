using SimpleJSON;
using System;
using System.Drawing;
using UnityEngine;

[Serializable]
public class GameSettings : Settings
{
    public string prefabItemImageUrl;
    public UnityEngine.Color textColor;
}

public static class SetParams
{
    public static void setCustomParameters(GameSettings settings = null, JSONNode jsonNode = null)
    {
        if (settings != null && jsonNode != null)
        {
            ////////Game Customization params/////////
            string prefabItemUrl = jsonNode["setting"]["itemImage"] != null ?
                                jsonNode["setting"]["itemImage"].ToString().Replace("\"", "") : null;

            if (!string.IsNullOrEmpty(prefabItemUrl))
            {
                if (!prefabItemUrl.StartsWith("https://") || !prefabItemUrl.StartsWith(APIConstant.blobServerRelativePath))
                    settings.prefabItemImageUrl = APIConstant.blobServerRelativePath + prefabItemUrl;
            }

            string hexColor = jsonNode["setting"]["text_color"] != null ?
                                jsonNode["setting"]["text_color"].ToString().Replace("\"", "") : null;
            if (hexColor != null)
            {
                if (ColorUtility.TryParseHtmlString(hexColor, out UnityEngine.Color color))
                {
                    settings.textColor = color;
                    LoaderConfig.Instance.gameSetup.keyboardTextColor = settings.textColor;
                }
            }
        }


    }
}