using UnityEngine;
using UnityEngine.Video;
using Fungus;

public class VideoController : MonoBehaviour
{
    // Unityエディタから設定できるようにpublicでVideo Playerを参照
    public VideoPlayer videoPlayer;
    // 動画終了時に送信するFungusメッセージ名
    public string finishMessage = "VideoFinished";
    // 参照するFlowchartコンポーネメント
    public Flowchart flowchart;

    // FungusのCall Methodから呼び出す再生開始メソッド
    public void PlayVideo()
    {
        // 処理の安全性を高めるため、事前にイベントを解除
        videoPlayer.loopPointReached -= OnVideoFinished;
        // 動画終了時にOnVideoFinishedメソッドを呼び出すよう登録
        videoPlayer.loopPointReached += OnVideoFinished;

        // Raw Imageをアクティブにし、動画を再生
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.Play();
    }

    // 動画再生終了時に呼び出されるコールバックメソッド
    private void OnVideoFinished(VideoPlayer vp)
    {
        // 動画が終了したのでイベントを解除
        videoPlayer.loopPointReached -= OnVideoFinished;

        // 動画表示用のオブジェクトを非アクティブにする（非表示にする）
        videoPlayer.gameObject.SetActive(false);

        // FungusのFlowchartにメッセージを送信し、次の処理をトリガー
        if (flowchart != null)
        {
            flowchart.SendFungusMessage(finishMessage);
        }
    }
}
