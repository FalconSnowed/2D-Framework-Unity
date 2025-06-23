using UnityEngine;

public class ZoneTransition : MonoBehaviour
{
    public string targetScene;
    public Vector2 spawnPosition;
    public Animator fadeAnimator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);

        // Téléportation (si même scène)
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = spawnPosition;

        fadeAnimator.SetTrigger("FadeIn");
    }
}
