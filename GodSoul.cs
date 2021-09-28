using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Modding;
using UnityEngine;
using JetBrains.Annotations;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;
using SFCore;
using HellMod;
using UnityEngine.PlayerLoop;

namespace GodSoul
{
    [UsedImplicitly]
    public class GodSoul : Mod, ITogglableMod, IGlobalSettings<GlobalModSettings>
    {
        public GodSoul() : base("God Soul") { }

        private GlobalModSettings _settings = new();

        public void OnLoadGlobal(GlobalModSettings s) => _settings = s;

        public float framecount = 0;

        public GlobalModSettings OnSaveGlobal() => _settings;

        public List<Enemy> dupedenemies = new List<Enemy>();

        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public override void Initialize()
        {
            ModHooks.ColliderCreateHook += OnCollMake;
            ModHooks.SlashHitHook += OnSlashHit;
            ModHooks.HeroUpdateHook += OnHeroUpdate;
            ModHooks.LanguageGetHook += LanguageGet;
            ModHooks.TakeHealthHook += OnHealthTaken;
            ModHooks.SoulGainHook += OnSoulGain;
            ModHooks.GetPlayerIntHook += OnInt;
            ModHooks.HitInstanceHook += OnHit;
            ModHooks.GetPlayerBoolHook += OnBool;
            On.HeroController.CanFocus += Focus;
        }
        private bool Focus(On.HeroController.orig_CanFocus orig, HeroController self)
        {
            return (false);
        }
        /*If you are reading this... I'm sorry. My code is a mess. but it works so that's good enough for me. 
        Towards the end of the mod making process I got more and more insane, so there are lots of random things. 
        Good Luck */
        IEnumerator forever()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (saw)
                {
                    UnityEngine.Object.DontDestroyOnLoad(saw);
                }
                if (shock)
                {
                    UnityEngine.Object.DontDestroyOnLoad(shock);
                }
                if (beam)
                {
                    UnityEngine.Object.DontDestroyOnLoad(beam);
                }
                if (spikes)
                {
                    UnityEngine.Object.DontDestroyOnLoad(spikes);
                }
                if (mantisshot)
                {
                    UnityEngine.Object.DontDestroyOnLoad(mantisshot);
                }
                if (smolhop)
                {
                    UnityEngine.Object.DontDestroyOnLoad(smolhop);
                }
                if (bighop)
                {
                    UnityEngine.Object.DontDestroyOnLoad(bighop);
                }
                if (plant)
                {
                    UnityEngine.Object.DontDestroyOnLoad(plant);
                }
                if (megalight)
                {
                    UnityEngine.Object.DontDestroyOnLoad(megalight);
                }
                if (explosion)
                {
                    UnityEngine.Object.DontDestroyOnLoad(explosion);
                }
            }
        }
        private bool OnBool(string name, bool orig)
        {
            if (name == "hasDreamGate")
            {
                return (false);
            }
            if (name == "equippedCharm_6")
            {
                return (true);
            }
            if (name == "gotCharm_6")
            {
                return (true);
            }
            return (orig);
        }

        private void OnSlashHit(Collider2D otherCollider, GameObject slash)
        {
            //Log(otherCollider.gameObject.name);
            //Log(HeroController.instance.gameObject.transform.position);
            if (otherCollider.gameObject.name.Contains("Mines Plat"))
            {
                slashsymbol.Spawn(otherCollider.gameObject.transform);
                AudioSource au = HeroController.instance.gameObject.AddComponent<AudioSource>();
                au.PlayOneShot(slashsound);
            }
            if (otherCollider.gameObject.name.Contains("Thorn"))
            {
                HeroController.instance.TakeDamage(otherCollider.gameObject, GlobalEnums.CollisionSide.top, 1, 0);
            }
            foreach (MonoBehaviour m in otherCollider.GetComponents<MonoBehaviour>())
            {
                //Log(m.GetType());
                if (m.GetType().Name == "HealthManager" && !Contains(m.gameObject))
                {
                    GameObject gi = m.gameObject;
                    Clone(gi,2);
                }
            }
        }

        public bool Contains(GameObject gi)
        {
            foreach(Enemy e in dupedenemies)
            {
                if (e.enemy == gi)
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> illegalnames = new List<String>(1) { "Ordeal Zoteling" };
        private void OnCollMake(GameObject gi)
        {
            if (gi.name.Contains("Explosion") && explosion == null)
            {
                explosion = UnityEngine.Object.Instantiate(gi);
                explosion.SetActive(false);
                explosion.transform.localScale = 0.2f * explosion.transform.localScale;
                UnityEngine.Object.DontDestroyOnLoad(explosion);
            }
            if (!Contains(gi) && gi.GetComponent<HealthManager>() != null && !illegalnames.Contains(gi.name) && firstframe > wt + 25)
            {
                if ((gi.GetComponent<HealthManager>().enabled && !gi.GetComponent<HealthManager>().CheckInvincible() && gi.activeInHierarchy) || gi.name.Contains("Grey Prince"))
                {
                    Clone(gi, 1);
                }
                else
                {
                    deadones.Add(gi);
                }
            }
        }
        public bool jellydeadly = true;
        public bool jellyroom;
        public bool droppersinvis = true;
        public bool mantisroom;
        public bool sanctumroom;
        public bool radiantroom;
        public int firstframe = 0;
        public string Sceneplace = "";
        public string extra;
        public GameObject megalight;
        public List<Fsm> zaps;
        public int wt = 130;
        int homescene;
        public GameObject blockersaw;
        public bool cansummon = true;
        public bool cansummon2 = true;
        public bool cansummon3 = true;
        public bool cansummon4 = true;
        public float rotatenum;
        public GameObject slashsymbol;
        public AudioClip slashsound;
        Vector3 pos;
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
        public int round;
        public GameObject annoying;
        public bool infected;

        IEnumerator waitawhile(float t, int type)
        {
            yield return new WaitForSeconds(t);
            if (type == 0)
            {
                cansummon = true; 
                yield break;
            }
            if (type == 1)
            {
                cansummon2 = true; 
                yield break;
            }
            if (type == 2)
            {
                cansummon3 = true;
                yield break;
            }
            if (type == 3)
            {
                cansummon4 = true;
                yield break;
            }
        }

        IEnumerator MegaReference(int b, string name, bool gp)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(b); ;
            Log(UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(b).name);
            if(name == "Electric Mage New")
            {
                for (int i = 0; i < 6; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            if (gp)
            {
                annoying = UnityEngine.Object.Instantiate(GameObject.Find(name).transform.parent.gameObject, new Vector3(1000, 1000), GameObject.Find(name).transform.parent.rotation);
            }
            else
            {
                annoying = UnityEngine.Object.Instantiate(GameObject.Find(name), new Vector3(1000, 1000), GameObject.Find(name).transform.rotation);
            }
            UnityEngine.Object.DontDestroyOnLoad(annoying);
            annoying.SetActive(false);
            Log(annoying);

        }

        public GameObject killme;
        private void OnHeroUpdate()
        {
            firstframe++;
            if (firstframe == wt)
            {
                GameManager.instance.StartCoroutine(forever());
                pos = HeroController.instance.transform.position;
                homescene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
                UnityEngine.SceneManagement.SceneManager.LoadScene(423);
                Log(UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(423).name);
            }
            if (firstframe == wt + 4)
            {
                megalight = UnityEngine.Object.Instantiate(GameObject.Find("Chest (3)"), new Vector3(1000, 1000), GameObject.Find("Chest (3)").transform.rotation);
                UnityEngine.Object.DontDestroyOnLoad(megalight);
                infected = PlayerData.instance.crossroadsInfected;
                PlayerData.instance.crossroadsInfected = true;
                UnityEngine.SceneManagement.SceneManager.LoadScene(37);
            }
            if (firstframe == wt + 8)
            {
                GameObject.Find("Bursting Zombie").GetComponent<HealthManager>().Die(1, AttackTypes.Nail, true);
                PlayerData.instance.crossroadsInfected = infected;
            }
            if (firstframe == wt + 12)
            {
                GameManager.instance.StartCoroutine(MegaReference(147,"Plant Trap", false));
            }
            if (firstframe == wt + 16)
            {
                annoying.GetComponent<HealthManager>().hp = 200;
                plant = annoying;
                GameManager.instance.StartCoroutine(MegaReference(310,"Giant Hopper (1)", false));
            }
            if (firstframe == wt + 20)
            {
                bighop = annoying;
                smolhop = GameManager.Instantiate(GameObject.Find("Hopper"), new Vector3(1000, 1000, 0), new Quaternion());
                smolhop.SetActive(false);
                GameManager.instance.StartCoroutine(MegaReference(328, "Ruins Flying Sentry Javelin",false));
            }
            if (firstframe == wt + 14)
            {
                //HeroController.instance.transform.position = pos;
            }
            if (firstframe == wt + 24)
            {
                sentry = annoying;
                GameManager.instance.StartCoroutine(MegaReference(202, "Mantis Heavy Flyer", false));
            }
            if (firstframe == wt + 28)
            {
                mantisshot = UnityEngine.Object.Instantiate(annoying.LocateMyFSM("Heavy Flyer").GetAction<SpawnObjectFromGlobalPool>("Shoot", 4).gameObject.Value);
                mantisshot.SetActive(false);
                GameManager.instance.StartCoroutine(MegaReference(120, "ruind_bridge_roof_04_spikes", false));
            }
            if (firstframe == wt + 32)
            {
                spikes = annoying;
                GameManager.instance.StartCoroutine(MegaReference(407, "Radiant Beam", false));
            }
            if (firstframe == wt + 36)
            {
                beam = annoying;
                GameManager.instance.StartCoroutine(MegaReference(466, "Mega Jellyfish GG", false)); //figure out what's wrong with the path to get the attack gameobject
            }
            if (firstframe == wt + 44)
            {
                shock = UnityEngine.Object.Instantiate(annoying.LocateMyFSM("Mega Jellyfish").GetAction<SpawnObjectFromGlobalPool>("Gen", 2).gameObject.Value);
                shock.SetActive(false);
                GameManager.instance.StartCoroutine(MegaReference(370, "wp_saw", false)); //figure out what's wrong with the path to get the attack gameobject
            }
            if (firstframe == wt + 48)
            {
                saw = annoying;
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
            }
            
            framecount += UnityEngine.Time.deltaTime;
            if (firstframe > wt + 50)
            {
                if (!killme)
                {
                    killme = new GameObject();
                }
                if (Sceneplace != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                {
                    if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 389)
                    {
                        GameObject s = UnityEngine.Object.Instantiate(spikes, new Vector3(69, 26.5f, -1), spikes.transform.rotation);
                        s.transform.eulerAngles += new Vector3(0, 0, 5);
                        s.transform.localScale *= new Vector2(6, 1);
                        Log(s.GetComponent<DamageHero>().hazardType);
                        s.GetComponent<DamageHero>().hazardType = 1;
                        s.SetActive(true);
                    }
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 148)
                    {
                        blockersaw = UnityEngine.Object.Instantiate(saw, new Vector3(11, 3, 0), saw.transform.rotation);
                        blockersaw.SetActive(true);
                    }
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 165)
                    {
                        GameObject _ = UnityEngine.Object.Instantiate(smolhop, new Vector3(23, 27, 0), saw.transform.rotation);
                        _.SetActive(true);
                    }
                    mantisroom = false;
                    jellyroom = false;
                    radiantroom = false;
                    sanctumroom = false;
                    rotatenum = 0;
                    deadones = new List<GameObject>();
                    dupedenemies = new List<Enemy>();
                    Log(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " -- " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                    framecount = 0;
                    GameObject[] obj = UnityEngine.Object.FindObjectsOfType<GameObject>();
                    if (obj != null)
                    {
                        foreach (GameObject gi in obj)
                        {
                            //Log(gi.name);
                            if (gi.name.Contains("Radiance"))
                            {
                                Log("radiantroom");
                                radiantroom = true;
                            }
                            if (gi.name.Contains("Ruins") && gi.name.Contains("Vial"))
                            {
                                sanctumroom = true;
                            }
                            if (gi.GetComponent<DropPlatform>())
                            {
                                gi.AddComponent<DropPlat>();
                                DropPlat q = gi.GetComponent<DropPlat>();
                                DropPlatform w = gi.GetComponent<DropPlatform>();
                                q.flipUpSound = w.flipUpSound;
                                q.collider = w.collider;
                                q.dropAnim = w.dropAnim;
                                q.dropSound = w.dropSound;
                                q.idleAnim = w.idleAnim;
                                q.spriteAnimator = w.spriteAnimator;
                                q.strikeEffect = w.strikeEffect;
                                q.landSound = w.landSound;
                                q.raiseAnim = w.raiseAnim;
                                q.waittime = 0.05f;
                                UnityEngine.Object.Destroy(w);
                            }
                            if (gi.name.Contains("Mines Platform"))
                            {
                                gi.transform.eulerAngles = new Vector3(0, 0, 180);
                                FlipPlatform f = gi.GetComponent<FlipPlatform>();
                                f.topSpikes.SetActive(true);
                                f.bottomSpikes.SetActive(false);
                                f.topSpikes.transform.eulerAngles = new Vector3(0, 0, 180);
                                slashsymbol = f.nailStrikePrefab;
                                slashsound = f.hitSound;
                                UnityEngine.Object.Destroy(f); 
                            }
                            if ((gi.name.Contains("abyss_plat")|| gi.name.Contains("lighthouse_0")) && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 336)
                            {
                                UnityEngine.Object.Destroy(gi);
                            }
                            if (gi.name.Contains("Laser Turret"))
                            {
                                GameManager.instance.StartCoroutine(staggerspawn(gi, 5, 0, 50));
                            }
                            if (gi.name.Contains("Jelly"))
                            {
                                jellyroom = true;
                            }
                            if (gi.name.Contains("White Palace Fly"))
                            {
                                gi.LocateMyFSM("Control").GetAction<iTweenMoveTo>("Wait", 0).time = 7;
                                foreach (var i in gi.GetComponent<tk2dSpriteAnimator>().Library.clips)
                                {
                                    if (i.name == "Wound")
                                    {
                                        i.fps *= 0.14f;
                                    }
                                }
                            }
                            if (gi.name.Contains("wp_saw"))
                            {
                                gi.transform.localScale *= 1.1f;
                            }
                            if (gi.name.Contains("Crystallised Lazer Bug"))
                            {
                                PlayMakerFSM s = gi.LocateMyFSM("Laser Bug");
                                s.GetAction<WaitRandom>("Idle", 0).timeMin = 0.2f;
                                s.GetAction<WaitRandom>("Idle", 0).timeMax = 0.2f;
                                s.GetAction<Wait>("Beam", 0).time = 0.3f;
                                foreach (var i in gi.GetComponent<tk2dSpriteAnimator>().Library.clips)
                                {
                                    if (i.name == "Ball Antic")
                                    {
                                        i.fps *= 5;
                                    }
                                }
                            }
                            if (gi.name.Contains("Chest"))
                            {
                                GameObject g2 = UnityEngine.Object.Instantiate(megalight, gi.transform.position, gi.transform.rotation) as GameObject;
                                g2.SetActive(true);
                                gi.SetActive(false);
                            }
                            if (gi.name.Contains("Zap Cloud"))
                            {
                                foreach (var i in gi.GetComponent<tk2dSpriteAnimator>().Library.clips)
                                {
                                    if (i.name == "Idle")
                                    {
                                        i.fps *= 2;
                                    }
                                }
                                gi.transform.localScale *= 1.2f;
                                PlayMakerFSM s = gi.LocateMyFSM("zap control");
                                s.GetAction<FloatAdd>("Ready", 3).add = 20f;
                            }
                            if (!Contains(gi) && gi.GetComponent<HealthManager>() != null && !(gi.name.Contains("Mega Fat Bee") || gi.name.Contains("Royal Gaurd")))
                            {
                                if ((gi.GetComponent<HealthManager>().enabled && !gi.GetComponent<HealthManager>().CheckInvincible() && gi.activeInHierarchy) || gi.name.Contains("Crystal") || gi.name.Contains("Crawler") || gi.name.Contains("Moss"))
                                {
                                    Clone(gi,0);
                                }
                                else
                                {
                                    if (!deadones.Contains(gi))
                                    {
                                        deadones.Add(gi);
                                        Log(gi.name + " added to deadones");
                                    }
                                }
                            }
                        }
                        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 227)
                        {
                            jellyroom = true;
                        }
                        try
                        {
                            if (Int32.Parse(Sceneplace.Substring(8)) > 3 && Sceneplace.Contains("Fungus3"))
                            {
                                mantisroom = true;
                            }
                        }
                        catch
                        {
                            //nothing
                        }
                        if (jellyroom)
                        {
                            mantisroom = false;
                        }
                    }
                }
                foreach (Enemy gi in dupedenemies)
                {
                    if (!gi.enemy)
                    {
                        dupedenemies.Remove(gi);
                        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Crossroads") && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == Sceneplace && !gi.name.Contains("Hollow Shade"))
                        {
                            GameObject exp = UnityEngine.Object.Instantiate(explosion, gi.lastpos, explosion.transform.rotation);
                            exp.SetActive(true);
                        }
                        break;
                    }
                    else
                    {
                        gi.lastpos = gi.enemy.transform.position;
                    }
                    if (jellydeadly)
                    {
                        if (gi.enemy.name.Contains("Jellyfish") && !(gi.enemy.name.Contains("Baby")|| gi.enemy.name.Contains("Mega")))
                        {
                            if (Vector3.Distance(HeroController.instance.gameObject.transform.position, gi.enemy.transform.position) < 5)
                            {
                                gi.enemy.GetComponent<HealthManager>().Die((float)rand.NextDouble() * 360, AttackTypes.Nail, true);
                            }
                        }
                    }
                }
                foreach (GameObject gi in deadones)
                {
                    if (!gi)
                    {
                        deadones.Remove(gi);
                    }
                    else
                    {
                        HealthManager g = gi.GetComponent<HealthManager>();
                        if (g.enabled && !g.CheckInvincible() && gi.activeInHierarchy)
                        {
                            deadones.Remove(gi);
                            Clone(gi, 3);
                        }
                    }
                }
                Sceneplace = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if (cansummon && !jellyroom && HeroController.instance.CheckTouchingGround() && !(UnityEngine.Object.FindObjectsOfType<PromptMarker>().Length > 800000000) && HeroController.instance.CanInput() && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Fungus1") || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Fungus3")))
                {
                    GameObject _ = UnityEngine.Object.Instantiate(plant, HeroController.instance.gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 0));
                    _.SetActive(true);
                    cansummon = false;
                    GameManager.instance.StartCoroutine(waitawhile(0.75f, 0));
                }
                if (cansummon4 && !(UnityEngine.Object.FindObjectsOfType<PromptMarker>().Length > 0) && HeroController.instance.CanInput() && jellyroom)
                {
                    GameObject _ = UnityEngine.Object.Instantiate(shock, HeroController.instance.gameObject.transform.position + new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                    _.SetActive(true);
                    cansummon4 = false;
                    GameManager.instance.StartCoroutine(waitawhile(0.5f, 3));
                }
                if (cansummon3 && !(UnityEngine.Object.FindObjectsOfType<PromptMarker>().Length > 0) && HeroController.instance.CanInput() && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("White_Palace") || radiantroom && HeroController.instance.transform.position.y > 40))
                {
                    GameObject killus = new GameObject();
                    killus.transform.position = HeroController.instance.transform.position;
                    killus.transform.eulerAngles = new Vector3(0, 0, 180 + rotatenum);
                    GameObject _ = UnityEngine.Object.Instantiate(beam, killus.transform);
                    _.transform.localPosition = new Vector3(0, -30, 0);
                    _.transform.localScale = new Vector3(60, 2, 1);
                    Vector3 e = _.transform.eulerAngles;
                    Vector3 E = _.transform.position;
                    _.transform.parent = null;
                    Vector3 d = _.transform.position;
                    Vector3 i = HeroController.instance.transform.position;
                    i.x -= d.x;
                    i.y -= d.y;
                    _.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(i.y, i.x) * Mathf.Rad2Deg);
                    _.transform.position = new Vector3(E.x, E.y, 0);
                    _.SetActive(true);
                    Log(_.transform.position + "  -   " + _.transform.eulerAngles + " >>> " + HeroController.instance.transform.position);
                    EMT x = new EMT();
                    x.name = "ANTIC";
                    EMT y = new EMT();
                    y.name = "FIRE";
                    y.waittime = 0.5f;
                    EMT z = new EMT();
                    z.waittime = 0.5f;
                    z.name = "END";
                    GameManager.instance.StartCoroutine(beamevent(_.GetComponent<PlayMakerFSM>(), new EMT[3] { x , y, z }));
                    cansummon3 = false;
                    if (radiantroom)
                    {
                        GameManager.instance.StartCoroutine(waitawhile(0.5f, 2));
                        rotatenum += 30;
                    }
                    else
                    {
                        GameManager.instance.StartCoroutine(waitawhile(0.75f, 2));
                    }
                }
                if(Sceneplace.Length > 7)
                {
                    if (cansummon2 && mantisroom && !(UnityEngine.Object.FindObjectsOfType<PromptMarker>().Length > 0) && HeroController.instance.CanInput())
                    {
                        int x = (rand.Next(2) * 2) - 1;
                        float y = rand.Next(2);
                        GameObject shoot = UnityEngine.Object.Instantiate(mantisshot, HeroController.instance.gameObject.transform.position + new Vector3(((float)rand.NextDouble() + 25) * x, ((float)rand.NextDouble() + 15) * y, 0), Quaternion.Euler(0, 0, 0));
                        shoot.SetActive(true);
                        Rigidbody2D r = shoot.GetComponent<Rigidbody2D>();
                        //r.angularVelocity = Vector3.Angle(shoot.transform.position, HeroController.instance.gameObject.transform.position);
                        r.velocity = (HeroController.instance.gameObject.transform.position - shoot.transform.position);
                        cansummon2 = false;
                        GameManager.instance.StartCoroutine(waitawhile(1.2f, 1));
                    }
                }
                if (blockersaw && PlayerData.instance.zoteRescuedBuzzer)
                {
                    UnityEngine.GameObject.Destroy(blockersaw);
                }
            }
        }
        System.Random rand = new System.Random();
        IEnumerator beamevent(PlayMakerFSM f, EMT[] emt)
        {
            foreach(EMT e in emt)
            {
                yield return new WaitForSeconds(e.waittime);
                f.SendEvent(e.name);
            }
            Log(f.ActiveStateName);
        }
        IEnumerator staggerspawn(GameObject g, int n, float p, float r)
        {
            Log(n);
            for (int i = 0; i < n; i++)
            {
                Log(i);
                GameObject l2 = UnityEngine.Object.Instantiate(g, g.transform.position + new Vector3(((float)rand.NextDouble() - 0.5f) * p,((float)rand.NextDouble() - 0.5f) * p, 0), Quaternion.Euler(g.transform.rotation.eulerAngles + new Vector3(0, 0, ((float)rand.NextDouble() - 0.5f) * r)));
                l2.SetActive(true);
                yield return new WaitForSeconds((float)rand.NextDouble() * 0.5f);
            }
        }

        public DamageHero dmghero;
        public void Clone(GameObject gi, int type)
        {
            //       UnityEngine.SceneManagement.SceneManager.LoadScene(373)
            if (type < 1 && (gi.name.Contains("Hive Knight")||gi.name.Contains("Nightmare Grimm Boss")))
            {
                return;
            }
            if (type == 3 && gi.name.Contains("Grey Prince"))
            {
                return;
            }
            if (Sceneplace.Contains("Ruins") && !gi.name.Contains("Flying") && !sanctumroom)
            {
                GameObject g2 = UnityEngine.GameObject.Instantiate(sentry, gi.transform.position, gi.transform.rotation);
                g2.SetActive(true);
            }
            if(dmghero == null&& gi.GetComponent<DamageHero>())
            {
                dmghero = gi.GetComponent<DamageHero>();
            }
            int repeattimes = 1;
            float posrandomiser = 1;
            if (gi.name.Contains("Mantis"))
            {
                mantisroom = true;
                if(gi.GetComponent<HealthManager>().hp < 50)
                {
                    repeattimes = 3;
                }
            }
            if(gi.name.Contains("Royal Gaurd"))
            {
                repeattimes = 3;
            }
            if (gi.name.Contains("Moss"))
            {
                repeattimes = 5;
            }
            if(gi.name.Contains("Mage")&&!(gi.name.Contains("Lord")|| gi.name.Contains("Knight")))
            {
                repeattimes = 3; 
            }
            if (gi.name.Contains("White Palace Fly"))
            {
                repeattimes = 0;
            }
            if (gi.name.Contains("Mosquito"))
            {
                repeattimes = 5;
            }
            if (gi.name.Contains("Mushroom"))
            {
                posrandomiser = 18;
                repeattimes = 5;
                gi.GetComponent<HealthManager>().hp *= 2;
                if (gi.name.Contains("Turret"))
                {
                    gi.GetComponent<HealthManager>().hp = 99999;
                }
                if (gi.name.Contains("Brawler"))
                {
                    posrandomiser = 2;
                }
                if (gi.name.Contains("Baby"))
                {
                    gi.GetComponent<HealthManager>().hp *= 2;
                    DamageHero e = gi.AddComponent<DamageHero>();
                    e = dmghero;
                    e.damageDealt = 1;
                    e.hazardType = 1;
                }
            }
            if (gi.name.Contains("Fung"))
            {
                repeattimes = 10;
                gi.GetComponent<HealthManager>().hp = 25;
                posrandomiser = 10;
            }
            if (gi.name.Contains("Bee") && !gi.name.Contains("Fat"))
            {
                repeattimes = 4;
            }
            if (gi.name.Contains("Fat Fly"))
            {
                repeattimes = 9;
            }
            if(gi.name.Contains("Buzzer") && !gi.name.Contains("Giant"))
            {
                repeattimes = 5;
            }
            if (gi.name.Contains("Sentry"))
            {
                repeattimes = 5;
                posrandomiser = 8;
                if (gi.name.Contains("Flying"))
                {
                    posrandomiser = 1;
                }
            }
            if (gi.name.Contains("Head")||gi.name.Contains("Zote Boss"))
            {
                return;
            }
            if (gi.name.Contains("Crystallised Lazer Bug"))
            {
                repeattimes = 8;
                posrandomiser = 4;
            }
            if (gi.name.Contains("Hornet"))
            {
                repeattimes = 2;
            }
            if (gi.name.Contains("Inflater"))
            {
                repeattimes = 5;
            }
            if (gi.name.Contains("Spitter"))
            {
                repeattimes = 4;
            }
            if (gi.name.Contains("Hopper") && !gi.name.Contains("Giant") && !Sceneplace.ToLower().Contains("ruins") && type != 1)
            {
                if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 165)
                {
                    repeattimes = 10;
                    posrandomiser = 7;
                }
                else
                {
                    GameObject g2 = UnityEngine.GameObject.Instantiate(bighop, gi.transform.position, gi.transform.rotation);
                    g2.SetActive(true);
                    UnityEngine.Object.Destroy(gi);
                    return;
                }
            }
            if (gi.name.Contains("Fluke") || gi.name.Contains("Jellyfish Baby"))
            {
                gi.GetComponent<HealthManager>().SetGeoSmall(0);
                if (gi.GetComponent<HealthManager>().hp < 101)
                {
                    repeattimes++;
                    if (gi.GetComponent<HealthManager>().hp < 81)
                    {
                        repeattimes++;
                        if (gi.GetComponent<HealthManager>().hp < 51)
                        {
                            repeattimes++;
                            if (gi.GetComponent<HealthManager>().hp < 26)
                            {
                                repeattimes++;
                                if (gi.GetComponent<HealthManager>().hp < 21)
                                {
                                    repeattimes++;
                                    if (gi.GetComponent<HealthManager>().hp < 11)
                                    {
                                        repeattimes += 5;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (gi.GetComponent<HealthManager>().hp < 20)
                {
                    gi.GetComponent<HealthManager>().hp = 20;
                }
            }
            gi.GetComponent<HealthManager>().hp = Mathf.RoundToInt(gi.GetComponent<HealthManager>().hp *1.3f);
            if (gi.name.Contains("Spider")&&gi.GetComponent<HealthManager>().hp <150)
            {
                repeattimes = 6;
            }
            dupedenemies.Add(new Enemy { enemy = gi, lastpos = gi.transform.position, name = gi.name }) ;
            for (int i = 0; i < repeattimes; i++)
            {
                GameObject g2 = UnityEngine.Object.Instantiate(gi, gi.transform.position + Vector3.right * ((float)rand.NextDouble() -0.5f ) * posrandomiser, gi.transform.rotation) as GameObject;
                g2.SetActive(true);
                dupedenemies.Add(new Enemy { enemy = g2, lastpos = g2.transform.position, name = g2.name });
                g2.GetComponent<HealthManager>().SetGeoLarge(0);
                g2.GetComponent<HealthManager>().SetGeoMedium(0);
                g2.GetComponent<HealthManager>().SetGeoSmall(0);
                Log(gi.name + " /\\/\\ " + type);
                if (gi.name.Contains("Black Knight") && rand.Next(0,1)>0)
                {
                    g2.GetComponent<HealthManager>().Die((float)rand.NextDouble() * 360, AttackTypes.Nail, true);
                }
                if (g2.name.Contains("Mushroom Baby"))
                {
                    DamageHero e = g2.AddComponent<DamageHero>();
                    e = dmghero;
                    e.damageDealt = 1;
                    e.hazardType = 1;
                }
                if (gi.name == "Nightmare Grimm Boss")
                {
                    Log(g2.LocateMyFSM("Control").ActiveStateName);
                    g2.LocateMyFSM("Control").SendEvent("WAKE");
                }
                if (gi.name.Contains("Radiance"))
                {
                    Log("radiantroom");
                    g2.GetComponent<HealthManager>().hp = 9999;
                    radiantroom = true;
                }
            }
        }

        public int entitynum;

        private string LanguageGet(string key, string sheetTitle, string orig)
        {
                Log(key + ": \"" + orig + "\"");
            if (orig == "Nosk")
            {
                return ("The Imposter");
            }
            if (key == "TUT_TAB_03")
            {
                return ("Check your inventory");
            }
            if (key == "ATRIUM_ENTER_TEXT")
            {
                return ("So you think you can do the Pantheons?<br><page>That's kinda.........<page>.....<page>cringe");
            }
            if (key == "TUT_TAB_01")
            {
                return ("Healing is for the weak, and this mod isn't for the weak");
            }
                return key == "SHOP_DESC_WAYWARDCOMPASS"
                ? "This is the best charm in the game"
                 : orig;
        }

        public List<GameObject> deadones;
        private HitInstance OnHit(Fsm owner, HitInstance hit)
        {
            //PlayerData.instance.equippedCharm_13 = true;
            switch (hit.AttackType)
            {
                case AttackTypes.Nail when _settings.LimitNail:
                    // hit.DamageDealt = 10000;
                    break;
            }

            return hit;
        }

        // private void OnNewGame() => OnSaveLoaded();


        private int OnInt(string intName, int orig)
        {
            return intName switch
            {
                //"maxMP" when _settings.LimitSoulCapacity => PlayerData.instance.maxMP * 3,
                //"MPReserveMax" when _settings.LimitSoulCapacity => PlayerData.instance.MPReserveMax / 3,

                // Dreamshield
                // "charmCost_1" => -2,

                _ => orig
            };
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
            ModHooks.ColliderCreateHook -= OnCollMake;
            ModHooks.LanguageGetHook -= LanguageGet;
            ModHooks.TakeHealthHook -= OnHealthTaken;
            ModHooks.SoulGainHook -= OnSoulGain;
            ModHooks.GetPlayerIntHook -= OnInt;
            ModHooks.HitInstanceHook -= OnHit;
            ModHooks.HeroUpdateHook -= OnHeroUpdate;
            ModHooks.GetPlayerBoolHook -= OnBool;
            On.HeroController.CanFocus -= Focus;
        }
    }
}
