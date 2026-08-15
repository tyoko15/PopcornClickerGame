using UnityEngine;
using UnityEngine.UI;

public class Popcorn : MonoBehaviour
{
    [Header("飛ぶ勢いの設定")]
    private float minForce;  // 最小の勢い
    private float maxForce;  // 最大の勢い

    [Header("消滅までの時間（秒）")]
    [SerializeField] private float returnTime = 2.0f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        minForce = 3 * GameManager.Instance.popcornForceMultiple;
        maxForce = minForce + 3;
        Pop();      // プールから出るたびに弾けさせる！
        Invoke("ReturnPopcorn", returnTime);
    }

    /// <summary>
    /// ポップコーンのイラスト編集
    /// </summary>
    /// <param name="image"></param>
    public void InitImage(Sprite image) => transform.GetComponent<SpriteRenderer>().sprite = image;


    /// <summary>
    /// 弾ける挙動関数
    /// </summary>
    public void Pop()
    {
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

    /// <summary>
    /// リセット
    /// </summary>
    void ReturnPopcorn()
    {
        PopcornPool.Instance.ReturnPopcorn(gameObject);
    }
}