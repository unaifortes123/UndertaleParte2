using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapyrusAnimationController : MonoBehaviour
{
	private Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void PlayAnimation(string animationName)
	{
		if (string.IsNullOrEmpty(animationName)) return;
		Debug.Log("Reproduciendo animación: " + animationName);
		animator.Play(animationName);
	}
	public void SetIdle()
	{
		Debug.Log("Papyrus vuelve a Idle");
		animator.Play("Papyrus_Idle");
	}
}
