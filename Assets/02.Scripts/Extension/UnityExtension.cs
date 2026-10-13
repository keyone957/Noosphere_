using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtension
{
    // 헥사값 컬러 반환( 코드 순서 : RGBA )
    public static Color HexColor( string hexCode )
    {
        Color color;
        if ( ColorUtility.TryParseHtmlString( hexCode, out color ) )
        {
            return color;
        }
            
        Debug.LogError( "[UnityExtension::HexColor]invalid hex code - " + hexCode );
        return Color.white;
    }
}
