using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    private string nextScene;
    private string startSceneName = "StartScene";
    private string loadSceneName = "LoadingScene";
    private string mainSceneName = "MainScene";

    private string HHSceneName = "HHScene";
    private Slider progressBar;
    public async UniTask LoadMainScene(Action onComplete = null)
    {
        nextScene = mainSceneName;
        SceneManager.LoadScene(loadSceneName);
        await UniTask.WaitUntil(() => GameServerSocketManager.Instance.IsRoomReady);
        Debug.Log("LoadMainScene : RoomReady");
        GameServerSocketManager.Instance.IsLoading = true;
        await OnLoadMainSceneAsync();
        Debug.Log("LoadMainScene : SceneReady");
        await UniTask.WaitUntil(() => GameManager.Instance.Player != null);
        Debug.Log("LoadMainScene : PlayerReady");
        await VivoxManager.Instance.Join3DChannel(GameManager.Instance.Player.gameObject);
        Debug.Log("LoadMainScene : VivoxReady");
        await UniTask.WaitUntil(() => RemoteManager.Instance != null);
        Debug.Log("LoadMainScene : RemoteReady");
        GameServerSocketManager.Instance.IsLoading = false;
        onComplete?.Invoke();
    }
    private async UniTask OnLoadMainSceneAsync()
    {
        await UniTask.Yield();

        await UniTask.WaitUntil(() =>
        {
            progressBar = FindFirstObjectByType<Slider>();
            return progressBar != null;
        });

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone) 
        {
            await UniTask.Yield();
            timer += Time.deltaTime;
            if(op.progress < 0.9f)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, op.progress);
                if (progressBar.value >= op.progress)
                    timer = 0f;
            }
            else
            {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);
                if (progressBar.value >= 0.99f)
                {
                    op.allowSceneActivation = true;
                    SoundManager.Instance.PlayBgm(EBgm.WaitingRoom);
                    return;
                }
            }
        }
    }
    public async UniTask LoadStartScene()
    {
        nextScene = startSceneName;
        SceneManager.LoadScene(loadSceneName);

        SoundManager.Instance.StopBgm();
        VivoxManager.Instance.LeaveAllChannel();
        await OnLoadStartSceneAsync();
        await UniTask.WaitUntil(() => StartUIManager.Instance != null);
        StartUIManager.Instance.InitStartScene();
    }
    private async UniTask OnLoadStartSceneAsync()
    {
        await UniTask.Yield();

        await UniTask.WaitUntil(() =>
        {
            progressBar = FindFirstObjectByType<Slider>();
            return progressBar != null;
        });

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            await UniTask.Yield();
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, op.progress);
                if (progressBar.value >= op.progress)
                    timer = 0f;
            }
            else
            {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);
                if (progressBar.value >= 0.99f)
                {
                    op.allowSceneActivation = true;
                    return;
                }
            }
        }
    }
}
