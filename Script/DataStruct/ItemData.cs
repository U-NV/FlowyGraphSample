using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemData", menuName = "FlowyGraphSample/ItemData", order = 2)]
public class ItemData : ScriptableObject
{
    public string itemName;

    [SerializeField] private string key;
    public string Key => string.IsNullOrEmpty(key) ? itemName : key;

#if UNITY_EDITOR
    private void OnValidate()
    {
        key = GenerateReadableKey();
    }

    private string GenerateReadableKey()
    {
        var assetPath = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(assetPath))
        {
            return itemName;
        }

        var fileName = Path.GetFileNameWithoutExtension(assetPath);
        var hash = GetShortHash(assetPath);
        return $"{fileName}_{hash}";
    }

    private static string GetShortHash(string input)
    {
        using (var md5 = MD5.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(bytes);
            var builder = new StringBuilder(8);
            for (var i = 0; i < 4; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
#endif
}
