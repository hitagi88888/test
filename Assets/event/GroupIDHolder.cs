using UnityEngine;

public class GroupIDHolder : MonoBehaviour
{
    public int GroupID; // GroupIDを整数型として設定

    public void OnObjectClicked() // クリック時に呼ばれる
    {
        Debug.Log("クリックされました");
        TextClickSystem.Instance.OnObjectClicked(GroupID);
    }
}