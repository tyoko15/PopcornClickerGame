using UnityEngine;

public class Popcorn : MonoBehaviour
{
    [Header("飛ぶ勢いの設定")]
    [SerializeField] private float minForce = 3f;  // 最小の勢い
    [SerializeField] private float maxForce = 6f;  // 最大の勢い

    [Header("消滅までの時間（秒）")]
    [SerializeField] private float destroyTime = 2.0f;
    float timer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ① 上方向を中心に、左右ランダムな角度（斜め上）へ飛ばすベクトルを作成
        // （例: x軸は-0.5〜0.5、y軸は0.8〜1.2の範囲でランダム）
        Vector2 jumpDirection = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.8f, 1.2f)).normalized;

        // ② ランダムな強さを設定
        float randomForce = Random.Range(minForce, maxForce);

        // ③ 物理的な力を加えて弾けさせる (Impulse = 一瞬でポンッと力を加える)
        rb.AddForce(jumpDirection * randomForce, ForceMode2D.Impulse);

        // ④ 少し回転を加えてポップコーンらしさを出す（回転トルク）
        float randomTorque = Random.Range(-50f, 50f);
        rb.AddTorque(randomTorque);

    }
    private void Update()
    {
        if (timer > destroyTime)
        {
            Destroy(gameObject);
        }
        else timer += Time.deltaTime;
    }
}