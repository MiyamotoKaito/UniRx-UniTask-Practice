using System;
using System.Collections;
using UniRx;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Samples.Section1
{
    public class DownLoadTexture : MonoBehaviour
    {
        [SerializeField] private RawImage _rawImage;

        private void Start()
        {
            // 表示したい画像のアドレス
            var uri = "file:///C:/UnityProjects/UniR3UniTaskPractice/Assets/Arts/Textures/証明写真.png";

            GetTextureAsync(uri).
                OnErrorRetry(
                onError: (Exception _) => { },
                retryCount: 3
                ).Subscribe(
                    result => { _rawImage.texture = result; },
                    error => { Debug.LogError(error); }
                ).AddTo(this);
               
        }
        /// <summary>
        /// コルーチンを使ってその結果をObservableで返す
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private IObservable<Texture> GetTextureAsync(string uri)
        {
            return Observable.FromCoroutine<Texture>(observer =>
            {
                return GetTextureCoroutine(observer, uri);
            });
        }
        /// <summary>
        /// コルーチンでテクスチャをダウンロードする
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        private IEnumerator GetTextureCoroutine(IObserver<Texture> observer, string uri)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    observer.OnError(new Exception(uwr.error));
                    yield break;
                }

                var result = DownloadHandlerTexture.GetContent(uwr);

                observer.OnNext(result);
                observer.OnCompleted();
            }
        }
    }
}