using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using HellMod;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using Modding;
using Modding.Delegates;
using On;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GodSoul;

[UsedImplicitly]
public class GodSoul : Mod, ITogglableMod, IMod, ILogger, IGlobalSettings<GlobalModSettings>
{
	private GlobalModSettings _settings = new GlobalModSettings();

	public float framecount = 0f;

	public List<Enemy> dupedenemies = new List<Enemy>();

	private WebClient web = new WebClient();

	public bool jellydeadly = true;

	public bool jellyroom;

	public bool droppersinvis = true;

	public bool mantisroom;

	public bool sanctumroom;

	public bool radiantroom;

	public List<PlayMakerFSM> radiances = new List<PlayMakerFSM>();

	public int firstframe = 0;

	public string Sceneplace = "";

	public string extra;

	public GameObject megalight;

	public List<Fsm> zaps;

	public int wt = 130;

	private int homescene;

	private int build;

	public GameObject blockersaw;
	public bool cansummon = true;
	public bool cansummon2 = true;
	public bool cansummon3 = true;
	public bool cansummon4 = true;

	public float rotatenum;

	public GameObject slashsymbol;

	public AudioClip slashsound;

	private Vector3 pos;

	public GameObject explosion;

	public GameObject Sign;

	public GameObject plant;

	public GameObject bighop;

	public GameObject sentry;

	public GameObject mantisshot;

	public GameObject spikes;

	public GameObject beam;

	public GameObject shock;

	public GameObject saw;

	public GameObject smolhop;

	public GameObject Poof;

	public GameObject GasEffect;

	public GameObject Fungbab;

	public GameObject Fungfly;

	public int round;

	public GameObject annoying;

	public GameObject hk1;

	public GameObject hk2;

	public bool finalroom;

	public bool infected;

	public bool hitSpin;

	public List<HealthManager> ghosts;

	public bool FightingGhost;

	public GameObject killme;

	private Random rand = new Random();

	public DamageHero dmghero;

	public List<GameObject> deadones;

	public GodSoul()
		: base("God Soul")
	{
	}

	public void OnLoadGlobal(GlobalModSettings s)
	{
		_settings = s;
	}

	public GlobalModSettings OnSaveGlobal()
	{
		return _settings;
	}

	public override string GetVersion()
	{
		return "1.4";
	}

	public override void Initialize()
	{
		ModHooks.add_ColliderCreateHook((Action<GameObject>)OnCollMake);
		ModHooks.add_SlashHitHook(new SlashHitHandler(OnSlashHit));
		ModHooks.add_HeroUpdateHook((Action)OnHeroUpdate);
		ModHooks.add_LanguageGetHook(new LanguageGetProxy(LanguageGet));
		ModHooks.add_TakeHealthHook(new TakeHealthProxy(OnHealthTaken));
		ModHooks.add_SoulGainHook((Func<int, int>)OnSoulGain);
		ModHooks.add_GetPlayerIntHook(new GetIntProxy(OnInt));
		ModHooks.add_HitInstanceHook(new HitInstanceHandler(OnHit));
		ModHooks.add_GetPlayerBoolHook(new GetBoolProxy(OnBool));
		HeroController.add_CanFocus(new hook_CanFocus(Focus));
	}

	private bool Focus(orig_CanFocus orig, HeroController self)
	{
		return true;
		if (finalroom)
		{
			((Loggable)this).Log(hk1.GetComponent<HealthManager>().hp + "   " + hk2.GetComponent<HealthManager>().hp);
		}
		if (Sceneplace.Contains("Dream_Guardian") || (finalroom && (hk1.GetComponent<HealthManager>().hp <= 0 || hk2.GetComponent<HealthManager>().hp <= 0)))
		{
			return true;
		}
		return false;
	}

	private IEnumerator forever()
	{
		while (true)
		{
			yield return (object)new WaitForEndOfFrame();
			if (Object.op_Implicit((Object)(object)saw))
			{
				Object.DontDestroyOnLoad((Object)(object)saw);
			}
			if (Object.op_Implicit((Object)(object)shock))
			{
				Object.DontDestroyOnLoad((Object)(object)shock);
			}
			if (Object.op_Implicit((Object)(object)beam))
			{
				Object.DontDestroyOnLoad((Object)(object)beam);
			}
			if (Object.op_Implicit((Object)(object)spikes))
			{
				Object.DontDestroyOnLoad((Object)(object)spikes);
			}
			if (Object.op_Implicit((Object)(object)mantisshot))
			{
				Object.DontDestroyOnLoad((Object)(object)mantisshot);
			}
			if (Object.op_Implicit((Object)(object)smolhop))
			{
				Object.DontDestroyOnLoad((Object)(object)smolhop);
			}
			if (Object.op_Implicit((Object)(object)bighop))
			{
				Object.DontDestroyOnLoad((Object)(object)bighop);
			}
			if (Object.op_Implicit((Object)(object)plant))
			{
				Object.DontDestroyOnLoad((Object)(object)plant);
			}
			if (Object.op_Implicit((Object)(object)megalight))
			{
				Object.DontDestroyOnLoad((Object)(object)megalight);
			}
			if (Object.op_Implicit((Object)(object)explosion))
			{
				Object.DontDestroyOnLoad((Object)(object)explosion);
			}
			if (Object.op_Implicit((Object)(object)Poof))
			{
				Object.DontDestroyOnLoad((Object)(object)Poof);
			}
			if (Object.op_Implicit((Object)(object)GasEffect))
			{
				Object.DontDestroyOnLoad((Object)(object)GasEffect);
			}
			if (Object.op_Implicit((Object)(object)Fungfly))
			{
				Object.DontDestroyOnLoad((Object)(object)Fungfly);
			}
		}
	}

	private bool OnBool(string name, bool orig)
	{
		return name switch
		{
			"hasDreamGate" => false, 
			"equippedCharm_6" => true, 
			"gotCharm_6" => true, 
			"zoteDead" => false, 
			_ => orig, 
		};
	}

	private void OnSlashHit(Collider2D otherCollider, GameObject slash)
	{
		if (((Object)((Component)otherCollider).get_gameObject()).get_name().Contains("Spikes Top"))
		{
			ObjectPoolExtensions.Spawn(slashsymbol, ((Component)otherCollider).get_gameObject().get_transform());
			AudioSource val = ((Component)HeroController.get_instance()).get_gameObject().AddComponent<AudioSource>();
			val.PlayOneShot(slashsound);
		}
		MonoBehaviour[] components = ((Component)otherCollider).GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour val2 in components)
		{
			if (((object)val2).GetType().Name == "HealthManager" && !Contains(((Component)val2).get_gameObject()))
			{
				GameObject gameObject = ((Component)val2).get_gameObject();
				Clone(gameObject, 2);
			}
		}
	}

	public bool Contains(GameObject gi)
	{
		foreach (Enemy dupedenemy in dupedenemies)
		{
			if ((Object)(object)dupedenemy.enemy == (Object)(object)gi)
			{
				return true;
			}
		}
		return false;
	}

	private void OnCollMake(GameObject gi)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)gi).get_name().Contains("Explosion") && (Object)(object)explosion == (Object)null)
		{
			explosion = Object.Instantiate<GameObject>(gi);
			explosion.SetActive(false);
			explosion.get_transform().set_localScale(0.2f * explosion.get_transform().get_localScale());
			Object.DontDestroyOnLoad((Object)(object)explosion);
		}
		if (!Contains(gi) && (Object)(object)gi.GetComponent<HealthManager>() != (Object)null && ((Object)gi).get_name() != "Ordeal Zoteling" && firstframe > wt + 25)
		{
			if ((((Behaviour)gi.GetComponent<HealthManager>()).get_enabled() && !gi.GetComponent<HealthManager>().CheckInvincible() && gi.get_activeInHierarchy()) || ((Object)gi).get_name().Contains("Grey Prince") || ((Object)gi).get_name().Contains("Mega Jellyfish"))
			{
				Clone(gi, 1);
			}
			else
			{
				deadones.Add(gi);
			}
		}
	}

	private IEnumerator waitawhile(float t, int type)
	{
		yield return (object)new WaitForSeconds(t);
		switch (type)
		{
		case 0:
			cansummon = true;
			break;
		case 1:
			cansummon2 = true;
			break;
		case 2:
			cansummon3 = true;
			break;
		case 3:
			cansummon4 = true;
			break;
		}
	}

	private IEnumerator Reload(Vector3 pos)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		yield return (object)new WaitForSeconds(3f);
		SceneManager.LoadScene(build);
		((Loggable)this).Log("Reloading");
		yield return (object)new WaitForEndOfFrame();
		yield return (object)new WaitForEndOfFrame();
		yield return (object)new WaitForEndOfFrame();
		yield return (object)new WaitForEndOfFrame();
		((Component)HeroController.get_instance()).get_transform().set_position(pos);
	}

	private IEnumerator MegaReference(int b, string name, bool gp)
	{
		SceneManager.LoadScene(b);
		GodSoul godSoul = this;
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(b);
		((Loggable)godSoul).Log(((Scene)(ref sceneByBuildIndex)).get_name());
		if (name == "Electric Mage New")
		{
			for (int j = 0; j < 6; j++)
			{
				yield return (object)new WaitForEndOfFrame();
			}
		}
		else
		{
			for (int i = 0; i < 2; i++)
			{
				yield return (object)new WaitForEndOfFrame();
			}
		}
		if (gp)
		{
			annoying = Object.Instantiate<GameObject>(((Component)GameObject.Find(name).get_transform().get_parent()).get_gameObject(), new Vector3(1000f, 1000f), GameObject.Find(name).get_transform().get_parent()
				.get_rotation());
		}
		else
		{
			annoying = Object.Instantiate<GameObject>(GameObject.Find(name), new Vector3(1000f, 1000f), GameObject.Find(name).get_transform().get_rotation());
		}
		Object.DontDestroyOnLoad((Object)(object)annoying);
		annoying.SetActive(false);
		((Loggable)this).Log((object)annoying);
	}

	private void OnHeroUpdate()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Expected O, but got Unknown
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_0948: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f70: Unknown result type (might be due to invalid IL or missing references)
		//IL_110a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1114: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13be: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1409: Unknown result type (might be due to invalid IL or missing references)
		//IL_1455: Unknown result type (might be due to invalid IL or missing references)
		//IL_1465: Unknown result type (might be due to invalid IL or missing references)
		//IL_148f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1494: Unknown result type (might be due to invalid IL or missing references)
		//IL_1512: Unknown result type (might be due to invalid IL or missing references)
		//IL_1523: Unknown result type (might be due to invalid IL or missing references)
		//IL_1696: Unknown result type (might be due to invalid IL or missing references)
		//IL_169b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16af: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1701: Unknown result type (might be due to invalid IL or missing references)
		//IL_1715: Unknown result type (might be due to invalid IL or missing references)
		//IL_178a: Unknown result type (might be due to invalid IL or missing references)
		//IL_179e: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1807: Unknown result type (might be due to invalid IL or missing references)
		//IL_180c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1832: Unknown result type (might be due to invalid IL or missing references)
		//IL_1856: Unknown result type (might be due to invalid IL or missing references)
		//IL_185d: Expected O, but got Unknown
		//IL_186e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1896: Unknown result type (might be due to invalid IL or missing references)
		//IL_18cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1903: Unknown result type (might be due to invalid IL or missing references)
		//IL_190c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1911: Unknown result type (might be due to invalid IL or missing references)
		//IL_1928: Unknown result type (might be due to invalid IL or missing references)
		//IL_192d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1939: Unknown result type (might be due to invalid IL or missing references)
		//IL_193e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1949: Unknown result type (might be due to invalid IL or missing references)
		//IL_195b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1975: Unknown result type (might be due to invalid IL or missing references)
		//IL_197c: Unknown result type (might be due to invalid IL or missing references)
		//IL_198e: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b71: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bca: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf0: Unknown result type (might be due to invalid IL or missing references)
		firstframe++;
		Scene val;
		if (firstframe == wt)
		{
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(forever());
			pos = ((Component)HeroController.get_instance()).get_transform().get_position();
			val = SceneManager.GetActiveScene();
			homescene = ((Scene)(ref val)).get_buildIndex();
			SceneManager.LoadScene(423);
			val = SceneManager.GetSceneByBuildIndex(423);
			((Loggable)this).Log(((Scene)(ref val)).get_name());
		}
		if (firstframe == wt + 4)
		{
			megalight = Object.Instantiate<GameObject>(GameObject.Find("Chest (3)"), new Vector3(1000f, 1000f), GameObject.Find("Chest (3)").get_transform().get_rotation());
			Object.DontDestroyOnLoad((Object)(object)megalight);
			infected = PlayerData.get_instance().crossroadsInfected;
			PlayerData.get_instance().crossroadsInfected = true;
			SceneManager.LoadScene(37);
		}
		if (firstframe == wt + 8)
		{
			GameObject.Find("Bursting Zombie").GetComponent<HealthManager>().Die((float?)1f, (AttackTypes)0, true);
			PlayerData.get_instance().crossroadsInfected = infected;
		}
		if (firstframe == wt + 12)
		{
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(147, "Plant Trap", gp: false));
		}
		if (firstframe == wt + 16)
		{
			annoying.GetComponent<HealthManager>().hp = 200;
			plant = annoying;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(310, "Giant Hopper (1)", gp: false));
		}
		if (firstframe == wt + 20)
		{
			bighop = annoying;
			smolhop = Object.Instantiate<GameObject>(GameObject.Find("Hopper"), new Vector3(1000f, 1000f, 0f), default(Quaternion));
			smolhop.SetActive(false);
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(328, "Ruins Flying Sentry Javelin", gp: false));
		}
		if (firstframe == wt + 14)
		{
		}
		if (firstframe == wt + 24)
		{
			sentry = annoying;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(202, "Mantis Heavy Flyer", gp: false));
		}
		if (firstframe == wt + 28)
		{
			mantisshot = Object.Instantiate<GameObject>(FsmUtil.GetAction<SpawnObjectFromGlobalPool>(FSMUtility.LocateMyFSM(annoying, "Heavy Flyer"), "Shoot", 4).gameObject.get_Value());
			mantisshot.SetActive(false);
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(120, "ruind_bridge_roof_04_spikes", gp: false));
		}
		if (firstframe == wt + 32)
		{
			spikes = annoying;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(407, "Radiant Beam", gp: false));
		}
		if (firstframe == wt + 36)
		{
			beam = annoying;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(466, "Mega Jellyfish GG", gp: false));
		}
		if (firstframe == wt + 44)
		{
			shock = Object.Instantiate<GameObject>(FsmUtil.GetAction<SpawnObjectFromGlobalPool>(FSMUtility.LocateMyFSM(annoying, "Mega Jellyfish"), "Gen", 2).gameObject.get_Value());
			shock.SetActive(false);
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(370, "wp_saw", gp: false));
		}
		if (firstframe == wt + 48)
		{
			saw = annoying;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MegaReference(167, "Zombie Fungus B", gp: false));
		}
		if (firstframe == wt + 52)
		{
			Fungfly = Object.Instantiate<GameObject>(GameObject.Find("Fungus Flyer"), new Vector3(1000f, 1000f), default(Quaternion));
			Fungfly.SetActive(false);
			SceneManager.LoadScene(2);
		}
		framecount += Time.get_deltaTime();
		if (firstframe <= wt + 55)
		{
			return;
		}
		if (!Object.op_Implicit((Object)(object)killme))
		{
			killme = new GameObject();
		}
		string sceneplace = Sceneplace;
		val = SceneManager.GetActiveScene();
		if (sceneplace != ((Scene)(ref val)).get_name())
		{
			val = SceneManager.GetActiveScene();
			Sceneplace = ((Scene)(ref val)).get_name();
			val = SceneManager.GetActiveScene();
			build = ((Scene)(ref val)).get_buildIndex();
			if (build == 389)
			{
				GameObject val2 = Object.Instantiate<GameObject>(spikes, new Vector3(69f, 26.5f, -1f), spikes.get_transform().get_rotation());
				Transform transform = val2.get_transform();
				transform.set_eulerAngles(transform.get_eulerAngles() + new Vector3(0f, 0f, 5f));
				Transform transform2 = val2.get_transform();
				transform2.set_localScale(Vector2.op_Implicit(Vector2.op_Implicit(transform2.get_localScale()) * new Vector2(6f, 1f)));
				val2.GetComponent<DamageHero>().hazardType = 1;
				val2.SetActive(true);
			}
			if (build == 186)
			{
				GameObject val3 = Object.Instantiate<GameObject>(Fungfly, new Vector3(39f, 17.4f, 0f), Fungfly.get_transform().get_rotation());
				val3.SetActive(true);
				GameObject val4 = Object.Instantiate<GameObject>(Fungfly, new Vector3(62f, 32.5f, 0f), Fungfly.get_transform().get_rotation());
				val4.SetActive(true);
				GameObject val5 = Object.Instantiate<GameObject>(Fungfly, new Vector3(83.3f, 21.3f, 0f), Fungfly.get_transform().get_rotation());
				val5.SetActive(true);
				GameObject val6 = Object.Instantiate<GameObject>(sentry, new Vector3(119.4f, 17.2f, 0f), sentry.get_transform().get_rotation());
				val6.SetActive(true);
			}
			if (build == 187)
			{
				GameObject val7 = Object.Instantiate<GameObject>(Fungfly, new Vector3(24f, 64.3f, 0f), Fungfly.get_transform().get_rotation());
				val7.SetActive(true);
				GameObject val8 = Object.Instantiate<GameObject>(Fungfly, new Vector3(41f, 41f, 0f), Fungfly.get_transform().get_rotation());
				val8.SetActive(true);
			}
			if (build == 113)
			{
				blockersaw = Object.Instantiate<GameObject>(saw, new Vector3(27f, 96.5f, 0f), saw.get_transform().get_rotation());
				blockersaw.SetActive(true);
			}
			if (build == 148)
			{
				blockersaw = Object.Instantiate<GameObject>(saw, new Vector3(11f, 3f, 0f), saw.get_transform().get_rotation());
				blockersaw.SetActive(true);
			}
			if (build == 149)
			{
				blockersaw = Object.Instantiate<GameObject>(saw, new Vector3(68.6f, 10.4f, 0f), saw.get_transform().get_rotation());
				blockersaw.SetActive(true);
			}
			if (build == 165)
			{
				GameObject val9 = Object.Instantiate<GameObject>(smolhop, new Vector3(23f, 27f, 0f), saw.get_transform().get_rotation());
				val9.SetActive(true);
			}
			if (build == 327)
			{
				GameObject val10 = Object.Instantiate<GameObject>(sentry, new Vector3(9f, 110f, 0f), sentry.get_transform().get_rotation());
				val10.SetActive(true);
				GameObject val11 = Object.Instantiate<GameObject>(sentry, new Vector3(11.2f, 82f, 0f), sentry.get_transform().get_rotation());
				val11.SetActive(true);
				GameObject val12 = Object.Instantiate<GameObject>(sentry, new Vector3(22f, 69f, 0f), sentry.get_transform().get_rotation());
				val12.SetActive(true);
				GameObject val13 = Object.Instantiate<GameObject>(sentry, new Vector3(16f, 33f, 0f), sentry.get_transform().get_rotation());
				val13.SetActive(true);
			}
			mantisroom = false;
			jellyroom = false;
			radiantroom = false;
			sanctumroom = false;
			finalroom = false;
			rotatenum = 0f;
			deadones = new List<GameObject>();
			dupedenemies = new List<Enemy>();
			ghosts = new List<HealthManager>();
			radiances = new List<PlayMakerFSM>();
			FightingGhost = false;
			val = SceneManager.GetActiveScene();
			string name = ((Scene)(ref val)).get_name();
			val = SceneManager.GetActiveScene();
			((Loggable)this).Log(name + " -- " + ((Scene)(ref val)).get_buildIndex());
			framecount = 0f;
			GameObject[] array = Object.FindObjectsOfType<GameObject>();
			if (array != null)
			{
				GameObject[] array2 = array;
				foreach (GameObject val14 in array2)
				{
					if (((Object)val14).get_name().Contains("Ruins") && ((Object)val14).get_name().Contains("Vial"))
					{
						sanctumroom = true;
					}
					if (Object.op_Implicit((Object)(object)val14.GetComponent<DropPlatform>()))
					{
						val14.AddComponent<DropPlat>();
						DropPlat component = val14.GetComponent<DropPlat>();
						DropPlatform component2 = val14.GetComponent<DropPlatform>();
						component.flipUpSound = component2.flipUpSound;
						component.collider = component2.collider;
						component.dropAnim = component2.dropAnim;
						component.dropSound = component2.dropSound;
						component.idleAnim = component2.idleAnim;
						component.spriteAnimator = component2.spriteAnimator;
						component.strikeEffect = component2.strikeEffect;
						component.landSound = component2.landSound;
						component.raiseAnim = component2.raiseAnim;
						component.waittime = 0.05f;
						Object.Destroy((Object)(object)component2);
					}
					if (((Object)val14).get_name().Contains("Fungoon Baby") && !Object.op_Implicit((Object)(object)Fungbab))
					{
						Fungbab = Object.Instantiate<GameObject>(val14);
						((Object)Fungbab).set_name("Fungbab");
						Fungbab.GetComponent<HealthManager>().SetGeoSmall(0);
						Fungbab.SetActive(false);
					}
					if (((Object)val14).get_name().Contains("Mines Platform"))
					{
						val14.get_transform().set_eulerAngles(new Vector3(0f, 0f, 180f));
						FlipPlatform component3 = val14.GetComponent<FlipPlatform>();
						component3.topSpikes.SetActive(true);
						component3.bottomSpikes.SetActive(false);
						component3.topSpikes.get_transform().set_eulerAngles(new Vector3(0f, 0f, 180f));
						slashsymbol = val14.GetComponent<FlipPlatform>().nailStrikePrefab;
						slashsound = val14.GetComponent<FlipPlatform>().hitSound;
						Object.Destroy((Object)(object)val14.GetComponent<FlipPlatform>());
					}
					if (((Object)val14).get_name().Contains("abyss_plat") || ((Object)val14).get_name().Contains("lighthouse_0"))
					{
						val = SceneManager.GetActiveScene();
						if (((Scene)(ref val)).get_buildIndex() == 336)
						{
							Object.Destroy((Object)(object)val14);
						}
					}
					if (((Object)val14).get_name().Contains("Laser Turret"))
					{
						((MonoBehaviour)GameManager.get_instance()).StartCoroutine(staggerspawn(val14, 5, 0f, 50f));
					}
					if (((Object)val14).get_name().Contains("Jelly"))
					{
						jellyroom = true;
					}
					if (((Object)val14).get_name().Contains("White Palace Fly"))
					{
						FsmUtil.GetAction<iTweenMoveTo>(FSMUtility.LocateMyFSM(val14, "Control"), "Wait", 0).time = FsmFloat.op_Implicit(7f);
						tk2dSpriteAnimationClip[] clips = val14.GetComponent<tk2dSpriteAnimator>().get_Library().clips;
						foreach (tk2dSpriteAnimationClip val15 in clips)
						{
							if (val15.name == "Wound")
							{
								val15.fps *= 0.14f;
							}
						}
					}
					if (((Object)val14).get_name().Contains("wp_saw"))
					{
						Transform transform3 = val14.get_transform();
						transform3.set_localScale(transform3.get_localScale() * 1.1f);
					}
					if (((Object)val14).get_name().Contains("Crystallised Lazer Bug"))
					{
						PlayMakerFSM val16 = FSMUtility.LocateMyFSM(val14, "Laser Bug");
						FsmUtil.GetAction<WaitRandom>(val16, "Idle", 0).timeMin = FsmFloat.op_Implicit(0.2f);
						FsmUtil.GetAction<WaitRandom>(val16, "Idle", 0).timeMax = FsmFloat.op_Implicit(0.2f);
						FsmUtil.GetAction<Wait>(val16, "Beam", 0).time = FsmFloat.op_Implicit(0.3f);
						tk2dSpriteAnimationClip[] clips2 = val14.GetComponent<tk2dSpriteAnimator>().get_Library().clips;
						foreach (tk2dSpriteAnimationClip val17 in clips2)
						{
							if (val17.name == "Ball Antic")
							{
								val17.fps *= 5f;
							}
						}
					}
					if (((Object)val14).get_name().Contains("Chest") && build != 447 && build != 193 && build != 391)
					{
						GameObject val18 = Object.Instantiate<GameObject>(megalight, val14.get_transform().get_position(), val14.get_transform().get_rotation());
						val18.SetActive(true);
						val14.SetActive(false);
						PlayMakerFSM val19 = FSMUtility.LocateMyFSM(val18, "Chest Control");
						((Loggable)this).Log(val19.get_ActiveStateName());
						val19.get_FsmVariables().GetFsmBool("Activated").set_Value(false);
						val19.get_FsmVariables().GetFsmBool("Geo Chest").set_Value(true);
						val19.SetState("Idle");
						((Loggable)this).Log("CHest Active: " + val19.get_FsmVariables().GetFsmBool("Activated").get_Value());
						((Loggable)this).Log("CHest Geo: " + val19.get_FsmVariables().GetFsmBool("Geo Chest").get_Value());
						((Loggable)this).Log("CHest Hero Range: " + val19.get_FsmVariables().GetFsmBool("Hero Range").get_Value());
						((MonoBehaviour)GameManager.get_instance()).StartCoroutine(ChestActivate(val19));
					}
					if (((Object)val14).get_name().Contains("Zap Cloud"))
					{
						tk2dSpriteAnimationClip[] clips3 = val14.GetComponent<tk2dSpriteAnimator>().get_Library().clips;
						foreach (tk2dSpriteAnimationClip val20 in clips3)
						{
							if (val20.name == "Idle")
							{
								val20.fps *= 2f;
							}
						}
						Transform transform4 = val14.get_transform();
						transform4.set_localScale(transform4.get_localScale() * 1.2f);
						PlayMakerFSM val21 = FSMUtility.LocateMyFSM(val14, "zap control");
						FsmUtil.GetAction<FloatAdd>(val21, "Ready", 3).add = FsmFloat.op_Implicit(20f);
					}
					if (!Contains(val14) && (Object)(object)val14.GetComponent<HealthManager>() != (Object)null && !((Object)val14).get_name().Contains("Mega Fat Bee") && !((Object)val14).get_name().Contains("Hornet") && !((Object)val14).get_name().Contains("Traitor Lord") && !((Object)val14).get_name().Contains("Grimm") && !((Object)val14).get_name().Contains("Royal Gaurd"))
					{
						if ((((Behaviour)val14.GetComponent<HealthManager>()).get_enabled() && !val14.GetComponent<HealthManager>().CheckInvincible()) || ((Object)val14).get_name().Contains("Crystal") || ((Object)val14).get_name().Contains("Crawler") || ((Object)val14).get_name().Contains("Moss") || ((Object)val14).get_name().Contains("Grey Prince") || (((Object)val14).get_name().Contains("Mawlek Body") && build == 45) || ((Object)val14).get_name().Contains("Mega Jellyfish"))
						{
							Clone(val14, 0);
						}
						else if (!deadones.Contains(val14))
						{
							deadones.Add(val14);
							((Loggable)this).Log(((Object)val14).get_name() + " added to deadones");
						}
					}
				}
				val = SceneManager.GetActiveScene();
				if (((Scene)(ref val)).get_buildIndex() == 227)
				{
					jellyroom = true;
				}
				try
				{
					if (int.Parse(Sceneplace.Substring(8)) > 3 && Sceneplace.Contains("Fungus3") && !PlayerData.get_instance().defeatedMantisLords)
					{
						mantisroom = true;
					}
				}
				catch
				{
				}
				if (jellyroom)
				{
					mantisroom = false;
				}
			}
		}
		foreach (Enemy dupedenemy in dupedenemies)
		{
			if (!Object.op_Implicit((Object)(object)dupedenemy.enemy))
			{
				dupedenemies.Remove(dupedenemy);
				val = SceneManager.GetActiveScene();
				if (((Scene)(ref val)).get_name().Contains("Crossroads"))
				{
					val = SceneManager.GetActiveScene();
					if (((Scene)(ref val)).get_name() == Sceneplace && !dupedenemy.name.Contains("Hollow Shade"))
					{
						GameObject val22 = Object.Instantiate<GameObject>(explosion, dupedenemy.lastpos, explosion.get_transform().get_rotation());
						val22.SetActive(true);
					}
				}
				if (dupedenemy.name.Contains("Fungoon Baby") || dupedenemy.name.Contains("Fungbab"))
				{
					GameObject val23 = Object.Instantiate<GameObject>(Fungbab, dupedenemy.lastpos, Fungbab.get_transform().get_rotation());
					val23.SetActive(true);
				}
				break;
			}
			dupedenemy.lastpos = dupedenemy.enemy.get_transform().get_position();
			if (jellydeadly && ((Object)dupedenemy.enemy).get_name().Contains("Jellyfish") && !((Object)dupedenemy.enemy).get_name().Contains("Baby") && !((Object)dupedenemy.enemy).get_name().Contains("Mega") && Vector3.Distance(((Component)HeroController.get_instance()).get_gameObject().get_transform().get_position(), dupedenemy.enemy.get_transform().get_position()) < 5f && (!PlayerData.get_instance().hasXunFlower || PlayerData.get_instance().xunFlowerBroken))
			{
				dupedenemy.enemy.GetComponent<HealthManager>().Die((float?)((float)rand.NextDouble() * 360f), (AttackTypes)0, true);
			}
		}
		if (deadones.Count > 0 && deadones != null)
		{
			foreach (GameObject deadone in deadones)
			{
				if (!Object.op_Implicit((Object)(object)deadone))
				{
					deadones.Remove(deadone);
					continue;
				}
				HealthManager component4 = deadone.GetComponent<HealthManager>();
				if (((Behaviour)component4).get_enabled() && !component4.CheckInvincible() && deadone.get_activeInHierarchy())
				{
					deadones.Remove(deadone);
					Clone(deadone, 3);
				}
			}
		}
		if (cansummon && !jellyroom && HeroController.get_instance().CheckTouchingGround() && Object.FindObjectsOfType<PromptMarker>().Length == 0 && HeroController.get_instance().CanInput())
		{
			val = SceneManager.GetActiveScene();
			if (!((Scene)(ref val)).get_name().Contains("Fungus1"))
			{
				val = SceneManager.GetActiveScene();
				if (!((Scene)(ref val)).get_name().Contains("Fungus3"))
				{
					goto IL_1749;
				}
			}
			GameObject val24 = Object.Instantiate<GameObject>(plant, ((Component)HeroController.get_instance()).get_gameObject().get_transform().get_position() + new Vector3(0f, 1f, 0f), Quaternion.Euler(0f, 0f, 0f));
			val24.SetActive(true);
			cansummon = false;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(waitawhile(0.75f, 0));
		}
		goto IL_1749;
		IL_1749:
		if (cansummon4 && Object.FindObjectsOfType<PromptMarker>().Length == 0 && HeroController.get_instance().CanInput() && jellyroom)
		{
			GameObject val25 = Object.Instantiate<GameObject>(shock, ((Component)HeroController.get_instance()).get_gameObject().get_transform().get_position() + new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
			val25.SetActive(true);
			cansummon4 = false;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(waitawhile(0.5f, 3));
		}
		if (cansummon3 && Object.FindObjectsOfType<PromptMarker>().Length == 0 && HeroController.get_instance().CanInput())
		{
			val = SceneManager.GetActiveScene();
			if (((Scene)(ref val)).get_name().Contains("White_Palace") || (radiantroom && ((Component)HeroController.get_instance()).get_transform().get_position().y > 58f))
			{
				GameObject val26 = new GameObject();
				val26.get_transform().set_position(((Component)HeroController.get_instance()).get_transform().get_position());
				val26.get_transform().set_eulerAngles(new Vector3(0f, 0f, 180f + rotatenum));
				GameObject val27 = Object.Instantiate<GameObject>(beam, val26.get_transform());
				val27.get_transform().set_localPosition(new Vector3(0f, -30f, 0f));
				val27.get_transform().set_localScale(new Vector3(60f, 2f, 1f));
				Vector3 eulerAngles = val27.get_transform().get_eulerAngles();
				Vector3 position = val27.get_transform().get_position();
				val27.get_transform().set_parent((Transform)null);
				Vector3 position2 = val27.get_transform().get_position();
				Vector3 position3 = ((Component)HeroController.get_instance()).get_transform().get_position();
				position3.x -= position2.x;
				position3.y -= position2.y;
				val27.get_transform().set_eulerAngles(new Vector3(0f, 0f, Mathf.Atan2(position3.y, position3.x) * 57.29578f));
				val27.get_transform().set_position(new Vector3(position.x, position.y, 0f));
				val27.SetActive(true);
				EMT eMT = new EMT();
				eMT.name = "ANTIC";
				EMT eMT2 = new EMT();
				eMT2.name = "FIRE";
				eMT2.waittime = 0.7f;
				EMT eMT3 = new EMT();
				eMT3.waittime = 0.2f;
				eMT3.name = "END";
				((MonoBehaviour)GameManager.get_instance()).StartCoroutine(beamevent(val27.GetComponent<PlayMakerFSM>(), new EMT[3] { eMT, eMT2, eMT3 }));
				cansummon3 = false;
				if (radiantroom)
				{
					((MonoBehaviour)GameManager.get_instance()).StartCoroutine(waitawhile(0.8f, 2));
					rotatenum += 30f;
				}
				else
				{
					((MonoBehaviour)GameManager.get_instance()).StartCoroutine(waitawhile(0.75f, 2));
				}
			}
		}
		if (Sceneplace.Length > 7 && cansummon2 && mantisroom && Object.FindObjectsOfType<PromptMarker>().Length == 0 && HeroController.get_instance().CanInput() && (!PlayerData.get_instance().hasXunFlower || PlayerData.get_instance().xunFlowerBroken))
		{
			int num = rand.Next(2) * 2 - 1;
			float num2 = rand.Next(2);
			GameObject val28 = Object.Instantiate<GameObject>(mantisshot, ((Component)HeroController.get_instance()).get_gameObject().get_transform().get_position() + new Vector3(((float)rand.NextDouble() + 25f) * (float)num, ((float)rand.NextDouble() + 15f) * num2, 0f), Quaternion.Euler(0f, 0f, 0f));
			val28.SetActive(true);
			Rigidbody2D component5 = val28.GetComponent<Rigidbody2D>();
			component5.set_velocity(Vector2.op_Implicit(((Component)HeroController.get_instance()).get_gameObject().get_transform().get_position() - val28.get_transform().get_position()));
			cansummon2 = false;
			((MonoBehaviour)GameManager.get_instance()).StartCoroutine(waitawhile(1.2f, 1));
		}
		if (Object.op_Implicit((Object)(object)blockersaw) && PlayerData.get_instance().zoteRescuedBuzzer && build != 113)
		{
			Object.Destroy((Object)(object)blockersaw);
		}
		if (!PlayerData.get_instance().equippedCharms.Contains(6))
		{
			PlayerData.get_instance().equippedCharms.Add(6);
		}
		if (Object.FindObjectOfType<CameraTarget>().quaking)
		{
			((Loggable)this).Log("Quake");
			HeroController.get_instance().ClearMP();
		}
		if (FightingGhost)
		{
			foreach (HealthManager ghost in ghosts)
			{
				if (ghost.hp < 1)
				{
					((Loggable)this).Log("Ghost Dead");
					FightingGhost = false;
					ghosts = new List<HealthManager>();
					((MonoBehaviour)GameManager.get_instance()).StartCoroutine(Reload(((Component)HeroController.get_instance()).get_transform().get_position()));
				}
			}
		}
		if (radiances.Count <= 0)
		{
			return;
		}
		foreach (PlayMakerFSM radiance in radiances)
		{
			if (radiance.get_ActiveStateName() == "Abyss Up")
			{
				radiance.SendEvent("PHASE2 RISE");
			}
		}
	}

	private IEnumerator beamevent(PlayMakerFSM f, EMT[] emt)
	{
		foreach (EMT e in emt)
		{
			yield return (object)new WaitForSeconds(e.waittime);
			f.SendEvent(e.name);
		}
	}

	private IEnumerator staggerspawn(GameObject g, int n, float p, float r)
	{
		for (int i = 0; i < n; i++)
		{
			Vector3 val = g.get_transform().get_position() + new Vector3(((float)rand.NextDouble() - 0.5f) * p, ((float)rand.NextDouble() - 0.5f) * p, 0f);
			Quaternion rotation = g.get_transform().get_rotation();
			GameObject l2 = Object.Instantiate<GameObject>(g, val, Quaternion.Euler(((Quaternion)(ref rotation)).get_eulerAngles() + new Vector3(0f, 0f, ((float)rand.NextDouble() - 0.5f) * r)));
			l2.SetActive(true);
			yield return (object)new WaitForSeconds((float)rand.NextDouble() * 0.5f);
		}
	}

	public void Clone(GameObject gi, int type)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
		((Loggable)this).Log(((Object)gi).get_name() + " /\\/\\ " + type);
		if ((type < 1 && ((Object)gi).get_name().Contains("Hive Knight")) || (type == 3 && ((Object)gi).get_name().Contains("Grey Prince")))
		{
			return;
		}
		if (Sceneplace.Contains("Ruins") && !((Object)gi).get_name().Contains("Flying") && !sanctumroom)
		{
			GameObject val = Object.Instantiate<GameObject>(sentry, gi.get_transform().get_position(), gi.get_transform().get_rotation());
			val.SetActive(true);
		}
		if ((Object)(object)dmghero == (Object)null && Object.op_Implicit((Object)(object)gi.GetComponent<DamageHero>()))
		{
			dmghero = gi.GetComponent<DamageHero>();
		}
		int num = 1;
		float num2 = 1f;
		float num3 = 0f;
		if (((Object)gi).get_name().Contains("Mantis"))
		{
			if (gi.GetComponent<HealthManager>().hp < 50)
			{
				num = 3;
			}
			if (((Object)gi).get_name().Contains("Heavy") || !PlayerData.get_instance().defeatedMantisLords || build > 400)
			{
				mantisroom = true;
			}
		}
		if (((Object)gi).get_name().Contains("Royal Gaurd"))
		{
			num = 3;
		}
		if (((Object)gi).get_name().Contains("Moss"))
		{
			num = 5;
			if (((Object)gi).get_name().Contains("Flyer"))
			{
				num2 = 10f;
				num3 = 10f;
			}
		}
		if (((Object)gi).get_name().Contains("Mage") && !((Object)gi).get_name().Contains("Lord"))
		{
			num = 3;
		}
		if (((Object)gi).get_name().Contains("White Palace Fly"))
		{
			num = 0;
			gi.GetComponent<HealthManager>().hp = 0;
		}
		if (((Object)gi).get_name().Contains("Mosquito"))
		{
			num = 5;
		}
		if (((Object)gi).get_name().Contains("Mushroom"))
		{
			num2 = 18f;
			num = 5;
			if (((Object)gi).get_name().Contains("Turret"))
			{
				gi.GetComponent<HealthManager>().hp = 99999;
			}
			if (((Object)gi).get_name().Contains("Brawler"))
			{
				num2 = 2f;
			}
			if (((Object)gi).get_name().Contains("Baby"))
			{
				HealthManager component = gi.GetComponent<HealthManager>();
				component.hp *= 2;
				DamageHero val2 = gi.AddComponent<DamageHero>();
				val2 = dmghero;
				val2.damageDealt = 1;
				val2.hazardType = 1;
			}
		}
		if (((Object)gi).get_name().Contains("Fung"))
		{
			num = 10;
			if (((Object)gi).get_name().Contains("Baby"))
			{
				num = 0;
				gi.GetComponent<HealthManager>().hp = 2;
			}
			else
			{
				num = 10;
				gi.GetComponent<HealthManager>().hp = 5;
				num2 = 10f;
			}
		}
		if (((Object)gi).get_name().Contains("Crawler") || ((Object)gi).get_name().Contains("Climber"))
		{
			num = 5;
			num2 = 10f;
		}
		if (((Object)gi).get_name().Contains("Fungbab"))
		{
			gi.GetComponent<HealthManager>().hp = 1;
			num = 1;
			num2 = 4f;
			num3 = 4f;
		}
		if (((Object)gi).get_name().Contains("Bee") && !((Object)gi).get_name().Contains("Fat"))
		{
			num = 4;
		}
		if (((Object)gi).get_name().Contains("Infected Knight"))
		{
			num = 2;
		}
		if (((Object)gi).get_name().Contains("Ghost Warrior"))
		{
			if (build < 400)
			{
				FightingGhost = true;
				ghosts.Add(gi.GetComponent<HealthManager>());
			}
			if (((Object)gi).get_name().Contains("Slug"))
			{
				num = 4;
			}
			if (((Object)gi).get_name().Contains("Galien"))
			{
				GameObject val3 = Object.Instantiate<GameObject>(GameObject.Find("Galien Hammer"));
				FSMUtility.LocateMyFSM(val3, "Control").SetState("Start Spin");
				((Loggable)this).Log("Galien!!");
			}
		}
		if (((Object)gi).get_name().Contains("Radiance"))
		{
			radiances.Add(FSMUtility.LocateMyFSM(gi, "Control"));
			radiantroom = true;
		}
		if (((Object)gi).get_name().Contains("Fat Fly"))
		{
			num = 9;
		}
		if (((Object)gi).get_name().Contains("False Knight"))
		{
			num = 2;
		}
		Scene activeScene;
		if (((Object)gi).get_name().Contains("Buzzer") && !((Object)gi).get_name().Contains("Giant"))
		{
			num = 5;
			activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).get_name().Contains("Crossroads"))
			{
				gi.GetComponent<HealthManager>().hp = 1;
			}
		}
		if (((Object)gi).get_name().Contains("Sentry"))
		{
			num = 5;
			num2 = 8f;
			if (((Object)gi).get_name().Contains("Flying"))
			{
				num2 = 1f;
			}
		}
		if (((Object)gi).get_name().Contains("Head") || ((Object)gi).get_name().Contains("Zote Boss"))
		{
			return;
		}
		if (((Object)gi).get_name().Contains("Crystallised Lazer Bug"))
		{
			num = 3;
			num2 = 8f;
		}
		if (((Object)gi).get_name().Contains("Crystal Flyer"))
		{
			num = 4;
			num2 = 4f;
		}
		if (((Object)gi).get_name().Contains("Hornet") && build != 132)
		{
			num = 2;
		}
		if (((Object)gi).get_name().Contains("Inflater"))
		{
			num = 4;
		}
		if (((Object)gi).get_name().Contains("Spitter"))
		{
			num = 4;
		}
		if (((Object)gi).get_name().Contains("Hopper") && !((Object)gi).get_name().Contains("Giant") && !Sceneplace.ToLower().Contains("ruins") && type != 1)
		{
			activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).get_buildIndex() != 165)
			{
				GameObject val4 = Object.Instantiate<GameObject>(bighop, gi.get_transform().get_position(), gi.get_transform().get_rotation());
				val4.SetActive(true);
				Object.Destroy((Object)(object)gi);
				return;
			}
			num = 10;
			num2 = 7f;
		}
		if (((Object)gi).get_name().Contains("Fluke") || ((Object)gi).get_name().Contains("Jellyfish Baby"))
		{
			gi.GetComponent<HealthManager>().SetGeoSmall(0);
			if (gi.GetComponent<HealthManager>().hp < 101)
			{
				num++;
				if (gi.GetComponent<HealthManager>().hp < 81)
				{
					num++;
					if (gi.GetComponent<HealthManager>().hp < 51)
					{
						num++;
						if (gi.GetComponent<HealthManager>().hp < 26)
						{
							num++;
							if (gi.GetComponent<HealthManager>().hp < 21)
							{
								num++;
								if (gi.GetComponent<HealthManager>().hp < 11)
								{
									num += 5;
								}
							}
						}
					}
				}
			}
		}
		if (((Object)gi).get_name().Contains("Spider") && gi.GetComponent<HealthManager>().hp < 150)
		{
			num = 6;
		}
		dupedenemies.Add(new Enemy
		{
			enemy = gi,
			lastpos = gi.get_transform().get_position(),
			name = ((Object)gi).get_name()
		});
		for (int i = 0; i < num; i++)
		{
			GameObject val5 = Object.Instantiate<GameObject>(gi, gi.get_transform().get_position() + new Vector3(((float)rand.NextDouble() - 0.5f) * num2, ((float)rand.NextDouble() - 0.5f) * num3), gi.get_transform().get_rotation());
			val5.SetActive(true);
			dupedenemies.Add(new Enemy
			{
				enemy = val5,
				lastpos = val5.get_transform().get_position(),
				name = ((Object)val5).get_name()
			});
			HealthManager component2 = val5.GetComponent<HealthManager>();
			component2.SetGeoLarge(0);
			component2.SetGeoMedium(0);
			component2.SetGeoSmall(0);
			if (((Object)val5).get_name().Contains("Ghost Warrior"))
			{
				ghosts.Add(component2);
			}
			if (((Object)gi).get_name().Contains("Mega Jellyfish"))
			{
				PlayMakerFSM val6 = FSMUtility.LocateMyFSM(val5, "Mega Jellyfish");
				val6.SetState("Wake Pause");
			}
			if (((Object)gi).get_name().Contains("Black Knight") && rand.Next(0, 5) > 3)
			{
				val5.GetComponent<HealthManager>().Die((float?)((float)rand.NextDouble() * 360f), (AttackTypes)0, true);
			}
			if (((Object)gi).get_name().Contains("Mantis Lord S"))
			{
				PlayMakerFSM val7 = FSMUtility.LocateMyFSM(val5, "Mantis Lord");
				val7.get_FsmVariables().FindFsmBool("Sub").set_Value(false);
				val7.SetState("Init");
				((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MantisActivate(val7));
			}
			if (((Object)gi).get_name().Contains("Traitor Lord"))
			{
				PlayMakerFSM fsm = FSMUtility.LocateMyFSM(val5, "Mantis");
				((MonoBehaviour)GameManager.get_instance()).StartCoroutine(TraitorActivate(fsm));
			}
			if (((Object)gi).get_name().Contains("Flamebearer"))
			{
				PlayMakerFSM val8 = FSMUtility.LocateMyFSM(val5, "Control");
				((Loggable)this).Log("Flamebearerrrr");
				val8.SendEvent("GRIMMKIN SPAWN");
				((MonoBehaviour)GameManager.get_instance()).StartCoroutine(FlameActivate(val8));
			}
			if (((Object)gi).get_name() == "Hollow Knight Boss")
			{
				finalroom = true;
				hk1 = gi;
				hk2 = val5;
			}
			if (((Object)val5).get_name().Contains("Mushroom Baby"))
			{
				DamageHero val9 = val5.AddComponent<DamageHero>();
				val9 = dmghero;
				val9.damageDealt = 1;
				val9.hazardType = 1;
			}
			if (((Object)val5).get_name().Contains("False Knight"))
			{
				PlayMakerFSM val10 = FSMUtility.LocateMyFSM(val5, "FalseyControl");
				val10.SendEvent("STUN");
			}
			if (((Object)val5).get_name().Contains("Zombie Spider"))
			{
				PlayMakerFSM val11 = FSMUtility.LocateMyFSM(val5, "Chase");
				val11.get_FsmVariables().FindFsmBool("Wait For Battle").set_Value(false);
				FsmUtil.GetAction<WaitRandom>(val11, "Wait", 0).timeMin = FsmFloat.op_Implicit(0f);
				FsmUtil.GetAction<WaitRandom>(val11, "Wait", 0).timeMax = FsmFloat.op_Implicit(1f);
			}
			if (((Object)val5).get_name().Contains("Mimic Spider"))
			{
				PlayMakerFSM val12 = FSMUtility.LocateMyFSM(val5, "Mimic Spider");
				val12.SendEvent("WAKE");
				val5.get_transform().set_position(new Vector3(97f, 7.4f));
			}
			if (((Object)gi).get_name() == "Mawlek Body" && build == 45)
			{
				PlayMakerFSM val13 = FSMUtility.LocateMyFSM(val5, "Mawlek Control");
				val13.SetState("Init");
				((MonoBehaviour)GameManager.get_instance()).StartCoroutine(MawlekActivate(val13));
			}
			if (((Object)gi).get_name() == "Nightmare Grimm Boss")
			{
				PlayMakerFSM val14 = FSMUtility.LocateMyFSM(val5, "Control");
				val14.SetState("Init");
				((MonoBehaviour)GameManager.get_instance()).StartCoroutine(GrimmActivate(val14));
			}
			if (((Object)gi).get_name().Contains("Radiance"))
			{
				radiances.Add(FSMUtility.LocateMyFSM(val5, "Control"));
			}
		}
	}

	private IEnumerator GrimmActivate(PlayMakerFSM fsm)
	{
		yield return (object)new WaitForEndOfFrame();
		fsm.SendEvent("WAKE");
	}

	private IEnumerator FlameActivate(PlayMakerFSM fsm)
	{
		yield return (object)new WaitForEndOfFrame();
		fsm.SendEvent("START");
	}

	private IEnumerator MantisActivate(PlayMakerFSM fsm)
	{
		yield return (object)new WaitForEndOfFrame();
		fsm.SendEvent("MLORD START MAIN");
	}

	private IEnumerator TraitorActivate(PlayMakerFSM fsm)
	{
		yield return (object)new WaitForEndOfFrame();
		((Loggable)this).Log(fsm.get_ActiveStateName());
		fsm.SetState("Active");
	}

	private IEnumerator MawlekActivate(PlayMakerFSM fsm)
	{
		yield return (object)new WaitForEndOfFrame();
		fsm.SendEvent("WAKE");
		((Loggable)this).Log(fsm.get_ActiveStateName());
	}

	private IEnumerator ChestActivate(PlayMakerFSM fsm)
	{
		yield return (object)new WaitForEndOfFrame();
		fsm.SendEvent("FINISHED");
		((Loggable)this).Log(fsm.get_ActiveStateName());
	}

	private string LanguageGet(string key, string sheetTitle, string orig)
	{
		if (orig == "Nosk")
		{
			return "The Imposter";
		}
		object result;
		switch (key)
		{
		case "SENSE_TAB_01":
		{
			string text = web.DownloadString("https://github.com/Nexade/TabletString/blob/main/Text");
			text = text.Substring(text.IndexOf("AAAAA") + 5, text.IndexOf("BBBBB") - text.IndexOf("AAAAA") - 5);
			return text + "<page> Message Nexade #4250 on discord with proof that you've beaten the Radiance to be added";
		}
		case "TUT_TAB_03":
			return "Check your inventory";
		case "ATRIUM_ENTER_TEXT":
			return "So you think you can do the Pantheons?<br><page>That's kinda.........<page>.....<page>cringe";
		case "CHARM_DESC_19":
			return "I've brought this down from six to five notches. You're welcome";
		case "TUT_TAB_01":
			return "Healing is for the weak, and this mod isn't for the weak";
		default:
			result = orig;
			break;
		case "SHOP_DESC_WAYWARDCOMPASS":
			result = "This is the best charm in the game";
			break;
		}
		return (string)result;
	}

	private HitInstance OnHit(Fsm owner, HitInstance hit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		AttackTypes attackType = hit.AttackType;
		AttackTypes val = attackType;
		if ((int)val != 0 || !_settings.LimitNail)
		{
		}
		return hit;
	}

	private int OnInt(string intName, int orig)
	{
		if (1 == 0)
		{
		}
		int result = ((intName == "charmCost_19") ? 5 : ((!(intName == "charmCost_6")) ? orig : 0));
		if (1 == 0)
		{
		}
		return result;
	}

	private int OnHealthTaken(int damage)
	{
		return damage;
	}

	private int OnSoulGain(int amount)
	{
		return amount;
	}

	public void Unload()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		ModHooks.remove_ColliderCreateHook((Action<GameObject>)OnCollMake);
		ModHooks.remove_LanguageGetHook(new LanguageGetProxy(LanguageGet));
		ModHooks.remove_TakeHealthHook(new TakeHealthProxy(OnHealthTaken));
		ModHooks.remove_SoulGainHook((Func<int, int>)OnSoulGain);
		ModHooks.remove_GetPlayerIntHook(new GetIntProxy(OnInt));
		ModHooks.remove_HitInstanceHook(new HitInstanceHandler(OnHit));
		ModHooks.remove_HeroUpdateHook((Action)OnHeroUpdate);
		ModHooks.remove_GetPlayerBoolHook(new GetBoolProxy(OnBool));
		HeroController.remove_CanFocus(new hook_CanFocus(Focus));
	}
}
