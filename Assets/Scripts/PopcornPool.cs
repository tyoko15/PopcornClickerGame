using System.Collections.Generic;
using UnityEngine;

public class PopcornPool : MonoBehaviour
{
    public static PopcornPool Instance { get; private set; }

    [Header("設定")]
    [SerializeField] private GameObject popcornPrefab; // ポップコーンのプレハブ
    [SerializeField] private int initialPoolSize = 300; // あらかじめ作っておく個数

    // 非アクティブなポップコーンを保管しておくキュー（保管庫）
    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    // ゲーム開始時にあらかじめ大量作成して隠しておく（プレウォーム）
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(popcornPrefab, transform);
            obj.SetActive(false); // 非表示にしておく
            poolQueue.Enqueue(obj); // 保管庫に入れる
        }
    }

    // 🍿 保管庫からポップコーンを借りる（Instantiateの代わり）
    public GameObject GetPopcorn(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        AudioManager.Instance.PlayOneShotSE(0);
        // 保管庫に空きがあれば使い回す
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            // もし足りなくなったら、その時だけ新しく1つ追加作成する
            obj = Instantiate(popcornPrefab, transform);
        }

        // 位置と回転をリセットして画面に出す
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    // 📦 使い終わったら保管庫に戻す（Destroyの代わり）
    public void ReturnPopcorn(GameObject obj)
    {
        // 物理挙動（Rigidbody2D）を使っている場合は速度をリセットしておく
        if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        obj.SetActive(false); // 画面から消す
        poolQueue.Enqueue(obj); // 保管庫に戻す
    }
}