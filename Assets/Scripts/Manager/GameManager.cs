using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState currentState;

    public float targetPlayTime;    // 클리어 조건이 될 목표 시간.

    private float elapsedPlayTime;  // 실제로 플레이가 진행된 누적 시간.

    private bool isPlaying; // 현재 게임이 진행중인지 여부.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = GameState.Ready;
        elapsedPlayTime = 0.0f;
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == GameState.Playing && isPlaying == true)
        {
            elapsedPlayTime += Time.deltaTime;

            if(elapsedPlayTime >= targetPlayTime)
            {
                HandleGameClear();
            }
        }

        if(currentState == GameState.Ready)
        {
            if(Input.GetKeyDown(KeyCode.Space) == true)
            {
                StartGame();
            }
        }

        // 테스트용 코드. 나중에 지워도 됨.
        if(currentState == GameState.Playing)
        {
            if(Input.GetKeyDown(KeyCode.K) == true)
            {
                HandleGameOver();
            }
        }
    }

    public void StartGame()
    {
        elapsedPlayTime = 0.0f;

        currentState = GameState.Playing;

        isPlaying = true;

        // 추구 추가할 기능.
        // 플레이어 초기화.
        // 적 스폰 시작.
        // UI 업데이트.

        Debug.Log("Game Started");
    }

    public void HandleGameOver()
    {
        currentState = GameState.GameOver;

        isPlaying = false;

        // GameOver UI 표시.
        // 재시작 버튼 노출.
        // 데이터 정리.

        Debug.Log("Game Over");
    }

    public void HandleGameClear()
    {
        currentState = GameState.Clear;

        isPlaying = false;

        // 보상 처리.
        // 다음 난이도 잠금 해제.
        // 결과 화면 표시.

        Debug.Log("Game Clear");
    }

    public bool IsPlaying()
    {
        if(currentState == GameState.Playing && isPlaying == true)
        {
            return true;
        }

        return false;
    }

    public float GetElapsedPlayTime()
    {
        return elapsedPlayTime;
    }
}
