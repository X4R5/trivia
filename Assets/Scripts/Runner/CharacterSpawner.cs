using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private Transform centerPoint;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private float radius = 5f;

    [SerializeField] int characterCount = 4;

    private void Start()
    {
        characterCount = PlayerPrefs.GetInt("QuizScore");
        PlaceCharactersInCircle(characterCount);
        PlayerPrefs.SetInt("QuizScore", 0);
    }

    private void PlaceCharactersInCircle(int count)
    {
        if (count == 1)
        {
            var newCharacter = Instantiate(characterPrefab, centerPoint.position, Quaternion.identity, centerPoint);
            newCharacter.transform.SetParent(centerPoint);
        }
        else if (count == 2)
        {
            Vector3 position1 = centerPoint.position + new Vector3(-radius, 0, 0);
            Vector3 position2 = centerPoint.position + new Vector3(radius, 0, 0);
            var newCharacter = Instantiate(characterPrefab, position1, Quaternion.identity, centerPoint);
            newCharacter.transform.SetParent(centerPoint);
            newCharacter = Instantiate(characterPrefab, position2, Quaternion.identity, centerPoint);
            newCharacter.transform.SetParent(centerPoint);
        }
        else
        {
            Instantiate(characterPrefab, centerPoint.position, Quaternion.identity, centerPoint);

            float angleStep = 360f / (count - 1);
            for (int i = 1; i < count; i++)
            {
                float angle = angleStep * (i - 1);
                Vector3 position = centerPoint.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
                var newCharacter = Instantiate(characterPrefab, position, Quaternion.identity, centerPoint);
                newCharacter.transform.SetParent(centerPoint);
            }
        }
    }
}
