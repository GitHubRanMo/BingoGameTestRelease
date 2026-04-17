using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
{
    //清除动画
    public AnimationClip clearAnimation;
    public AudioClip clearSource;
    private bool isBeingCleared = false;
    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }
    //后期扩展类
    protected GamePiece piece;
    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }
    public virtual void Clear()
    {
        //Debug.Log(piece);
        piece.GridRef.level.OnPieceCleared(piece);
        isBeingCleared= true;
        StartCoroutine(ClearCoroutine());
    }
    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play(clearAnimation.name);
            AudioSource.PlayClipAtPoint(clearSource,transform.position);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }
}
