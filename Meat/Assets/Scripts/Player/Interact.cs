using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interact : MonoBehaviour
{
    [Header("Cutting Board References")]
    [SerializeField] Transform cuttingBoardCamera;
    [SerializeField] Animator sliceAnim;
    [SerializeField] Transform cuttingBoardCamLoc;
    [SerializeField] ParticleSystem BloodSplatter;

    [Header("Computer References")]
    [SerializeField] Transform computerCamera;
    [SerializeField] Transform computerCamLoc;
    [SerializeField] TMP_Text BaggerText;
    [SerializeField] TMP_Text BaggerNumberText;
    [SerializeField] Material RedMat;
    [SerializeField] Material GreenMat;

    [Header("Meat References")]
    [SerializeField] GameObject MeatPiece1Left;
    [SerializeField] GameObject MeatPiece1Right;
    [SerializeField] GameObject MeatPiece1;
    [SerializeField] GameObject MeatBag;
    [SerializeField] GameObject BloodSprayEffect;

    [Header("Player References")]
    [SerializeField] GameObject MeatHand;
    [SerializeField] GameObject MeatHandLeft;
    [SerializeField] GameObject MeatHandRight;
    [SerializeField] GameObject BagHand;
    [SerializeField] GameObject WoodHand;
    [SerializeField] GameObject JarHand;
    [SerializeField] GameObject StickHand;
    [SerializeField] GameObject GrabMarker;
    [SerializeField] GameObject WoodPiece;
    [SerializeField] GameObject Jar;
    [SerializeField] GameObject MeatHandRend;
    [SerializeField] GameObject MeatHandLeftRend;
    [SerializeField] GameObject MeatHandRightRend;
    [SerializeField] GameObject FlashLight;
    [SerializeField] GameObject MeatHandSmoke;
    [SerializeField] GameObject MeatHandLeftSmoke;
    [SerializeField] GameObject MeatHandRightSmoke;
    [SerializeField] GameObject[] InteractMarkers;
    [SerializeField] Transform doorHandleMarker;
    [SerializeField] Transform leftBaggerMarker;
    [SerializeField] Transform rightBaggerMarker;
    [SerializeField] Transform trashChuteMarker;
    [SerializeField] Transform playerCamera;

    [Header("Other References")]
    [SerializeField] LayerMask hitMask;
    [SerializeField] LayerMask enemyHitMask;
    [SerializeField] ParticleSystem ElectricityEffectBurst;
    [SerializeField] GameObject ElectricityEffect;
    [SerializeField] GameObject FireEffect;
    [SerializeField] GameObject StickObj;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject HumanoidMonster;
    [SerializeField] GameObject HeadMonster;
    [SerializeField] Animator StickAnim;

    [Header("Interact Properties")]
    [SerializeField] float interactDistance = 5f;

    [Header("Sounds")]
    [SerializeField] AudioSource MeatSound1;
    [SerializeField] AudioSource MeatSound2;
    [SerializeField] AudioSource MeatSound3;
    [SerializeField] AudioSource BagSound1;
    [SerializeField] AudioSource BagSound2;
    [SerializeField] AudioSource BagSound3;
    [SerializeField] AudioSource ClickDown;
    [SerializeField] AudioSource ClickUp;
    [SerializeField] AudioSource DoorOpen;
    [SerializeField] AudioSource DoorClose;
    [SerializeField] AudioSource MetalDoorOpen;
    [SerializeField] AudioSource MetalDoorClose;
    [SerializeField] AudioSource ButtonPress;
    [SerializeField] AudioSource BaggerRun;
    [SerializeField] AudioSource SwitchSound;
    [SerializeField] AudioSource KnifeCut;
    [SerializeField] AudioSource zap1;
    [SerializeField] AudioSource zap2;
    [SerializeField] AudioSource zap3;
    [SerializeField] AudioSource zap4;
    [SerializeField] AudioSource CookerSwitchSound;
    [SerializeField] AudioSource BaggerClogged;
    [SerializeField] AudioSource Ambience;
    [SerializeField] AudioSource BaggerUnclog;
    [SerializeField] GameObject MetalDoorOpenLoc;
    [SerializeField] GameObject MetalDoorCloseLoc;

    [Header("Keybinds")]
    [SerializeField] KeyCode interact = KeyCode.E;
    [SerializeField] KeyCode swtichItem = KeyCode.Q;
    [SerializeField] KeyCode Pause = KeyCode.Escape;

    Animator doorAnim;
    Animator baggerDoorAnim;

    public bool isComputer = false;
    public bool clickedClockOut = false;
    private bool holdingMeat = false; 
    private bool holdingBag = false;
    private bool holdingWood = false;
    private bool holdingJar = false;
    private bool holdingMeatLeft = false;
    private bool holdingMeatRight = false;
    private bool isCuttingBoard = false;
    private bool knifeCooldown = true;
    private bool dontThrow = false;
    private bool cuttingBoardCameraAnim = false;
    private bool computerCameraAnim = false;
    private bool isAnimatingCam = false;
    private bool canAnimateCam = true;
    private bool isAnimatingComputer = false;
    private bool isAnimatingCuttingBoard = false;
    private bool canOpenOrCloseDoor = true;
    private bool canOpenOrCloseDoorLeft = true;
    private bool canOpenOrCloseDoorRight = true;
    private bool canPressButton = true;
    private bool lightSwitchCooldown = false;
    private bool baggerClogged = false;
    private bool canFlipCookerSwitch = true;
    private bool holdingStick = false;
    private bool canHitStick = true;
    private bool makeBagCooldown = false;
    private bool paused = false;
    private bool baggerErrorBias = false;
    [HideInInspector] public bool cookerSwitchFlipped = false;

    private string holdingCookedLevel;
    private string holdingCookedLevelLeft;
    private string holdingCookedLevelRight;


    [Header("Lights")]
    public Texture2D[] darkLightmapDir;
    public Texture2D[] darkLightmapColor;
    public Texture2D[] brightLightmapDir;
    public Texture2D[] brightLightmapColor;
    public Light[] handheldLights;
    public Cubemap darkProbe, brightProbe;
    public ReflectionProbe probe;
    public GameObject light1, light2;

    private LightmapData[] darkLightmap, brightLightmap;

    private void Start()
    {
        StartCoroutine(FadeInAmbience());
        // NORMAL
        List<LightmapData> darklightmap = new List<LightmapData>();

        for (int i = 0; i < darkLightmapDir.Length; i++)
        {
            LightmapData lmdata = new LightmapData();

            lmdata.lightmapDir = darkLightmapDir[i];
            lmdata.lightmapColor = darkLightmapColor[i];

            darklightmap.Add(lmdata);
        }

        darkLightmap = darklightmap.ToArray();


        // NORM
        List<LightmapData> brightlightmap = new List<LightmapData>();

        for (int i = 0; i < brightLightmapDir.Length; i++)
        {
            LightmapData lmdata = new LightmapData();

            lmdata.lightmapDir = brightLightmapDir[i];
            lmdata.lightmapColor = brightLightmapColor[i];

            brightlightmap.Add(lmdata);
        }

        brightLightmap = brightlightmap.ToArray();

        probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
        probe.customBakedTexture = brightProbe;

        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;

        RenderSettings.customReflectionTexture = null;
        probe.customBakedTexture = brightProbe;
        probe.RenderProbe();
    }

    IEnumerator FadeInAmbience()
    {
        Ambience.volume = 0;
        while (Ambience.volume < 1)
        {
            Ambience.volume += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void Update()
    {
        interactDetection();
        input();
        cameraAnim(); 
    }

    private void interactDetection()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactDistance, hitMask, QueryTriggerInteraction.Collide))
        {
            // JUST FOR INTERACT MARKER
            if (isComputer || isCuttingBoard)
            {
                GrabMarker.GetComponent<Animator>().SetBool("On", false);
                for (int i = 0; i < 9; i++)
                {
                    InteractMarkers[i].GetComponent<Animator>().SetBool("On", false);
                    InteractMarkers[i].GetComponent<Animator>().SetBool("Press", false);
                }
            }
            else
            {
                if (hit.transform.CompareTag("Computer"))
                {
                    InteractMarkers[0].GetComponent<Animator>().SetBool("On", true);
                    GrabMarker.GetComponent<Animator>().SetBool("On", false);
                }
                else if (hit.transform.CompareTag("Board"))
                {
                    InteractMarkers[1].GetComponent<Animator>().SetBool("On", true);
                    GrabMarker.GetComponent<Animator>().SetBool("On", false);
                }
                else if (hit.transform.gameObject.CompareTag("BaggerDoorLeft"))
                {
                    InteractMarkers[2].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("BaggerDoorRight"))
                {
                    InteractMarkers[3].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("GreenButton"))
                {
                    InteractMarkers[4].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("RedButton"))
                {
                    InteractMarkers[5].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("TrashChuteDoor"))
                {
                    InteractMarkers[6].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("Door"))
                {
                    InteractMarkers[7].GetComponent<Animator>().SetBool("On", true);
                    GrabMarker.GetComponent<Animator>().SetBool("On", false);
                }
                else if (hit.transform.gameObject.CompareTag("LightSwitch"))
                {
                    if (!lightSwitchCooldown)
                    {
                        InteractMarkers[8].GetComponent<Animator>().SetBool("On", true);
                    }
                }
                else if (hit.transform.gameObject.CompareTag("CookerDoorLeft"))
                {
                    InteractMarkers[9].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("CookerDoorRight"))
                {
                    InteractMarkers[10].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("Blender") && EventHandler.i.Day == 5)
                {
                    InteractMarkers[13].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("CookerSwitch") && EventHandler.i.Day >= 3)
                {
                    InteractMarkers[11].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("Wood") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag && EventHandler.i.Day >= 3)
                {
                    InteractMarkers[12].GetComponent<Animator>().SetBool("On", true);
                }
                else if (hit.transform.gameObject.CompareTag("Meat")) 
                {
                    if (!holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag)
                    {
                        GrabMarker.GetComponent<Animator>().SetBool("On", true);
                        GrabMarker.GetComponent<RectTransform>().position = new Vector3(hit.transform.position.x, hit.transform.position.y + 0.75f, hit.transform.position.z);
                        InteractMarkers[1].GetComponent<Animator>().SetBool("On", false);
                    }
                    else if (holdingMeatLeft || holdingMeatRight)
                    {
                        GrabMarker.GetComponent<Animator>().SetBool("On", false);
                    }
                }
                else if ((hit.transform.gameObject.CompareTag("MeatLeft") || hit.transform.gameObject.CompareTag("MeatRight")) && !(holdingMeatLeft && holdingMeatRight) && !holdingMeat && !holdingBag)
                {
                    Renderer rend = hit.transform.GetComponentInChildren<Renderer>();

                    GrabMarker.GetComponent<Animator>().SetBool("On", true);
                    GrabMarker.GetComponent<RectTransform>().position = new Vector3(rend.bounds.center.x, rend.bounds.center.y + 0.75f, rend.bounds.center.z);
                    InteractMarkers[1].GetComponent<Animator>().SetBool("On", false);
                }
                else if (hit.transform.gameObject.CompareTag("Bag"))
                {
                    if (!holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag)
                    {
                        Renderer rend = hit.transform.GetComponent<Renderer>();

                        GrabMarker.GetComponent<Animator>().SetBool("On", true);
                        InteractMarkers[1].GetComponent<Animator>().SetBool("On", false);

                        if (hit.transform.GetComponent<Bag>().numPieces <= 2)
                        {
                            GrabMarker.GetComponent<RectTransform>().position = new Vector3(rend.bounds.center.x, rend.bounds.center.y + 0.75f, rend.bounds.center.z);
                        }
                        else
                        {
                            GrabMarker.GetComponent<RectTransform>().position = new Vector3(rend.bounds.center.x, rend.bounds.center.y + 1f, rend.bounds.center.z);
                        }
                    }
                    else if (holdingMeatLeft || holdingMeatRight)
                    {
                        GrabMarker.GetComponent<Animator>().SetBool("On", false);
                    }
                }
                else if (hit.transform.gameObject.CompareTag("WoodPiece"))
                {
                    if (hit.transform.GetComponent<Wood>().inFire == false && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag)
                    {
                        Renderer rend = hit.transform.GetComponentInChildren<Renderer>();

                        GrabMarker.GetComponent<Animator>().SetBool("On", true);
                        GrabMarker.GetComponent<RectTransform>().position = new Vector3(rend.bounds.center.x, rend.bounds.center.y + 0.75f, rend.bounds.center.z);
                        InteractMarkers[1].GetComponent<Animator>().SetBool("On", false);
                    }
                    else if (holdingMeatLeft || holdingMeatRight)
                    {
                        GrabMarker.GetComponent<Animator>().SetBool("On", false);
                    }
                }
                else if (hit.transform.gameObject.CompareTag("Jar"))
                {
                    if (!holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag)
                    {
                        Renderer rend = hit.transform.GetComponentInChildren<Renderer>();

                        GrabMarker.GetComponent<Animator>().SetBool("On", true);
                        GrabMarker.GetComponent<RectTransform>().position = new Vector3(rend.bounds.center.x, rend.bounds.center.y + 0.25f, rend.bounds.center.z + 0.4f);
                        InteractMarkers[1].GetComponent<Animator>().SetBool("On", false);
                    }
                    else if (holdingMeatLeft || holdingMeatRight)
                    {
                        GrabMarker.GetComponent<Animator>().SetBool("On", false);
                    }
                }
                else if (hit.transform.CompareTag("Stick") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag)
                {
                    Renderer rend = hit.transform.GetComponentInChildren<Renderer>();

                    GrabMarker.GetComponent<Animator>().SetBool("On", true);
                    GrabMarker.GetComponent<RectTransform>().position = new Vector3(rend.bounds.center.x, rend.bounds.center.y + 0.25f, rend.bounds.center.z + 0.4f);
                    InteractMarkers[1].GetComponent<Animator>().SetBool("On", false);
                }
            }

            // DON'T THROW WHEN LOOKING AT SOMETHING
            if (hit.transform.CompareTag("Board") || (hit.transform.CompareTag("Meat") && (holdingMeat || holdingMeatLeft || holdingMeatRight)) || (hit.transform.CompareTag("Bag") && (holdingMeat || holdingMeatLeft || holdingMeatRight)) || (hit.transform.CompareTag("Jar") && (holdingMeat || holdingMeatLeft || holdingMeatRight)) || (hit.transform.CompareTag("MeatLeft") && (holdingMeat || (holdingMeatLeft && holdingMeatRight))) || (hit.transform.CompareTag("MeatRight") && (holdingMeat || (holdingMeatLeft && holdingMeatRight))))
            {
                dontThrow = false;
            }
            else
            {
                dontThrow = true;
            }

            // INTERACT
            if (Input.GetKeyDown(interact) && hit.transform != null)
            {
                if (hit.transform.CompareTag("Computer") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !paused && !clickedClockOut)
                {
                    InteractMarkers[0].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    Computer();
                }
             

                if (hit.transform.CompareTag("Door"))
                {
                    InteractMarkers[7].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    doorAnim = hit.transform.GetComponentInParent<Animator>();
                    Door(doorAnim);
                }


                if (hit.transform.CompareTag("Meat") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !isAnimatingCam && !holdingStick && !isCuttingBoard)
                {
                    GrabMarker.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    string cooked;
                    if (hit.transform.GetComponent<Meat>().cookedWell)
                    {
                        cooked = "well";
                    }
                    else if (hit.transform.GetComponent<Meat>().cookedLight)
                    {
                        cooked = "light";
                    }
                    else
                    {
                        cooked = "none";
                    }

                    Meat(false, cooked, "Meat", hit);
                }
                else if ((hit.transform.CompareTag("MeatLeft") || hit.transform.CompareTag("MeatRight")) && !holdingMeatLeft && !holdingMeat && !isAnimatingCam && !holdingStick && !isCuttingBoard)
                {
                    GrabMarker.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    string cooked;
                    if (hit.transform.GetComponent<SmallMeat>().cookedWell)
                    {
                        cooked = "well";
                    }
                    else if (hit.transform.GetComponent<SmallMeat>().cookedLight)
                    {
                        cooked = "light";
                    }
                    else
                    {
                        cooked = "none";
                    }

                    Meat(false, cooked, "MeatLeft", hit);
                }
                else if ((hit.transform.CompareTag("MeatRight") || hit.transform.CompareTag("MeatLeft")) && !holdingMeatRight && !holdingMeat && !isAnimatingCam && !holdingStick && !isCuttingBoard)
                {
                    GrabMarker.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    string cooked;
                    if (hit.transform.GetComponent<SmallMeat>().cookedWell)
                    {
                        cooked = "well";
                    }
                    else if (hit.transform.GetComponent<SmallMeat>().cookedLight)
                    {
                        cooked = "light";
                    }
                    else
                    {
                        cooked = "none";
                    }

                    Meat(false, cooked, "MeatRight", hit);
                }


                if ((hit.transform.CompareTag("Board") && EventHandler.i.MeatOnBoard && !paused && !isCuttingBoard && !holdingMeat && !holdingMeatLeft && !holdingMeatRight) || isCuttingBoard)
                {
                    InteractMarkers[1].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    CuttingBoard();
                }
                else if ((hit.transform.CompareTag("Board") && !EventHandler.i.MeatOnBoard && !isCuttingBoard) || isCuttingBoard)
                {
                    InteractMarkers[1].GetComponent<Animator>().SetBool("NotBoard", true);
                    StartCoroutine(disablePress());
                }


                if (hit.transform.CompareTag("GreenButton") && canPressButton)
                {
                    canPressButton = false;

                    ButtonPress.Play();
                    hit.transform.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(resetButtonPress(hit.transform));

                    InteractMarkers[4].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    if (EventHandler.i.PiecesInBagger > 0)
                    {
                        int random = Random.Range(0, 6);

                        if (random < 5 && !baggerClogged)
                        {
                            baggerErrorBias = false;
                            MakeBag();
                        }
                        else
                        {
                            if (!makeBagCooldown && !baggerErrorBias)
                            {
                                baggerErrorBias = true;
                                baggerClogged = true;
                                BaggerClogged.Play();
                                BaggerText.text = "Error";
                                BaggerText.fontMaterial = RedMat;
                                BaggerNumberText.fontMaterial = RedMat;
                            }
                            else if (!makeBagCooldown)
                            {
                                baggerErrorBias = false;
                                MakeBag();
                            }
                        }
                    }
                }
                else if (hit.transform.CompareTag("RedButton") && canPressButton)
                {
                    canPressButton = false;

                    ButtonPress.Play();
                    hit.transform.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(resetButtonPress(hit.transform));

                    InteractMarkers[5].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    if (EventHandler.i.PiecesInBagger > 0)
                    {
                        if (baggerClogged)
                        {
                            StartCoroutine(UnclogBagger());
                        }
                    }
                }


                if (hit.transform.CompareTag("BaggerDoorLeft"))
                {
                    MetalDoorOpenLoc.transform.position = new Vector3(-33.783f, 4.386f, 26.266f);
                    MetalDoorCloseLoc.transform.position = new Vector3(-33.783f, 4.386f, 26.266f);

                    InteractMarkers[2].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    baggerDoorAnim = hit.transform.GetComponent<Animator>();
                    Bagger(baggerDoorAnim, "Left");
                }
                else if (hit.transform.CompareTag("BaggerDoorRight"))
                {
                    MetalDoorOpenLoc.transform.position = new Vector3(-30.218f, 4.386f, 26.266f);
                    MetalDoorCloseLoc.transform.position = new Vector3(-30.218f, 4.386f, 26.266f);

                    InteractMarkers[3].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    baggerDoorAnim = hit.transform.GetComponent<Animator>();
                    Bagger(baggerDoorAnim, "Right");
                }
                else if (hit.transform.CompareTag("CookerDoorLeft"))
                {
                    MetalDoorOpenLoc.transform.position = new Vector3(-26.772f, 0.256f, 33.077f);
                    MetalDoorCloseLoc.transform.position = new Vector3(-26.772f, 0.256f, 33.077f);

                    InteractMarkers[9].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    baggerDoorAnim = hit.transform.GetComponent<Animator>();
                    Bagger(baggerDoorAnim, "LeftC");
                }
                else if (hit.transform.CompareTag("CookerDoorRight"))
                {
                    MetalDoorOpenLoc.transform.position = new Vector3(-23.186f, 0.256f, 33.077f);
                    MetalDoorCloseLoc.transform.position = new Vector3(-23.186f, 0.256f, 33.077f);

                    InteractMarkers[10].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    baggerDoorAnim = hit.transform.GetComponent<Animator>();
                    Bagger(baggerDoorAnim, "RightC");
                }
                else if (hit.transform.CompareTag("Blender") && EventHandler.i.Day == 5)
                {
                    MetalDoorOpenLoc.transform.position = new Vector3(-31.906f, 4.791f, 10.481f);
                    MetalDoorCloseLoc.transform.position = new Vector3(-31.906f, 4.791f, 10.481f);

                    InteractMarkers[13].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    baggerDoorAnim = hit.transform.GetComponent<Animator>();
                    Bagger(baggerDoorAnim, "Blender");
                }
                else if (hit.transform.CompareTag("TrashChuteDoor"))
                {
                    MetalDoorOpenLoc.transform.position = new Vector3(-26.79904f, 3.24596f, 10.55951f);
                    MetalDoorCloseLoc.transform.position = new Vector3(-26.79904f, 3.24596f, 10.55951f);

                    InteractMarkers[6].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    baggerDoorAnim = hit.transform.GetComponent<Animator>();
                    Bagger(baggerDoorAnim, "Trash");
                }


                if (hit.transform.CompareTag("Bag") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingStick)
                {
                    GrabMarker.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    Bag(hit);
                }


                if (hit.transform.CompareTag("LightSwitch") && !lightSwitchCooldown)
                {
                    InteractMarkers[8].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    LightSwitch(hit.transform.Find("Cube.001"), false);
                }


                if (hit.transform.CompareTag("CookerSwitch") && EventHandler.i.Day >= 3)
                {
                    InteractMarkers[11].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    CookerSwitch(hit.transform);
                }


                if (hit.transform.CompareTag("Wood") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag && !holdingStick && EventHandler.i.Day >= 3)
                {
                    InteractMarkers[12].GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    Wood();
                }


                if (hit.transform.CompareTag("WoodPiece") && hit.transform.GetComponent<Wood>().inFire == false && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag && !holdingStick)
                {
                    GrabMarker.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    WoodPieceFunc(hit);
                }


                if (hit.transform.CompareTag("Jar") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag && !holdingStick)
                {
                    GrabMarker.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    JarFunc(hit);
                }


                if (hit.transform.CompareTag("Stick") && !holdingMeat && !holdingMeatLeft && !holdingMeatRight && !holdingBag)
                {
                    GrabMarker.GetComponent<Animator>().SetBool("Press", true);
                    StartCoroutine(disablePress());

                    Stick();
                }
            }
        }
        else
        {
            GrabMarker.GetComponent<Animator>().SetBool("On", false);
            for (int i = 0; i < InteractMarkers.Length; i++)
            {
                InteractMarkers[i].GetComponent<Animator>().SetBool("On", false);
                InteractMarkers[i].GetComponent<Animator>().SetBool("Press", false);
            }

            dontThrow = false;
        }
    }

    IEnumerator UnclogBagger()
    {
        BaggerUnclog.Play();
        yield return new WaitForSeconds(7f);
        BaggerText.text = "Bag Size";
        BaggerText.fontMaterial = GreenMat;
        BaggerNumberText.fontMaterial = GreenMat;
        baggerClogged = false;
    }

    public void ExtUnclogBagger()
    {
        BaggerText.text = "Bag Size";
        BaggerText.fontMaterial = GreenMat;
        BaggerNumberText.fontMaterial = GreenMat;
        baggerClogged = false;
    }

    IEnumerator disablePress()
    {
        yield return new WaitForEndOfFrame();
        InteractMarkers[1].GetComponent<Animator>().SetBool("NotBoard", false);

        for (int i = 0; i < InteractMarkers.Length; i++)
        {
            InteractMarkers[i].GetComponent<Animator>().SetBool("Press", false);
        }

        GrabMarker.GetComponent<Animator>().SetBool("Press", false);
    }

    IEnumerator switchMarkerPosition(GameObject marker, Vector3 position)
    {
        yield return new WaitForSeconds(0.333f);
        marker.GetComponent<RectTransform>().position = position;
    }

    IEnumerator resetButtonPress(Transform button)
    {
        yield return new WaitForEndOfFrame();
        button.GetComponent<Animator>().SetBool("Press", false);

        yield return new WaitForSeconds(0.75f);
        canPressButton = true;
    }

    private void input()
    {
        if (Input.GetKeyDown(interact) && !dontThrow)
        {
            ThrowFunc();
        }

        if (isCuttingBoard && Input.GetMouseButtonDown(0) && knifeCooldown)
        {
            knifeCooldown = false;
            StartCoroutine("SliceKnife");
        }

        if (holdingStick && Input.GetMouseButtonDown(0) && canHitStick)
        {
            StickAnim.SetTrigger("Poke");
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactDistance, enemyHitMask))
            {
                canHitStick = false;
                hit.transform.GetComponent<Worm>().health--;
                StartCoroutine(StickCooldown());

                int random = Random.Range(0, 3);
                if (random == 0)
                {
                    MeatSound1.Play();
                }
                else if (random == 1)
                {
                    MeatSound2.Play();
                }
                else if (random == 2)
                {
                    MeatSound3.Play();
                }

                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                Instantiate(BloodSprayEffect, hit.point, rotation).GetComponent<SprayParticle>().parent = hit.transform.gameObject;
            }
        }

        if (isComputer && Input.GetMouseButtonDown(0))
        {
            ClickDown.Play();
        }
        else if (isComputer && Input.GetMouseButtonUp(0))
        {
            ClickUp.Play();
        }

        if (Input.GetKeyDown(Pause) && !paused && !EventHandler.i.IsInStartingAnim && !isComputer && !isCuttingBoard && !EventHandler.i.IsInEnd && !EventHandler.i.NotInDay)
        {
            paused = true;
            PauseMenu.SetActive(true);
            if (!isComputer && !isCuttingBoard)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            GetComponent<CameraRotation>().enabled = false;
        }
        else if (Input.GetKeyDown(Pause) && paused)
        {
            paused = false;
            PauseMenu.SetActive(false);
            if (!isComputer && !isCuttingBoard)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            GetComponent<CameraRotation>().enabled = true;
        }
        else if (Input.GetKeyDown(Pause) && isComputer && !clickedClockOut)
        {
            Computer();
        }
        else if (Input.GetKeyDown(Pause) && isCuttingBoard)
        {
            CuttingBoard();
        }
    }

    public void ThrowFunc()
    {
        if (holdingMeat)
        {
            if (holdingWood)
            {
                WoodHand.SetActive(false);
                GameObject piece = Instantiate(WoodPiece, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation);
                EventHandler handler = EventHandler.i;
                for (int i = 0; i < handler.WoodPieces.Length; i++)
                {
                    if (handler.WoodPieces[i] == null)
                    {
                        handler.WoodPieces[i] = piece;
                        break;
                    }

                    if (i == handler.WoodPieces.Length - 1)
                    {
                        if (handler.WoodInCooker.Contains(handler.WoodPieces[0]))
                        {
                            handler.WoodInCooker.Remove(handler.WoodPieces[0]);
                        }
                        Destroy(handler.WoodPieces[0]);
                        for (int j = 0; j < handler.WoodPieces.Length - 1; j++)
                        {
                            handler.WoodPieces[j] = handler.WoodPieces[j + 1];
                        }
                        handler.WoodPieces[handler.WoodPieces.Length - 1] = piece;
                    }
                }
                holdingWood = false;
            }
            else if (holdingJar)
            {
                JarHand.SetActive(false);
                holdingMeat = false;
                holdingJar = false;
                Instantiate(Jar, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation);
            }
            else if (holdingBag)
            {
                BagHand.SetActive(false);
                Instantiate(MeatBag, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation);
                holdingBag = false;
            }
            else
            {
                MeatHand.SetActive(false);
                if (holdingCookedLevel == "well")
                {
                    Instantiate(MeatPiece1, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation).GetComponent<Meat>().cookedWell = true;
                }
                else if (holdingCookedLevel == "light")
                {
                    Instantiate(MeatPiece1, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation).GetComponent<Meat>().cookedLight = true;
                }
                else
                {
                    Instantiate(MeatPiece1, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation);
                }
            }
            holdingMeat = false;
        }
        else if (holdingMeatLeft)
        {
            MeatHandLeft.SetActive(false);
            if (holdingCookedLevelLeft == "well")
            {
                Instantiate(MeatPiece1Left, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation).GetComponent<SmallMeat>().cookedWell = true;
            }
            else if (holdingCookedLevelLeft == "light")
            {
                Instantiate(MeatPiece1Left, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation).GetComponent<SmallMeat>().cookedLight = true;
            }
            else
            {
                Instantiate(MeatPiece1Left, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation);
            }
            holdingMeatLeft = false;
        }
        else if (holdingMeatRight)
        {
            MeatHandRight.SetActive(false);
            if (holdingCookedLevelRight == "well")
            {
                Instantiate(MeatPiece1Right, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation).GetComponent<SmallMeat>().cookedWell = true;
            }
            else if (holdingCookedLevelRight == "light")
            {
                Instantiate(MeatPiece1Right, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation).GetComponent<SmallMeat>().cookedLight = true;
            }
            else
            {
                Instantiate(MeatPiece1Right, transform.position + new Vector3(playerCamera.transform.forward.x, playerCamera.transform.forward.y + 0.75f, playerCamera.transform.forward.z) * 2, playerCamera.transform.rotation);
            }
            holdingMeatRight = false;
        }
        else if (holdingStick)
        {
            StickHand.SetActive(false);
            StickObj.SetActive(true);
            holdingStick = false;
        }
    }

    IEnumerator StickCooldown()
    {
        yield return new WaitForSeconds(0.417f);
        canHitStick = true;
    }

    private void CookerSwitch(Transform hit)
    {
        if (!canFlipCookerSwitch)
        {
            return;
        }

        if (!cookerSwitchFlipped)
        {
            CookerSwitchSound.pitch = 1f;
            CookerSwitchSound.Play();
            zap3.Play();

            hit.localRotation = Quaternion.Euler(-90f, 0, -130f);
            ElectricityEffectBurst.Play();
            cookerSwitchFlipped = true;
            if (EventHandler.i.WoodInCooker.Count != 0)
            {
                EventHandler.i.CookerOn = true;
                FireEffect.SetActive(true);
            }
            canFlipCookerSwitch = false;
            StartCoroutine(CookerSwitchCooldown());
        }
        else
        {
            CookerSwitchSound.pitch = 0.8f;
            CookerSwitchSound.Play();

            hit.localRotation = Quaternion.Euler(-90f, 0, 0);
            ElectricityEffectBurst.Play();
            cookerSwitchFlipped = false;
            EventHandler.i.CookerOn = false;
            FireEffect.SetActive(false);
            canFlipCookerSwitch = false;
            StartCoroutine(CookerSwitchCooldown());
        }
    }

    IEnumerator CookerSwitchCooldown()
    {
        yield return new WaitForSeconds(1f);
        canFlipCookerSwitch = true;
    }

    private void LightSwitch(Transform hit, bool overwrite)
    {
        if (lightSwitchCooldown && !overwrite)
        {
            return;
        }

        if (!EventHandler.i.LightsOff)
        {
            hit.localPosition = new Vector3(-0.213f, 0.042f, 2.74449e-08f);
            hit.localRotation = Quaternion.Euler(-125f, -90f, 90f);

            SwitchSound.pitch = 0.8f;
            SwitchSound.Play();
            handheldLights[0].enabled = false;
            handheldLights[1].enabled = false;
            handheldLights[2].enabled = false;
            light1.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");
            light2.GetComponent<MeshRenderer>().materials[3].DisableKeyword("_EMISSION");

            LightmapSettings.lightmaps = darkLightmap;
            RenderSettings.customReflectionTexture = null;
            StartCoroutine(SwitchProbeCubemap(darkProbe));

            EventHandler.i.LightsOff = true;

            ElectricityEffect.SetActive(true);
            lightSwitchCooldown = true;
            StartCoroutine(LightSwitchCooldown(hit));
        }
        else
        {
            hit.localPosition = new Vector3(-0.1301248f, -0.2042657f, 2.74449e-08f);
            hit.localRotation = Quaternion.Euler(-55f, -90f, 90f);

            SwitchSound.pitch = 1f;
            SwitchSound.Play();
            handheldLights[0].enabled = true;
            handheldLights[1].enabled = true;
            handheldLights[2].enabled = true;
            light1.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");
            light2.GetComponent<MeshRenderer>().materials[3].EnableKeyword("_EMISSION");

            LightmapSettings.lightmaps = brightLightmap;
            RenderSettings.customReflectionTexture = null;
            StartCoroutine(SwitchProbeCubemap(brightProbe));

            EventHandler.i.LightsOff = false;
        }
    }

    IEnumerator SwitchProbeCubemap(Cubemap nextCubemap)
    {
        probe.customBakedTexture = nextCubemap;

        yield return new WaitForEndOfFrame();

        probe.RenderProbe();
    }

    IEnumerator LightSwitchCooldown(Transform hit)
    {
        FlashLight.SetActive(true);
        StartCoroutine(LightSwitchElectric());
        yield return new WaitForSeconds(10f);
        FlashLight.SetActive(false);
        LightSwitch(hit, true);
        yield return new WaitForSeconds(20f);
        ElectricityEffect.SetActive(false);
        lightSwitchCooldown = false;
    }

    IEnumerator LightSwitchElectric()
    {
        zap1.Play();
        for (int i = 0; i < 30; i++)
        {
            int random = Random.Range(0, 3);
            if (random == 0)
            {
                int ran = Random.Range(0, 4);
                if (ran == 0)
                {
                    zap1.Play();
                }
                else if (ran == 1)
                {
                    zap2.Play();
                }
                else if (ran == 2)
                {
                    zap3.Play();
                }
                else if (ran == 3)
                {
                    zap4.Play();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void cameraAnim()
    {
        if (!isAnimatingComputer)
        {
            if (cuttingBoardCameraAnim)
            {
                if (Vector3.Distance(cuttingBoardCamera.position, cuttingBoardCamLoc.position) > 0.025f)
                {
                    cuttingBoardCamera.position = Vector3.Lerp(cuttingBoardCamera.position, cuttingBoardCamLoc.position, 4f * Time.deltaTime);
                    cuttingBoardCamera.rotation = Quaternion.Slerp(cuttingBoardCamera.rotation, cuttingBoardCamLoc.rotation, 4f * Time.deltaTime);
                }
                else
                {
                    isAnimatingCam = true;
                }
            }
            else if (!cuttingBoardCameraAnim && isAnimatingCam && !computerCameraAnim)
            {
                if (Vector3.Distance(cuttingBoardCamera.position, playerCamera.position) > 0.025f)
                {
                    cuttingBoardCamera.transform.position = Vector3.Lerp(cuttingBoardCamera.transform.position, playerCamera.position, 4f * Time.deltaTime);
                    cuttingBoardCamera.transform.rotation = Quaternion.Slerp(cuttingBoardCamera.transform.rotation, playerCamera.rotation, 4f * Time.deltaTime);
                }
                else
                {
                    isCuttingBoard = false;
                    sliceAnim.SetBool("IsCuttingBoard", false);

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    GetComponent<PlayerMove>().enabled = true;
                    GetComponent<CameraRotation>().enabled = true;

                    cuttingBoardCamera.gameObject.SetActive(false);
                    playerCamera.gameObject.SetActive(true);

                    isAnimatingCam = false;
                    canAnimateCam = true;
                    isAnimatingCuttingBoard = false;
                }
            }
        }

        if (!isAnimatingCuttingBoard)
        {
            if (computerCameraAnim)
            {
                if (Vector3.Distance(computerCamera.position, computerCamLoc.position) > 0.025f)
                {
                    computerCamera.transform.position = Vector3.Lerp(computerCamera.transform.position, computerCamLoc.position, 4f * Time.deltaTime);
                    computerCamera.transform.rotation = Quaternion.Slerp(computerCamera.transform.rotation, computerCamLoc.rotation, 4f * Time.deltaTime);
                }
                else
                {
                    isAnimatingCam = true;
                }
            }
            else if (!computerCameraAnim && isAnimatingCam && !cuttingBoardCameraAnim)
            {
                if (Vector3.Distance(computerCamera.position, playerCamera.position) > 0.025f)
                {
                    computerCamera.transform.position = Vector3.Lerp(computerCamera.transform.position, playerCamera.position, 4f * Time.deltaTime);
                    computerCamera.transform.rotation = Quaternion.Slerp(computerCamera.transform.rotation, playerCamera.rotation, 4f * Time.deltaTime);
                }
                else
                {
                    isComputer = false;

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    GetComponent<PlayerMove>().enabled = true;
                    GetComponent<CameraRotation>().enabled = true;
                    
                    if (!HumanoidMonster.GetComponent<Humanoid>().isScaring && !HeadMonster.GetComponent<LookingAt>().isScaring)
                    {
                        computerCamera.gameObject.SetActive(false);
                        playerCamera.gameObject.SetActive(true);
                    }

                    isAnimatingCam = false;
                    canAnimateCam = true;
                    isAnimatingComputer = false;
                }
            }
        }
    }

    private void Computer()
    {
        if (isComputer && isAnimatingCam)
        {
            isAnimatingComputer = true;
            computerCameraAnim = false;
        }
        else if (canAnimateCam)
        {
            GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);

            canAnimateCam = false;
            isComputer = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GetComponent<PlayerMove>().enabled = false;
            GetComponent<CameraRotation>().enabled = false;

            computerCamera.transform.position = playerCamera.transform.position;
            computerCamera.transform.rotation = playerCamera.transform.rotation;
            playerCamera.gameObject.SetActive(false);
            computerCamera.gameObject.SetActive(true);

            computerCameraAnim = true;
        }
    }

    private void CuttingBoard()
    {
        if (isCuttingBoard && isAnimatingCam)
        {
            isAnimatingCuttingBoard = true;
            cuttingBoardCameraAnim = false;
        }
        else if (canAnimateCam)
        {
            canAnimateCam = false;
            isCuttingBoard = true;
            sliceAnim.SetBool("IsCuttingBoard", true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GetComponent<PlayerMove>().enabled = false;
            GetComponent<CameraRotation>().enabled = false;

            cuttingBoardCamera.transform.position = playerCamera.transform.position;
            cuttingBoardCamera.transform.rotation = playerCamera.transform.rotation;
            playerCamera.gameObject.SetActive(false);
            cuttingBoardCamera.gameObject.SetActive(true);

            cuttingBoardCameraAnim = true;

            EventHandler.i.BoardMeat.transform.position = new Vector3(-38.4f, 1.5f, 31.7f);
            EventHandler.i.BoardMeat.GetComponent<Meat>().rb.linearVelocity = new Vector3(0,0,0);
        }
    }

    private void Door(Animator doorAnim)
    {
        if (doorAnim.GetBool("IsOpen") && canOpenOrCloseDoor)
        {
            canOpenOrCloseDoor = false;
            StartCoroutine(WaitForDoor(1.5f, "Main"));

            StartCoroutine(switchMarkerPosition(InteractMarkers[7], new Vector3(-16.19f, 2.26f, 22.65f)));
            doorAnim.SetBool("IsOpen", false);

            EventHandler.i.DoorClosed = true;
            DoorClose.Play();
        }
        else if (!doorAnim.GetBool("IsOpen") && canOpenOrCloseDoor)
        {
            canOpenOrCloseDoor = false;
            StartCoroutine(WaitForDoor(3f, "Main"));

            StartCoroutine(switchMarkerPosition(InteractMarkers[7], new Vector3(-18.224f, 2.26f, 18.198f)));
            doorAnim.SetBool("IsOpen", true);

            EventHandler.i.DoorClosed = false;
            DoorOpen.Play();

            StartCoroutine(EventHandler.i.DoorTimer());
        }
    }

    IEnumerator WaitForDoor(float time, string which)
    {
        yield return new WaitForSeconds(time);
        if (which == "Main")
        {
            canOpenOrCloseDoor = true;
        }
        else if (which == "Left")
        {
            canOpenOrCloseDoorLeft = true;
        }
        else if (which == "Right")
        {
            canOpenOrCloseDoorRight = true;
        }
    }

    private void Meat(bool cut, string cookedLevel, string tag = "", RaycastHit? hit = null)
    {
        if (cut && EventHandler.i.BoardMeat != null)
        {
            Vector3 pos = EventHandler.i.BoardMeat.transform.position;

            if (EventHandler.i.BoardMeat.GetComponent<Meat>().cookedWell)
            {
                Instantiate(MeatPiece1Left, new Vector3(pos.x, pos.y, pos.z), EventHandler.i.BoardMeat.transform.rotation).GetComponent<SmallMeat>().cookedWell = true;
                Instantiate(MeatPiece1Right, new Vector3(pos.x, pos.y, pos.z), EventHandler.i.BoardMeat.transform.rotation).GetComponent<SmallMeat>().cookedWell = true;
            }
            else if (EventHandler.i.BoardMeat.GetComponent<Meat>().cookedLight)
            {
                Instantiate(MeatPiece1Left, new Vector3(pos.x, pos.y, pos.z), EventHandler.i.BoardMeat.transform.rotation).GetComponent<SmallMeat>().cookedLight = true;
                Instantiate(MeatPiece1Right, new Vector3(pos.x, pos.y, pos.z), EventHandler.i.BoardMeat.transform.rotation).GetComponent<SmallMeat>().cookedLight = true;
            }
            else
            {
                Instantiate(MeatPiece1Left, new Vector3(pos.x, pos.y, pos.z), EventHandler.i.BoardMeat.transform.rotation);
                Instantiate(MeatPiece1Right, new Vector3(pos.x, pos.y, pos.z), EventHandler.i.BoardMeat.transform.rotation);
            }

            BloodSplatter.Play();

            Destroy(EventHandler.i.BoardMeat);
        }
        else if (hit != null)
        {
            if (tag == "Meat")
            {
                MeatHand.SetActive(true);
                if (cookedLevel == "well")
                {
                    MeatHandSmoke.SetActive(true);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(56, 36, 17, 255);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.3f);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(3f, 0.25f);
                }
                else if (cookedLevel == "light")
                {
                    MeatHandSmoke.SetActive(true);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(135, 70, 0, 255);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.4f);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(2f, 0.25f);
                }
                else
                {
                    MeatHandSmoke.SetActive(false);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(255, 255, 255, 255);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.5f);
                    MeatHandRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(0.25f, 0.25f);
                }
                holdingMeat = true;

                holdingCookedLevel = cookedLevel;

                int val = Random.Range(1, 4);
                if (val == 1)
                {
                    MeatSound1.Play();
                }
                else if (val == 2)
                {
                    MeatSound2.Play();
                }
                else if (val == 3)
                {
                    MeatSound3.Play();
                }

                EventHandler.i.BoardMeat = null;
                EventHandler.i.MeatOnBoard = false;
            }
            else if (tag == "MeatLeft")
            {
                MeatHandLeft.SetActive(true);
                if (cookedLevel == "well")
                {
                    MeatHandLeftSmoke.SetActive(true);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(56, 36, 17, 255);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.3f);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(3f, 0.25f);
                }
                else if (cookedLevel == "light")
                {
                    MeatHandLeftSmoke.SetActive(true);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(135, 70, 0, 255);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.4f);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(2f, 0.25f);
                }
                else
                {
                    MeatHandLeftSmoke.SetActive(false); ;
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(255, 255, 255, 255);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.5f);
                    MeatHandLeftRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(0.25f, 0.25f);
                }
                holdingMeatLeft = true;

                holdingCookedLevelLeft = cookedLevel;

                int val = Random.Range(1, 4);
                if (val == 1)
                {
                    MeatSound1.Play();
                }
                else if (val == 2)
                {
                    MeatSound2.Play();
                }
                else if (val == 3)
                {
                    MeatSound3.Play();
                }
            }
            else if (tag == "MeatRight")
            {
                MeatHandRight.SetActive(true);
                if (cookedLevel == "well")
                {
                    MeatHandRightSmoke.SetActive(true);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(56, 36, 17, 255);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.3f);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(3f, 0.25f);
                }
                else if (cookedLevel == "light")
                {
                    MeatHandRightSmoke.SetActive(true);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(135, 70, 0, 255);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.4f);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(2f, 0.25f);
                }
                else
                {
                    MeatHandRightSmoke.SetActive(false);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.color = new Color32(255, 255, 255, 255);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Glossiness", 0.5f);
                    MeatHandRightRend.GetComponent<SkinnedMeshRenderer>().material.mainTextureScale = new Vector2(0.25f, 0.25f);
                }
                holdingMeatRight = true;

                holdingCookedLevelRight = cookedLevel;

                int val = Random.Range(1, 4);
                if (val == 1)
                {
                    MeatSound1.Play();
                }
                else if (val == 2)
                {
                    MeatSound2.Play();
                }
                else if (val == 3)
                {
                    MeatSound3.Play();
                }
            }

            Destroy(hit.Value.transform.gameObject);
        }
    }

    private void Wood()
    {
        WoodHand.SetActive(true);
        holdingWood = true;
        holdingMeat = true;
    }

    private void WoodPieceFunc(RaycastHit hit)
    {
        WoodHand.SetActive(true);
        holdingWood = true;
        holdingMeat = true;

        Destroy(hit.transform.gameObject);
    }

    private void JarFunc(RaycastHit hit)
    {
        JarHand.SetActive(true);
        holdingMeat = true;
        holdingJar = true;

        Destroy(hit.transform.gameObject);
    }

    private void MakeBag()
    {
        if (!makeBagCooldown)
        {
            StartCoroutine("runBagger");
        }
    }

    IEnumerator runBagger()
    {
        makeBagCooldown = true;
        BaggerRun.Play();
        yield return new WaitForSeconds(BaggerRun.clip.length);
        Instantiate(MeatBag, new Vector3(-32f, 1.5f, 34.5f), Quaternion.identity);
        makeBagCooldown = false;
    }

    private void Bag(RaycastHit hit)
    {
        EventHandler.i.PiecesInHeldBag = hit.transform.GetComponent<Bag>().numPieces;

        BagHand.SetActive(true);
        holdingMeat = true;
        holdingBag = true;

        int val = Random.Range(1, 4);
        if (val == 1)
        {
            BagSound1.Play();
        }
        else if (val == 2)
        {
            BagSound2.Play();
        }
        else if (val == 3)
        {
            BagSound3.Play();
        }

        Destroy(hit.transform.gameObject);
    }

    private void Stick()
    {
        holdingStick = true;
        StickObj.SetActive(false);
        StickHand.SetActive(true);
    }

    private void Bagger(Animator baggerDoorAnim, string which)
    {
        if (baggerDoorAnim.GetBool("IsOpen"))
        {
            if (which == "Left" && canOpenOrCloseDoorLeft)
            {
                canOpenOrCloseDoorLeft = false;
                baggerDoorAnim.SetBool("IsOpen", false);

                StartCoroutine(WaitForDoor(1f, "Left"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[2], new Vector3(-32.243f, 4.37f, 32.882f)));
            }
            else if (which == "Right" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", false);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[3], new Vector3(-31.77f, 4.37f, 32.882f)));
            }
            else if (which == "LeftC" && canOpenOrCloseDoorLeft)
            {
                canOpenOrCloseDoorLeft = false;
                baggerDoorAnim.SetBool("IsOpen", false);

                StartCoroutine(WaitForDoor(1f, "Left"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[9], new Vector3(-25.204f, 0.284f, 32.882f)));
            }
            else if (which == "RightC" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", false);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[10], new Vector3(-24.731f, 0.284f, 32.882f)));
            }
            else if (which == "Blender" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", false);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[13], new Vector3(-30.338f, 4.779f, 10.559f)));
            }
            else if (which == "Trash" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", false);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[6], new Vector3(-25.065f, 3.26f, 10.605f)));
            }

            MetalDoorClose.Play();
        }
        else if (!baggerDoorAnim.GetBool("IsOpen") && canOpenOrCloseDoor)
        {
            if (which == "Left" && canOpenOrCloseDoorLeft)
            {
                canOpenOrCloseDoorLeft = false;
                baggerDoorAnim.SetBool("IsOpen", true);

                StartCoroutine(WaitForDoor(1f, "Left"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[2], new Vector3(-34.232f, 4.37f, 31.26f)));

            }
            else if (which == "Right" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", true);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[3], new Vector3(-29.723f, 4.37f, 31.26f)));
            }
            else if (which == "LeftC" && canOpenOrCloseDoorLeft)
            {
                canOpenOrCloseDoorLeft = false;
                baggerDoorAnim.SetBool("IsOpen", true);

                StartCoroutine(WaitForDoor(1f, "Left"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[9], new Vector3(-27.193f, 0.284f, 31.26f)));
            }
            else if (which == "RightC" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", true);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[10], new Vector3(-22.684f, 0.284f, 31.26f)));
            }
            else if (which == "Blender" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", true);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[13], new Vector3(-32.441f, 4.779f, 12.001f)));
            }
            else if (which == "Trash" && canOpenOrCloseDoorRight)
            {
                canOpenOrCloseDoorRight = false;
                baggerDoorAnim.SetBool("IsOpen", true);

                StartCoroutine(WaitForDoor(1f, "Right"));
                StartCoroutine(switchMarkerPosition(InteractMarkers[6], new Vector3(-27.313f, 3.26f, 12.481f)));
            }

            MetalDoorOpen.Play();
        }
    }

    private IEnumerator SliceKnife()
    {
        RaycastHit hit;
        Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactDistance, hitMask);

        sliceAnim.SetBool("Slice", true);
        yield return new WaitForSeconds(0.2f);
        KnifeCut.Play();
        yield return new WaitForSeconds(0.2f);
        Meat(true, "none");
        EventHandler.i.MeatOnBoard = false;
        yield return new WaitForSeconds(0.55f);
        sliceAnim.SetBool("Slice", false);
        yield return new WaitForSeconds(0.5f);
        knifeCooldown = true;
    }

    public void Access()
    {
        Computer();
    }

    public void AccessUnpause()
    {
        if (paused)
        {
            paused = false;
            PauseMenu.SetActive(false);
            if (!isComputer && !isCuttingBoard)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void StopBagger()
    {
        makeBagCooldown = false;
        StopCoroutine("runBagger");
    }
}
