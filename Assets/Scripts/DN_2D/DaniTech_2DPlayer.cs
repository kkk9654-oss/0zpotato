using UnityEngine;

// Rigidbody2D 필수 컴포넌트 강제
[RequireComponent(typeof(Rigidbody2D))]
public class DaniTech_2DPlayer : MonoBehaviour
{
    [Header("화면 제한 설정")]
    public float padding = 0.5f; // 화면 밖으로 나가지 않게 여유 공간

    private Camera mainCam;

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _jumpForce = 12f;

    [Header("지면 체크 설정")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _checkRadius = 0.5f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("애니메이터")]
    [SerializeField] private DaniTech_2DAnimatorController AnimatorController_Entity;

    [Header("UI")]
    [SerializeField] private DaniTech_ScoreUI _scoreUI;

    private Rigidbody2D _rigidBody;
    private bool _isGrounded;
    private float _horizontalInput;
    private bool _lookRight = true;

    private int _currentScore;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        // 플레이어가 넘어지지 않도록 회전 고정
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Start()
    {
        // 메인 카메라 가져오기
        mainCam = Camera.main;
    }

    void Update()
    {
        // 1. 입력 받기
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. 점프
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            Jump();
        }

        // 3. 방향 전환
        if (_horizontalInput > 0 && !_lookRight)
            Flip();
        else if (_horizontalInput < 0 && _lookRight)
            Flip();

        // 4. 애니메이션 상태 처리
        bool isMoving = (_horizontalInput != 0);
        ChangePlayerState(isMoving ? DaniTech_EntityAnimState.Walk : DaniTech_EntityAnimState.Idle);

        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangePlayerState(DaniTech_EntityAnimState.Atk);
        }
    }

    void FixedUpdate()
    {
        // 1. 지면 체크 (물리 기준)
        _isGrounded = Physics2D.OverlapCircle(
            _groundCheck.position,
            _checkRadius,
            _groundLayer
        );

        // 2. 이동 처리
        Move();

        // 3. 화면 밖으로 못 나가게 제한 (중요)
        ClampToScreen();
    }

    // =========================
    // 이동
    // =========================
    void Move()
    {
        // Rigidbody2D는 velocity 사용 (linearVelocity ❌)
        _rigidBody.linearVelocity = new Vector2(
            _horizontalInput * _moveSpeed,
            _rigidBody.linearVelocity.y
        );
    }

    void Jump()
    {
        _rigidBody.linearVelocity = new Vector2(
            _rigidBody.linearVelocity.x,
            _jumpForce
        );
    }

    void Flip()
    {
        _lookRight = !_lookRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // =========================
    // 화면 밖 이동 방지
    // =========================
    void ClampToScreen()
    {
        Vector3 pos = transform.position;

        Vector3 min = mainCam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 max = mainCam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        pos.x = Mathf.Clamp(pos.x, min.x + padding, max.x - padding);
        pos.y = Mathf.Clamp(pos.y, min.y + padding, max.y - padding);

        transform.position = pos;
    }

    // =========================
    // 애니메이션 상태 변경
    // =========================
    private void ChangePlayerState(DaniTech_EntityAnimState newState)
    {
        AnimatorController_Entity.SetState(newState);
    }

    // =========================
    // 디버그용 지면 표시
    // =========================
    private void OnDrawGizmos()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }
    }

    // =========================
    // 몬스터 충돌 처리
    // =========================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
            return;

        var enemyComponent = collision.gameObject.GetComponent<DaniTech_2DEnemy>();

        if (enemyComponent == null)
        {
            Debug.LogWarning("Enemy 컴포넌트를 찾지 못했습니다.");
            return;
        }

        DaniTechGameObjectManager.Inst.RequestDestroyEntityObject(
            enemyComponent.EntityInstancId
        );

        AddGameScore();
    }

    // =========================
    // 점수 증가
    // =========================
    private void AddGameScore()
    {
        _currentScore++;
        _scoreUI.AddGameScore(_currentScore);
    }
}