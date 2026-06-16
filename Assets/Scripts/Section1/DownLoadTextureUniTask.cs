using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownLoadTextureUniTask : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage;

    private void Start()
    {
        // このMonoBehaviourに紐づいたCancellationTokenを取得
        var token = this.GetCancellationTokenOnDestroy();

        SetupTextureAsync(token).Forget();
    }
    /// <summary>
    /// UniTaskを使ってTextureをロードする
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTaskVoid SetupTextureAsync(CancellationToken token)
    {
        try
        {
            var uri = "file:///C:/UnityProjects/UniR3UniTaskPractice/Assets/Arts/Textures/証明写真.png";
            // UniRxを使いたいので、UniTaskからObservableへ変換する。
            var observable = Observable
                .Defer(() =>
                {
                    return GetTextureAsync(uri, token)
                    .ToObservable();
                })
                .Retry(3);

            var texture = await observable;

            _rawImage.texture = texture;
        }
        // when は条件式がtrueだったらCatchブロックが実行されるらしい。だからif文と考えてよろしC#6.0から導入された
        catch (Exception e) when (!(e is OperationCanceledException))
        {
            Debug.LogError(e);
        }
    }
    /// <summary>
    /// 戻り値TextureのUniTaskを使用して画像をURI経由でロードするメソッド
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask<Texture> GetTextureAsync(string uri, CancellationToken token)
    {
        using (var uwr = UnityWebRequestTexture.GetTexture(uri))
        {
            await uwr.SendWebRequest().WithCancellation(token);
            return ((DownloadHandlerTexture)uwr.downloadHandler).texture;
        }
    }
}
