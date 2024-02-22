using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private int _lives = 3;
    private bool _canTakeDamage = true;
    private bool _isBlinkingActive = false;
    [SerializeField] private MeshRenderer _playerMesh1;
    [SerializeField] private SkinnedMeshRenderer _playerMesh2;

    private void Update()
    {
        //Blink
        if (!_canTakeDamage && !_isBlinkingActive)
        {
            _isBlinkingActive = true;
            StartCoroutine(Blink());
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (_canTakeDamage && (other.tag == "Obstacle" || other.tag == "PartOfObstacle" || other.tag == "Plane"))
        {
            if (_lives > 0)
                _lives--;
            if (_lives <= 0)
                GameManager.Instance.GameOver();

            UIManager.Instance.UpdateLives(_lives);

            StartCoroutine(DontTakeDamage(3f));
        }
        else if (other.tag == "Gold")
        {
            GameManager.Instance.IncreaseScore(10);
            other.GetComponent<MeshRenderer>().enabled = false;
            other.GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator DontTakeDamage(float time)
    {
        _canTakeDamage = false;
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(time);
        _canTakeDamage = true;
        GetComponent<CapsuleCollider>().enabled = true;
    }

    IEnumerator Blink()
    {

        _playerMesh1.enabled = false;
        _playerMesh2.enabled = false;

        yield return new WaitForSeconds(0.1f);

        _playerMesh1.enabled = true;
        _playerMesh2.enabled = true;

        yield return new WaitForSeconds(0.1f);
        _isBlinkingActive = false;

    }
}
