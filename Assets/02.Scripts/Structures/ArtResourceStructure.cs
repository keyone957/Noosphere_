using UnityEngine;

public class ArtResourceStructure
{
    public string artresourceId;
    public string artresourceName;
    public string artresourceType;
    public string filePath;
    public string filePathInventoryThumbnail;
    public string filePathEvidencePrefab;
    public string filePathMapBackground;
    public string filePathContentBackground;
    public string filePathStartPage;
    public int pageCnt;

    public Sprite GetSpriteFromFilePath(string path)
    {
        Sprite resultSprite = Resources.Load<Sprite>(path);

        if (resultSprite == null)
        {
            Debug.Log(path + "에 이미지 리소스가 존재하지 않습니다.");
            return null;
        }

        return resultSprite;
    }

    public GameObject GetPrefabFromFilePath()
    {
        GameObject resultGameObject = Resources.Load<GameObject>(filePathEvidencePrefab);

        if (resultGameObject == null)
        {
            Debug.Log(filePathEvidencePrefab + "에 프리팹이 존재하지 않습니다.");
            return null;
        }

        return resultGameObject;
    }
}