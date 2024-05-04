using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour
{
    public Transform targetLocattion;
    public float minCamY, maxCamY, minCamX, maxCamX, minTrackSpeed, maxTrackSpeed, disMaxTrigger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newCamLoc = transform.position;

        // create speed based on distance to player
        float trackDiffBonus = (maxTrackSpeed - minTrackSpeed) * (Vector3.Distance(targetLocattion.position, new Vector3(transform.position.x, transform.position.y, targetLocattion.position.z))/disMaxTrigger);

        //move camera towrds players position keeying the Z coordinate locked
        newCamLoc = Vector3.MoveTowards(newCamLoc, targetLocattion.position, (minTrackSpeed + trackDiffBonus) * Time.deltaTime);
        newCamLoc.z = transform.position.z;
        newCamLoc.x = Mathf.Clamp(newCamLoc.x, minCamX, maxCamX);
        newCamLoc.y = Mathf.Clamp(newCamLoc.y, minCamY, maxCamY);
        transform.position = newCamLoc;
    }
}
