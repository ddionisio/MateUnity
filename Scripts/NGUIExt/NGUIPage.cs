using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Make sure to put a number on each page within the pages holder.
/// For use with input manager, make sure to add NGUIInputClick to the page buttons
/// </summary>
[AddComponentMenu("M8/NGUI/Page")]
public class NGUIPage : MonoBehaviour, IComparer<GameObject> {
    public delegate void OnPageEnd();

    public UIButton prevButton;
    public UIButton nextButton;

    public UILabel pageLabel;
    public string pageLabelFormat;

    public GameObject endButton;

    public Transform pagesHolder;

    public OnPageEnd pageEndCallback;

    private int mCurPageInd = 0;
    private GameObject[] mPages;

    public void NextPage() {
        NextClick(gameObject);
    }

    public void End() {
        EndClick(gameObject);
    }

    public int Compare(GameObject x, GameObject y) {
        string xNumStr = Regex.Match(x.name, @"\d+").Value;
        string yNumStr = Regex.Match(y.name, @"\d+").Value;

        return int.Parse(xNumStr) - int.Parse(yNumStr);
    }

    void Awake() {
        mPages = new GameObject[pagesHolder.childCount];
        for(int i = 0; i < mPages.Length; i++) {
            mPages[i] = pagesHolder.GetChild(i).gameObject;
        }

        System.Array.Sort<GameObject>(mPages, this);

        if(prevButton != null)
            UIEventListener.Get(prevButton.gameObject).onClick += PrevClick;

        if(nextButton != null)
            UIEventListener.Get(nextButton.gameObject).onClick += NextClick;

        if(endButton != null)
            UIEventListener.Get(endButton.gameObject).onClick += EndClick;
    }

    // Use this for initialization
    void Start() {
        if(prevButton != null)
            prevButton.isEnabled = false;

        //for dynamic pages or for some reason there's just one page
        if(mPages.Length <= 1 && nextButton != null)
            nextButton.isEnabled = false;

        if(mPages.Length >= 1) {
            mPages[0].SetActive(true);

            for(int i = 1; i < mPages.Length; i++) {
                mPages[i].SetActive(false);
            }
        }

        RefreshPageLabel();
    }

    // Update is called once per frame
    void Update() {

    }

    void RefreshPageLabel() {
        if(pageLabel != null && !string.IsNullOrEmpty(pageLabelFormat)) {
            pageLabel.text = string.Format(pageLabelFormat, mCurPageInd + 1, mPages.Length);
        }
    }

    void PrevClick(GameObject go) {
        if(mCurPageInd > 0) {
            //TODO: fancy transition
            mPages[mCurPageInd].SetActive(false);

            mCurPageInd--;

            mPages[mCurPageInd].SetActive(true);

            if(mCurPageInd == 0 && prevButton != null) {
                prevButton.isEnabled = false;
            }

            if(nextButton != null && !nextButton.isEnabled)
                nextButton.isEnabled = true;

            RefreshPageLabel();
        }
    }

    void NextClick(GameObject go) {
        int endInd = mPages.Length - 1;

        if(mCurPageInd < endInd) {
            //TODO: fancy transition
            mPages[mCurPageInd].SetActive(false);

            mCurPageInd++;

            mPages[mCurPageInd].SetActive(true);

            if(mCurPageInd == endInd && pageEndCallback == null && nextButton != null)
                nextButton.isEnabled = false;

            if(prevButton != null && !prevButton.isEnabled) {
                prevButton.isEnabled = true;
            }

            RefreshPageLabel();
        }
        else if(mCurPageInd == endInd && pageEndCallback != null) {
            pageEndCallback();
        }
    }

    void EndClick(GameObject go) {
        if(pageEndCallback != null) {
            pageEndCallback();
        }
    }
}
