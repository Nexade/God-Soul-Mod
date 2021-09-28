using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003B7 RID: 951
public class DropPlat : MonoBehaviour
{
	public float waittime;
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x00067828 File Offset: 0x00065A28
	private void Start()
	{
		this.Idle();
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x00067830 File Offset: 0x00065A30
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.flipRoutine == null && collision.gameObject.layer == 9 && collision.collider.bounds.min.y > this.collider.bounds.max.y)
		{
			this.flipRoutine = base.StartCoroutine(this.Flip());
		}
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x00067898 File Offset: 0x00065A98
	private void PlaySound(AudioClip clip)
	{
		if (this.audioSource && clip)
		{
			this.audioSource.PlayOneShot(clip);
		}
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x000678BB File Offset: 0x00065ABB
	private void Idle()
	{
		base.transform.SetPositionZ(0.003f);
		this.spriteAnimator.Play(this.idleAnim);
		if (this.collider)
		{
			this.collider.enabled = true;
		}
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x000678F7 File Offset: 0x00065AF7
	private IEnumerator Flip()
	{
		this.PlaySound(this.landSound);
		if (this.strikeEffect)
		{
			this.strikeEffect.SetActive(true);
		}
		yield return new WaitForSeconds(waittime);
		if (this.collider)
		{
			this.collider.enabled = false;
		}
		this.PlaySound(this.dropSound);
		yield return this.StartCoroutine(this.spriteAnimator.PlayAnimWait(this.dropAnim));
		this.transform.SetPositionZ(0.007f);
		yield return new WaitForSeconds(1.5f);
		this.PlaySound(this.flipUpSound);
		yield return this.StartCoroutine(this.spriteAnimator.PlayAnimWait(this.raiseAnim));
		this.flipRoutine = null;
		this.Idle();
		yield break;
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x00067906 File Offset: 0x00065B06
	private IEnumerator Jitter(float duration)
	{
		Transform sprite = this.spriteAnimator.transform;
		Vector3 initialPos = sprite.position;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			sprite.position = initialPos + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), 0f, 0f);
			yield return null;
		}
		sprite.position = initialPos;
		yield break;
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x0006791C File Offset: 0x00065B1C

	// Token: 0x04001A2B RID: 6699
	public tk2dSpriteAnimator spriteAnimator;

	// Token: 0x04001A2C RID: 6700
	[Space]
	public string idleAnim;

	// Token: 0x04001A2D RID: 6701
	public string dropAnim;

	// Token: 0x04001A2E RID: 6702
	public string raiseAnim;

	// Token: 0x04001A2F RID: 6703
	[Space]
	public AudioClip landSound;

	// Token: 0x04001A30 RID: 6704
	public AudioClip dropSound;

	// Token: 0x04001A31 RID: 6705
	public AudioClip flipUpSound;

	// Token: 0x04001A32 RID: 6706
	[Space]
	public GameObject strikeEffect;

	// Token: 0x04001A33 RID: 6707
	[Space]
	public Collider2D collider;

	// Token: 0x04001A34 RID: 6708
	private Coroutine flipRoutine;

	// Token: 0x04001A35 RID: 6709
	private AudioSource audioSource;
}

