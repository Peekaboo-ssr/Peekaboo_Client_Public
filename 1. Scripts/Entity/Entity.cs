using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [field: SerializeField] public AnimationData AnimationData { get; private set; }
    public Animator Animator { get; protected set; }

    protected int ObstacleLayerMask;
    protected const float HORIZONTAL_FOV = 91.0f;
    protected LayerMask playerLayerMask;
    //public float MaxSightRange;

    protected virtual void Awake()
    {
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();
        if (Animator == null) Debug.Log("Animator Null");

        ObstacleLayerMask = LayerMask.GetMask("Wall", "Door");
        playerLayerMask = LayerMask.GetMask("Player");
    }

    /// <summary>
    /// 시야 체크하여 Entity 반환
    /// </summary>
    public Collider FindEntityInSight(string otherEntityLayer, float sightRange)
    {
        int otherEntityLayerMask = LayerMask.GetMask(otherEntityLayer);
        Vector3 entityRayPos = transform.position + Vector3.up * 0.6f;
        //MaxSightRange = GetObstacleDistance(entityRayPos, transform.forward, _obstacleLayerMask);

        #region 시야각 체크
        Collider[] collidersInSight = Physics.OverlapSphere(entityRayPos, sightRange, otherEntityLayerMask, QueryTriggerInteraction.Collide);
        if (collidersInSight == null || collidersInSight.Length == 0)
        {
            return null;
        }

        // 시야각 내의 Collider 중 유효한 Entity 반환
        return collidersInSight.FirstOrDefault(collider =>
            IsEntityInView(collider, sightRange, entityRayPos));
        #endregion
    }

    /// <summary>
    /// 주어진 Collider가 카메라의 FOV와 장애물 여부를 기준으로 시야각 내에 있는지 확인
    /// </summary>
    public bool IsEntityInView(Collider collider, float sightRange, Vector3? entityRayPos = null)
    {
        if (collider == null || collider.gameObject == null) return false;

        Vector3 rayPos = entityRayPos ?? (transform.position + Vector3.up * 0.2f);

        #region 내 주변 반경 체크 => Ghost만!
        /*if (this as Ghost != null)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(
            transform.position,
            2.0f,
            ~0, // 모든 레이어 포함
            QueryTriggerInteraction.Collide
            );

            if (nearbyColliders != null && nearbyColliders.Any(c => c == collider))
                return true;
        }*/
        #endregion

        // Collider가 시야각 내에 있는지 확인
        Vector3 directionToEntity = (collider.transform.position - rayPos).normalized;
        float angleToEntity = Vector3.Angle(transform.forward, directionToEntity);

        if (angleToEntity > HORIZONTAL_FOV / 2)
        {
            return false; // 시야각 밖
        }

        // Collider와의 사이에 장애물이 있는지 확인
        if (Physics.Raycast(rayPos, directionToEntity, out RaycastHit hit, sightRange, ObstacleLayerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.collider == null || hit.collider.gameObject != collider.gameObject)
            {
                return false; // 장애물에 가로막힘
            }
        }

        return true; // 시야 내에 있음
    }

    #region 이전 코드, 안씀
    /// <summary>
    /// 벽이나 문까지의 거리 계산
    /// </summary>
    private float GetObstacleDistance(Vector3 startPosition, Vector3 forwardDirection, int _obstacleLayerMask)
    {
        if (Physics.Raycast(startPosition, forwardDirection, out RaycastHit hit, Mathf.Infinity, _obstacleLayerMask))
        {
            return hit.distance;
        }

        return Mathf.Infinity; // 장애물이 없으면 무한대
    }
    #endregion
}