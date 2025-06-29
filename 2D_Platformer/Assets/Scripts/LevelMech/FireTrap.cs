using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private float runOffDuration;
    [SerializeField] private ActivateButton activateButton;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private bool isActive;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        if (activateButton == null) Debug.LogWarning("You don't have activation button on " + gameObject.name + "!");
        else activateButton.button_press += ActivateButton_button_press;
        SetFire(true);
    }

    private void ActivateButton_button_press(object sender, System.EventArgs e)
    {
        SwitchOffFire();
    }

    public void SwitchOffFire()
    {
        if (!isActive) return;
        StartCoroutine(FireCoroutine());
    }


    private IEnumerator FireCoroutine()
    {
        SetFire(false);
        yield return new WaitForSeconds(runOffDuration);
        SetFire(true);
    }
    private void SetFire(bool active)
    {
        anim.SetBool("Active", active);
        capsuleCollider.enabled = active;
        isActive = active;
    }
}
