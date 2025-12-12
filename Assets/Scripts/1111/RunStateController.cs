using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 런의 상태를 한 곳에서 관리하는 컨트롤러
/// 시작, 일시정지, 클리어, 패배, 재시작까지 단일 파일로 담당한다
/// </summary>
public enum RunState
{
    Idle,       // 시작 전 상태
    Running,    // 진행 상태
    Paused,     // 일시정지 상태
    Cleared,    // 클리어 상태
    Failed,     // 패배 상태
    PostResult  // 결과 후 재시작 대기
}

public class RunStateController : MonoBehaviour
{
    [Header("시간 설정")]
    [SerializeField]
    private float defaultTimeScale = 1.0f; // 기본 시간 배율

    [Header("입력 설정")]
    [SerializeField]
    private KeyCode pauseKey = KeyCode.Escape; // 일시정지 토글 키

    [SerializeField]
    private KeyCode restartKey = KeyCode.R; // 재시작 키

    [SerializeField]
    private KeyCode testClearKey = KeyCode.G; // 임시 클리어 키

    private RunState currentState; // 현재 상태
    private float savedTimeScale;  // 일시정지 전 시간 배율
    private float runElapsed;      // 진행 상태에서 누적된 경과 시간

    /// <summary>
    /// 기능 요약: 컨트롤러를 초기화하고 기본 시간 배율을 적용한다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 전역 시간 배율을 변경한다
    /// </summary>
    private void OnEnable()
    {
        currentState = RunState.Idle; // 시작 전으로 설정
        savedTimeScale = defaultTimeScale; // 시간 배율 저장
        Time.timeScale = defaultTimeScale; // 시간 배율 적용
        runElapsed = 0.0f; // 경과 시간 초기화
    }

    /// <summary>
    /// 기능 요약: 진행 중에는 경과 시간을 누적하고, 간단 입력을 처리한다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// </summary>
    private void Update()
    {
        if (currentState == RunState.Running)
        {
            runElapsed += Time.unscaledDeltaTime; // 경과 시간 누적
        }

        // 아래 입력 처리들은 학습 편의 때문에 이 파일 안에 포함했다
        if (Input.GetKeyDown(pauseKey) == true)
        {
            TogglePause(); // 일시정지 토글
        }

        if (Input.GetKeyDown(testClearKey) == true)
        {
            // 보스 처치 연동 전까지 임시로 클리어를 시험한다
            MarkClear();
        }

        if (Input.GetKeyDown(restartKey) == true)
        {
            if (currentState == RunState.PostResult)
            {
                RestartRun(); // 결과 후 대기일 때만 재시작
            }
        }
    }

    /// <summary>
    /// 기능 요약: 런을 시작 상태에서 진행 상태로 전환한다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 부작용: 전역 시간 배율을 기본값으로 둔다
    /// 성능: 상수 시간
    /// </summary>
    public void BeginRun()
    {
        if (currentState == RunState.Idle)
        {
            currentState = RunState.Running; // 진행으로 전환
            Time.timeScale = defaultTimeScale; // 시간 확인
        }
    }

    /// <summary>
    /// 기능 요약: 일시정지와 재개를 전환한다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 부작용: 전역 시간 배율 변경
    /// 성능: 상수 시간
    /// </summary>
    public void TogglePause()
    {
        if (currentState == RunState.Running)
        {
            Pause();
            return;
        }

        if (currentState == RunState.Paused)
        {
            Resume();
            return;
        }
    }

    /// <summary>
    /// 기능 요약: 진행 상태를 일시정지 상태로 만든다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 부작용: 전역 시간 배율을 영으로 만든다
    /// 성능: 상수 시간
    /// </summary>
    public void Pause()
    {
        if (currentState == RunState.Running)
        {
            currentState = RunState.Paused; // 일시정지로 전환
            savedTimeScale = Time.timeScale; // 이전 배율 저장
            Time.timeScale = 0.0f; // 시간 멈춤
        }
    }

    /// <summary>
    /// 기능 요약: 일시정지에서 진행으로 되돌린다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 부작용: 전역 시간 배율 복원
    /// 성능: 상수 시간
    /// </summary>
    public void Resume()
    {
        if (currentState == RunState.Paused)
        {
            currentState = RunState.Running; // 진행으로 전환
            Time.timeScale = savedTimeScale; // 시간 복원
        }
    }

    /// <summary>
    /// 기능 요약: 진행 중일 때 클리어로 전환하고 결과 대기로 이동한다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 부작용: 시간 정지
    /// 성능: 상수 시간
    /// </summary>
    public void MarkClear()
    {
        if (currentState == RunState.Running)
        {
            currentState = RunState.Cleared; // 클리어로 표시
            Time.timeScale = 0.0f; // 멈춤
            currentState = RunState.PostResult; // 결과 대기로 이동
        }
    }

    /// <summary>
    /// 기능 요약: 진행 중일 때 패배로 전환하고 결과 대기로 이동한다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 부작용: 시간 정지
    /// 성능: 상수 시간
    /// </summary>
    public void MarkFail()
    {
        if (currentState == RunState.Running)
        {
            currentState = RunState.Failed; // 패배로 표시
            Time.timeScale = 0.0f; // 멈춤
            currentState = RunState.PostResult; // 결과 대기로 이동
        }
    }

    /// <summary>
    /// 기능 요약: 결과 대기에서 현재 씬을 다시 불러와 초기 상태로 만든다
    /// 입력 값: 없음
    /// 반환 값: 없음
    /// 부작용: 씬을 다시 로드한다
    /// 성능: 씬 로드 비용
    /// </summary>
    public void RestartRun()
    {
        if (currentState == RunState.PostResult || currentState == RunState.Idle)
        {
            Time.timeScale = defaultTimeScale; // 시간 복구
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 씬 재로드
        }
    }

    /// <summary>
    /// 기능 요약: 현재 상태를 알려준다
    /// 입력 값: 없음
    /// 반환 값: 현재 상태
    /// 부작용: 없음
    /// 성능: 상수 시간
    /// </summary>
    public RunState GetState()
    {
        return currentState; // 상태 반환
    }

    /// <summary>
    /// 기능 요약: 진행 상태에서 누적된 경과 시간을 알려준다
    /// 입력 값: 없음
    /// 반환 값: 초 단위 시간
    /// 부작용: 없음
    /// 성능: 상수 시간
    /// </summary>
    public float GetElapsedSeconds()
    {
        return runElapsed; // 시간 반환
    }
}
