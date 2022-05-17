using System.Collections;
using UnityEngine;

public class DropPlat : MonoBehaviour
{
	public float waittime;

	public tk2dSpriteAnimator spriteAnimator;

	[Space]
	public string idleAnim;

	public string dropAnim;

	public string raiseAnim;

	[Space]
	public AudioClip landSound;

	public AudioClip dropSound;

	public AudioClip flipUpSound;

	[Space]
	public GameObject strikeEffect;

	[Space]
	public Collider2D collider;

	private Coroutine flipRoutine;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = ((Component)this).GetComponent<AudioSource>();
	}

	private void Start()
	{
		Idle();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (flipRoutine == null && collision.get_gameObject().get_layer() == 9)
		{
			Bounds bounds = collision.get_collider().get_bounds();
			float y = ((Bounds)(ref bounds)).get_min().y;
			bounds = collider.get_bounds();
			if (y > ((Bounds)(ref bounds)).get_max().y)
			{
				flipRoutine = ((MonoBehaviour)this).StartCoroutine(Flip());
			}
		}
	}

	private void PlaySound(AudioClip clip)
	{
		if (Object.op_Implicit((Object)(object)audioSource) && Object.op_Implicit((Object)(object)clip))
		{
			audioSource.PlayOneShot(clip);
		}
	}

	private void Idle()
	{
		Extensions.SetPositionZ(((Component)this).get_transform(), 0.003f);
		spriteAnimator.Play(idleAnim);
		if (Object.op_Implicit((Object)(object)collider))
		{
			((Behaviour)collider).set_enabled(true);
		}
	}

	private IEnumerator Flip()
	{
		PlaySound(landSound);
		if (Object.op_Implicit((Object)(object)strikeEffect))
		{
			strikeEffect.SetActive(true);
		}
		yield return (object)new WaitForSeconds(waittime);
		if (Object.op_Implicit((Object)(object)collider))
		{
			((Behaviour)collider).set_enabled(false);
		}
		PlaySound(dropSound);
		yield return ((MonoBehaviour)this).StartCoroutine(Extensions.PlayAnimWait(spriteAnimator, dropAnim));
		Extensions.SetPositionZ(((Component)this).get_transform(), 0.007f);
		yield return (object)new WaitForSeconds(1.5f);
		PlaySound(flipUpSound);
		yield return ((MonoBehaviour)this).StartCoroutine(Extensions.PlayAnimWait(spriteAnimator, raiseAnim));
		flipRoutine = null;
		Idle();
	}

	private IEnumerator Jitter(float duration)
	{
		Transform sprite = ((Component)spriteAnimator).get_transform();
		Vector3 initialPos = sprite.get_position();
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.get_deltaTime())
		{
			sprite.set_position(initialPos + new Vector3(Random.Range(-0.1f, 0.1f), 0f, 0f));
			yield return null;
		}
		sprite.set_position(initialPos);
	}
}
