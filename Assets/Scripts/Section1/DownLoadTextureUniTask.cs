using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DownLoadTextureUniTask : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage;

    private void Start()
    {
        
    }

    private async UniTaskVoid SetupTextureAsync(CancellationToken token)
    {

    }
    private async UniTask<Texture> GetTextureAsync(string uri, CancellationToken token)
    {

    }
}
