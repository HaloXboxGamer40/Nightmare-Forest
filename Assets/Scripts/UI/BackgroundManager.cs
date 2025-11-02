using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour {

    public Sprite[] backgrounds;
    public Image imageBackground;
    public Animator animator;

    private GameObject bgOverlay;

    private void Awake() {

        bgOverlay = GameObject.Find("BackgroundSwitch");
        bgOverlay.SetActive(false);

    }

    public void SetBackground(int id, bool animate, GameObject menu1 = null, GameObject menu2 = null) {

        if (animate) {
            StartCoroutine(AnimatedBackground(id, menu1, menu2));
            return;
        }

        imageBackground.sprite = backgrounds[id];

    }

    IEnumerator AnimatedBackground(int id, GameObject menu1, GameObject menu2) {
        bgOverlay.SetActive(true);
        animator.Play("Switch Background", 0, 0f);

        yield return new WaitForSeconds(1f);
        imageBackground.sprite = backgrounds[id];
        menu1.SetActive(false);
        menu2.SetActive(true);

        yield return new WaitForSeconds(1f);
        bgOverlay.SetActive(false);
    }
}
