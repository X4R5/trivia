using UnityEngine;
using TMPro;
using Fusion;

public class FusionScoreManager : NetworkBehaviour
{
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    private int player1Score;
    private int player2Score;

    public void UpdateScore(int playerIndex, int scoreDelta)
    {
        if (!HasStateAuthority) return;

        if (playerIndex == 1)
        {
            player1Score += scoreDelta;
        }
        else if (playerIndex == 2)
        {
            player2Score += scoreDelta;
        }

        RpcUpdateScores(player1Score, player2Score);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RpcUpdateScores(int player1Score, int player2Score)
    {
        this.player1Score = player1Score;
        this.player2Score = player2Score;

        player1ScoreText.text = "Player 1 Score: " + player1Score;
        player2ScoreText.text = "Player 2 Score: " + player2Score;
    }
}
