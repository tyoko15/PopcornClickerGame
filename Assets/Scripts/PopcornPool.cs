using System.Collections;
using UnityEngine;

public class PopcornPool : MonoBehaviour
{
    public static PopcornPool Instance { get; private set; }

    [Header("設定")]
    [SerializeField] private GameObject popcornPrefab; // ポップコーンのプレハブ
    [SerializeField] private int maxPoolSize = 100;     // 画面上に存在できる最大個数（上限固定）
    [SerializeField] private int batchSize = 100;       // 1回あたりに生成する個数
    [SerializeField] private float intervalSeconds = 0.1f; // 生成ごとの待ち時間（秒）

    // 生成したオブジェクトを保持しておく配列
    private GameObject[] pool;
    private int createdCount = 0; // 現在生成が完了している個数
    private int currentIndex = 0; // 次に使い回すオブジェクトのインデックス

    void Awake()
    {
        Instance = this;
        pool = new GameObject[maxPoolSize];
    }

    void Start()
    {
        // ゲーム開始時にコルーチンで100個ずつ段階的に生成を開始
        StartCoroutine(InitializePoolCoroutine());
    }

    /// <summary>
    /// 🕒 100個生成しては指定秒数休むのを繰り返し、1000個まで準備するコルーチン
    /// </summary>
    private IEnumerator InitializePoolCoroutine()
    {
        while (createdCount < maxPoolSize)
        {
            // 今回のターンで作成する目標個数を計算（100個ずつ増やす）
            int targetCount = Mathf.Min(createdCount + batchSize, maxPoolSize);

            for (int i = createdCount; i < targetCount; i++)
            {
                pool[i] = Instantiate(popcornPrefab, transform);
                pool[i].SetActive(false); // 最初は非表示
            }

            createdCount = targetCount; // 生成完了数を更新

            // 指定した秒数（0.1秒）だけ待って処理を一時中断（フレームを分散させてカクつき防止）
            yield return new WaitForSeconds(intervalSeconds);
        }

        //Debug.Log($"🍿 ポップコーンプール準備完了！（合計: {createdCount}個）");
    }

    // 🍿 ポップコーンを取得・使い回す（上限を超えたら一番古いやつを画面上から強制移動して再利用）
    public GameObject GetPopcorn(Vector3 position, Quaternion rotation)
    {
        AudioManager.Instance.PlayOneShotSE(0);

        // 配列から次に使うポップコーンを取得
        GameObject obj = pool[currentIndex];

        // 前の状態（勢いなど）をリセットするために一瞬非アクティブ化
        obj.SetActive(false);

        // 物理挙動（Rigidbody2D）の速度と回転をリセット
        if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero; // (旧Unityバージョンの場合は rb.velocity)
            rb.angularVelocity = 0f;
        }

        // 新しい発生位置と回転を適用
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        // 画面に表示！
        obj.SetActive(true);

        // 次のインデックスへ進める（上限に達したら 0 に戻ってぐるぐる回る）
        currentIndex = (currentIndex + 1) % maxPoolSize;

        return obj;
    }

    // 📦 使い終わった時の処理（既存スクリプトとの互換性のために残しています）
    public void ReturnPopcorn(GameObject obj)
    {
        if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        obj.SetActive(false); // 時間経過等で自然消滅させたい場合にも使えます
    }
}