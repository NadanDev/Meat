using UnityEngine;

public class LookingAt : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] GameObject Target;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Head;
    [SerializeField] GameObject CamHolder;
    [SerializeField] GameObject PlayerCam;
    [SerializeField] GameObject ThisCam;
    [SerializeField] GameObject StartDayButton;
    [SerializeField] GameObject TempImage;
    [SerializeField] Animator StartUI;

    [SerializeField] GameObject[] SmallSpiders;

    private float speed = 1f;
    public bool isScaring;

    private void Update()
    {
        if (!isScaring)
        {
            float step = speed * Time.deltaTime;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Cam);
            if (CamHolder.activeSelf && GeometryUtility.TestPlanesAABB(planes, Target.GetComponent<Renderer>().bounds))
            {}
            else
            {
                if (EventHandler.i.SpiderStage == 0)
                {
                    SmallSpiders[0].SetActive(false);
                    SmallSpiders[1].SetActive(false);
                    SmallSpiders[2].SetActive(false);
                    SmallSpiders[3].SetActive(false);
                    SmallSpiders[4].SetActive(false);
                    SmallSpiders[5].SetActive(false);
                    SmallSpiders[6].SetActive(false);
                    Head.SetActive(false);
                    Head.transform.position = new Vector3(-18.11f, 0.186f, 14.57f);
                    Head.transform.rotation = Quaternion.Euler(-8.364f, -48.652f, -0.658f);
                }
                else if (EventHandler.i.SpiderStage == 1)
                {
                    SmallSpiders[0].SetActive(true);
                    SmallSpiders[1].SetActive(true);
                    Head.SetActive(false);
                }
                else if (EventHandler.i.SpiderStage == 2)
                {
                    SmallSpiders[0].SetActive(true);
                    SmallSpiders[1].SetActive(true);
                    SmallSpiders[2].SetActive(true);
                    SmallSpiders[3].SetActive(true);
                    SmallSpiders[4].SetActive(true);
                    Head.SetActive(false);
                }
                else if (EventHandler.i.SpiderStage == 3)
                {
                    SmallSpiders[0].SetActive(true);
                    SmallSpiders[1].SetActive(true);
                    SmallSpiders[2].SetActive(true);
                    SmallSpiders[3].SetActive(true);
                    SmallSpiders[4].SetActive(true);
                    SmallSpiders[5].SetActive(true);
                    SmallSpiders[6].SetActive(true);
                    Head.SetActive(true);
                }
                else if (EventHandler.i.SpiderStage == 4)
                {
                    SmallSpiders[0].SetActive(true);
                    SmallSpiders[1].SetActive(true);
                    SmallSpiders[2].SetActive(true);
                    SmallSpiders[3].SetActive(true);
                    SmallSpiders[4].SetActive(true);
                    SmallSpiders[5].SetActive(true);
                    SmallSpiders[6].SetActive(true);
                    Head.SetActive(true);
                    Head.transform.position = Vector3.MoveTowards(Head.transform.position, new Vector3(Player.transform.position.x, 0.186f, Player.transform.position.z), step);
                    Vector3 towards = Player.transform.position - Head.transform.position;
                    Quaternion rot = Quaternion.LookRotation(towards);
                    Head.transform.rotation = rot;
                }
                else if (EventHandler.i.SpiderStage == 5)
                {
                    SmallSpiders[0].SetActive(true);
                    SmallSpiders[1].SetActive(true);
                    SmallSpiders[2].SetActive(true);
                    SmallSpiders[3].SetActive(true);
                    SmallSpiders[4].SetActive(true);
                    SmallSpiders[5].SetActive(true);
                    SmallSpiders[6].SetActive(true);
                    Head.SetActive(true);
                    Head.transform.position = Vector3.MoveTowards(Head.transform.position, new Vector3(Player.transform.position.x, 0.186f, Player.transform.position.z), step * 4);
                    Vector3 towards = Player.transform.position - Head.transform.position;
                    Quaternion rot = Quaternion.LookRotation(towards);
                    Head.transform.rotation = rot;
                }
            }
        }
    }
}
