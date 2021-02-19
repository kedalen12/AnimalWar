using System.Linq;
using UnityEngine;

public class PlayerInput:MonoBehaviour
{
    public LayerMask mask;

    public Transform player;
    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("LPlayer").transform;
    }
 
    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f));
        
        RaycastHit[] hitpoints = Physics.RaycastAll(ray).OrderBy(h => h.distance).ToArray(); //sort by distance
        foreach (RaycastHit hit in hitpoints)
        {
            var d = Vector3.Distance(player.position, hit.transform.position) > 2f;
            var dist = Vector3.Distance(player.position, hit.transform.position);
            Debug.Log(dist);
            if(hit.collider.gameObject != this.gameObject && !hit.collider.gameObject.CompareTag("LPlayer"))
            {
                if (d)
                {
                    Vector3 fromOriginToObject = hit.point - player.position; //~GreenPosition~ - *BlackCenter*
                    fromOriginToObject *= 2 / dist; //Multiply by radius //Divide by Distance
                    transform.position = player.position + fromOriginToObject; //*BlackCenter* + all that Math
                }
                else
                {
                    var newPos = hit.point;
                    transform.position = newPos;
                    break; //stop iterating   
                }
               
            }            
        }        
    }
}