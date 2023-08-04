using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchFor : IState {
    LayerMask searchLayer;
    GameObject owner;
    float searchRadius;
    private string tagToLookFor;
    private Vector3 objPos;
    private System.Action<SearchResult> searchResultsCallback;

    private System.Action<CloseToEnemy> close;
    private float distance;
    private Vector3 reach;


    private bool isChasing;
    //NavMeshAgent agent;
    public SearchFor(LayerMask searchLayer, GameObject owner, float searchRadius, string tagToLookFor, System.Action<SearchResult> searchResultsCallback, System.Action<CloseToEnemy> close,float distance, Vector3 reach)
    {
        this.searchLayer = searchLayer;
        this.owner = owner;
        this.searchRadius = searchRadius;
        this.tagToLookFor = tagToLookFor;
        this.searchResultsCallback = searchResultsCallback;
        //this.agent = agent;

        this.close = close;
        this.distance = distance;
        this.reach = reach;
    }

    public void Enter()
    {
        Debug.Log("entered chase");
    }
    public void Execute()
    {
        var hitObj = Physics.OverlapSphere(this.owner.transform.position, this.searchRadius, searchLayer);
        var allObjWithTag = new List<Collider>();
        for (int i =0; i < hitObj.Length;i++)
        {
            if (hitObj[i].CompareTag(this.tagToLookFor))
            {
                allObjWithTag.Add(hitObj[i]);
                objPos = hitObj[i].transform.position;
                isChasing = true;
            }
        }
        if(hitObj.Length == 0)
        {
            isChasing = false;
        }
        //Debug.Log("Enemy is chasing = " + isChasing);
        var searchResults = new SearchResult(hitObj, allObjWithTag, objPos, isChasing);
        this.searchResultsCallback(searchResults);

        reach = owner.transform.TransformDirection(Vector3.forward); //might be able to be more OOP'd
        Debug.DrawRay(owner.transform.position, reach * distance, Color.red);
        
        var closeToEnemy = new CloseToEnemy();
        RaycastHit hit;

        if (Physics.Raycast(owner.transform.position, reach, out hit, distance, searchLayer))
        {
            if (hit.collider.gameObject.tag == tagToLookFor)
            {
                this.close(closeToEnemy);   
            }
        }
    }
    public void Exit()
    {

    }
}

public class SearchResult
{
    public Collider[] AllHitObjinRad;
    public List<Collider> AllHitObjWithTag;
    public Vector3 posObj;
    public bool isChase;

    public SearchResult(Collider[] AllHitObjinRad, List<Collider> AllHitObjWithTag, Vector3 objPos, bool isChase)
    {
        this.AllHitObjinRad = AllHitObjinRad;
        this.AllHitObjWithTag = AllHitObjWithTag;
        this.posObj = objPos;
        this.isChase = isChase;
    }
}

public class CloseToEnemy
{
    public CloseToEnemy()
    {

    }

}
